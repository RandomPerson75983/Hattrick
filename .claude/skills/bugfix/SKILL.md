# Bug Fix Skill

Diagnose bugs properly, then fix using sequential TDD agents with handoff file.

## State Files

- `.claude/bugfix-handoff.json` — Context passed between agents (deleted after fix)

---

## Phase 1 — Diagnosis

1. **Gather bug details**
   - What happens? What should happen?
   - Steps to reproduce

2. **Trace the execution path**
   - From user action, trace the complete code path to failure
   - Read ALL relevant files (components → services → repositories)

3. **Identify root cause**
   ```
   Root Cause Analysis:
   - Symptom: [what the user sees]
   - Execution path: [file1 → file2 → file3]
   - Root cause: [location and reason]
   ```

4. **WAIT for user to confirm diagnosis before proceeding**

5. **Create handoff file**
   ```json
   // .claude/bugfix-handoff.json
   {
     "symptom": "Brief description of what user sees",
     "rootCause": "File:line and reason",
     "executionPath": ["file1", "file2", "file3"],
     "testFiles": [],
     "implFiles": [],
     "notes": []
   }
   ```

---

## Phase 2 — Sequential TDD Fix

### Step 1: @test-writer
```
Spawn agent (subagent_type: "general-purpose", name: "test-writer")
Prompt: Read .claude/skills/sprint/test-writer.md
        Read .claude/bugfix-handoff.json for bug details
        Write a reproduction test that FAILS before the fix
        Update handoff.testFiles with path
        Update handoff.notes with any discoveries
```

### Step 2: @test-verifier
```
Spawn agent (subagent_type: "general-purpose", name: "test-verifier")
Prompt: Read .claude/skills/sprint/test-verifier.md
        Read .claude/bugfix-handoff.json for bug context
        Review the tests written by @test-writer for quality and coverage
        Report PASS or FAIL with specific issues
```

### Step 2b: Handle test-verifier result
- **PASS (no issues at all):** Continue to Step 3
- **FAIL:** Respawn @test-writer with the specific issues, then re-verify with @test-verifier
- **"PASS with suggestions/improvements":** Treat as FAIL. Any suggestion means tests aren't strong enough.
- Iterate until @test-verifier reports a clean PASS — do NOT proceed to @coder with weak tests

### Step 3: @coder
```
Spawn agent (subagent_type: "general-purpose", name: "coder")
Prompt: Read .claude/skills/sprint/coder.md
        Read .claude/bugfix-handoff.json for context
        Fix the root cause to make the test pass
        Update handoff.implFiles with modified files
        Update handoff.notes with fix details
```

### Step 4: 21-Agent Verification Review

Run the full test suite first:
```bash
dotnet test BaseballManager.Tests/BaseballManager.Tests.csproj
```
If tests fail, respawn @coder with failure details and re-run tests. Do NOT proceed to review with failing tests.

Once tests pass, identify all changed files from the handoff:
```
changedFiles = handoff.testFiles + handoff.implFiles
```

Generate a timestamp directory for findings:
```
timestamp=$(date +%Y%m%d_%H%M%S)
mkdir -p .claude/review-findings/$timestamp
```

Spawn 21 review agents in batches (7-7-7). Each reviews ONLY the changed files.

| Batch | Agents |
|-------|--------|
| 1 | 01-07: Magic Numbers, Null Safety, Thread Safety, Blazor, Test Quality, Player State, Repository |
| 2 | 08-14: Architecture, Plan Compliance, Logic Errors, Edge Cases, Performance, LINQ, Save/Load |
| 3 | 15-21: Resource Management, Assertion Quality, Error Handling, Duplicate Code, Dead Code, Over-Engineering, Security Basics |

**Agent prompt:**
```
Spawn agent (subagent_type: "general-purpose", name: "reviewer-XX")
Prompt: Read C:\Projects\BaseballManager\Docs\CODE_REVIEW_AGENTS\{XX_NAME}.md
        Review ONLY these files for issues in your specialty: [changedFiles]
        Also read .claude/bugfix-handoff.json for context on what was fixed
        Report: File, line, description, severity (Critical/High/Medium/Low), suggested fix
        Write findings to .claude/review-findings/{timestamp}/{XX_NAME}.json
        Write an empty array [] if no issues found.
```

### Step 5: Automated Synthesis

After all 21 review agents complete, run post-review agents **sequentially**:

1. **@deduplicator** — Read `Docs/CODE_REVIEW_AGENTS/22_DEDUPLICATION.md`, process `.claude/review-findings/{timestamp}/`, write `dedup-report.json`
2. **@validator** — Read `Docs/CODE_REVIEW_AGENTS/23_VALIDATION.md`, read `Docs/CLAUDE.md` for conventions, verify findings against source code, write `validation-report.json`

### Step 6: Handle validated results

1. **If Critical or High confirmed issues found:**
   - Create review-handoff.json with issues grouped by file
   - Fix using @test-writer → @test-verifier → @coder cycle (Steps 1-3)
   - Re-run 21-agent review on newly changed files
   - Iterate until no Critical/High issues remain
2. **If only Medium/Low confirmed issues found:**
   - Fix directly (these are convention/style issues, not logic bugs)
   - No need to re-review
3. **If all CLEAN:** Continue to Phase 3

---

## Phase 3 — Final Report

1. **Run full test suite:** `dotnet test BaseballManager.Tests/BaseballManager.Tests.csproj`

2. **Delete handoff and review files**

3. **Report results:**
   ```
   Bug Fix Complete:
   - Symptom: [what the user saw]
   - Root cause: [location and reason]
   - Reproduction test: [test name]
   - Fix: [file:line]
   - Tests passing: X/X
   - Review: X agents clean, Y issues found and fixed
   ```

---

## Rules

- NEVER guess — always trace the full path first
- NEVER fix before user confirms diagnosis
- NEVER skip the reproduction test
- Use sequential TDD agents — never fix directly
- Handoff file passes context between agents
- Launch review agents in batches (7-7-7), never all 21 at once
- Do NOT skip the 21-agent review — it is the final quality gate
