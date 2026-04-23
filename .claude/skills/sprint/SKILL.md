# Sprint Workflow

Execute a development sprint using sequential single-task agents to enforce TDD discipline.

## State Files

- `Docs/sprint-state.json` — Sprint progress (survives compaction)
- `Docs/quartet-handoff.json` — Context passed between agents (deleted after each quartet)

---

## PRE-FLIGHT — MODEL CHECK

**This runs FIRST, before anything else.**

If the sprint is following a phase plan (e.g., `Docs/PHASE_9_DYNAMIC_EVENTS.md`):

1. Check the plan for `Recommended Model` and `Recommended Effort` fields on the current section/sprint
2. Check what model you are running (find your model ID in the environment info)
3. Compare model:
   - If they match → Check effort, then continue to Phase 1
   - If they differ → **STOP IMMEDIATELY.** Output:
     ```
     MODEL MISMATCH: This sprint recommends [recommended model] but you are running [current model].
     Please /compact and switch to the recommended model with /model before re-running /sprint.
     ```
   - Do NOT proceed with planning or execution on the wrong model.
4. Report recommended effort level to the user:
   ```
   Recommended effort for this sprint: [effort level]. Set with /config.
   ```

### Model & Effort Guide

| Model | Effort | Use For |
|-------|--------|---------|
| Opus | High | New architecture, complex algorithms, cross-system integration |
| Sonnet | Medium-High | Implementation following established patterns, standard services |
| Haiku | Low | Models, DTOs, enums, interfaces, simple property classes |

---

## PHASE 1 — PLANNING

1. **Check for existing sprint state**
   - If `Docs/sprint-state.json` exists → Resume from saved position
   - If not → Continue with planning

2. **Analyze the feature**
   - Break into discrete quartets (test → review → implement → verify)
   - Each quartet = one unit of work (model, service method, UI component, etc.)

3. **Create sprint state**
   ```json
   {
     "sprintName": "Feature Name",
     "quartets": [
       { "name": "Add FooService", "description": "Brief requirements" }
     ],
     "currentQuartet": 0,
     "currentStep": "test-writer",
     "completedQuartets": []
   }
   ```

---

## PHASE 2 — EXECUTE QUARTETS

For each quartet, run this sequence:

### Step 1: @test-writer

Create handoff file first:
```json
// Docs/quartet-handoff.json
{
  "quartet": "PositionPlayerReadinessService",
  "requirements": "Formula: 50 + (min(games,10) * 5). Modifier 0.9 if <3 games.",
  "testFiles": [],
  "implFiles": [],
  "notes": []
}
```

Then spawn:
```
Spawn agent (subagent_type: "general-purpose", name: "test-writer")
Prompt: Read .claude/skills/sprint/test-writer.md
        Read Docs/quartet-handoff.json for requirements
        Write tests for: [quartet name]
        Update handoff.testFiles with paths you created
        Update handoff.notes with any gotchas discovered
```

Update sprint-state.json: `currentStep = "test-verifier"`

### Step 2: @test-verifier
```
Spawn agent (subagent_type: "general-purpose", name: "test-verifier")
Prompt: Read .claude/skills/sprint/test-verifier.md
        Read Docs/quartet-handoff.json for requirements context
        Review the tests written by @test-writer for quality and coverage
        Report PASS or FAIL with specific issues
```

### Step 2b: Handle test-verifier result
- **If PASS (no issues at all):** Continue to Step 3
- **If FAIL:** Respawn @test-writer with the specific issues, then re-verify with @test-verifier
- **If "PASS with suggestions/improvements":** Treat as FAIL. Any suggestion means tests aren't strong enough.
- Iterate until @test-verifier reports a clean PASS — do NOT proceed to @coder with weak tests

Update sprint-state.json: `currentStep = "coder"`

### Step 3: @coder
```
Spawn agent (subagent_type: "general-purpose", name: "coder")
Prompt: Read .claude/skills/sprint/coder.md
        Read Docs/quartet-handoff.json for context
        Implement to make tests pass
        Update handoff.implFiles with paths you created
        Update handoff.notes with key decisions
```

Update sprint-state.json: `currentStep = "verifier"`

### Step 4: @verifier
```
Spawn agent (subagent_type: "general-purpose", name: "verifier")
Prompt: Read .claude/skills/sprint/verifier.md
        Read Docs/quartet-handoff.json for file paths and context
        Verify the quartet
```

### Step 5: Handle result
- **If PASS:** Commit the quartet, delete handoff file, advance to next quartet
- **If FAIL:** Respawn @coder with failure details, then re-verify

### Step 6: Commit
```bash
git add [files from handoff.testFiles and handoff.implFiles]
git commit -m "feat: [quartet name]

Co-Authored-By: Claude <noreply@anthropic.com>"
```

Delete `Docs/quartet-handoff.json`

Update sprint-state.json: advance `currentQuartet`, reset `currentStep` to "test-writer"

**Repeat for all quartets.**

---

## PHASE 3 — FULL TEST SUITE

```bash
dotnet test Hattrick.Tests/Hattrick.Tests.csproj
```

ALL tests must pass before proceeding.

---

## PHASE 4 — DOCUMENTATION & CLEANUP

1. Update `Docs/STATUS.md` with session entry
2. Update `MEMORY.md` if needed
3. Delete `Docs/sprint-state.json`
4. Report final summary with test count
5. Run `/review` for code review

---

## RECOVERY (After Compaction)

1. Read `Docs/sprint-state.json`
2. Rebuild: `dotnet build Hattrick.Tests/Hattrick.Tests.csproj --verbosity quiet`
3. If `Docs/quartet-handoff.json` exists, read it for context
4. Resume from `currentQuartet` + `currentStep`

---

## Handoff File Purpose

The handoff file passes context between agents so they don't re-discover everything:
- **test-writer** creates it with requirements, updates with test paths and gotchas
- **test-verifier** reads it, reviews test quality, reports PASS/FAIL with issues
- **coder** reads it, updates with impl paths and decisions
- **verifier** reads it to know exactly what to check
- **leader** deletes it after commit

Keep it concise — file paths, key formulas, gotchas. NOT full file contents.

---

## Reminders

- **TDD IS NOT OPTIONAL** — Tests come first
- **YOU ARE THE LEADER** — Spawn agents, don't implement yourself
- **COMMIT AFTER EACH QUARTET** — Don't batch all commits
- **HANDOFF FILE IS CONTEXT** — Keep it lean, delete after commit
