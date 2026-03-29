# Phase Plan Completion Review Agent

**Your sole focus:** Verify the ENTIRE phase plan is complete, including Phase Completion Checklist.

**IMPORTANT:** This agent runs ONLY for full phase reviews, not sprint reviews.

## Pre-Review Setup

1. Identify the current phase plan file (e.g., `Docs/IMPLEMENTATION_PLAN.md`)
2. Read the ENTIRE document, especially:
   - Phase Completion Checklist (usually near the end)
   - Integration requirements
   - Cleanup tasks
   - "Before closing this phase" sections
3. Create a checklist of EVERY discrete item

## Critical Sections to Check

### Phase Completion Checklist

Many plans have a section like:

```markdown
## Phase Completion Checklist

Before closing this phase:

1. [ ] Update X system with new Y
2. [ ] Verify integration test passes
3. [ ] Add validation for Z
```

**Every single item in this section MUST be verified as complete.**

### Integration Requirements

Look for phrases like:
- "Update match engine"
- "Incorporate into season simulation"
- "Add to PlayerGenerationService"
- "Wire into game loop"
- "Integrate with save/load"

### Cleanup Tasks

Look for:
- "Remove after X is implemented"
- "Delete once Y can handle Z"
- Temporary hacks that should be removed

## Verification Process

For EACH planned item:

| Check | Description |
|-------|-------------|
| Exists | Code implementing this item exists |
| Complete | Not stubbed, partial, or TODO |
| Tested | Unit tests verify the functionality |
| Integrated | Wired into all relevant systems |

## Common Missed Items

1. **Phase Completion Checklist items** - Often at document end, easily skipped
2. **Season simulation updates** - New systems not incorporated into match/week engine
3. **Player generation updates** - New player data not generated for initial squads
4. **SaveGameMapper updates** - New data not exported/imported
5. **Integration test updates** - New assertions not added
6. **Cleanup tasks** - Temporary code not removed

## Cross-System Integration Checklist

For each new system/feature introduced in the phase:

- [ ] **New Game Flow** - Generated for initial squads?
- [ ] **Season Simulation** - Integrated into weekly/match processing?
- [ ] **Save Game** - Included in SaveGameDataDto?
- [ ] **Load Game** - Restored from save file?
- [ ] **Player Generation** - Applied when creating new players?

## Reporting Format

```
Phase Plan Completion Report:

Phase: [Phase Name]
Plan File: [File Path]

=== Phase Completion Checklist ===
Total items: X
Completed: Y
MISSING: Z

Missing Items:
1. [CRITICAL] [Item description] - not implemented
2. [CRITICAL] [Item description] - partially implemented

=== Integration Requirements ===
[List any "integrate with X" items that are missing]

=== Cleanup Tasks ===
[List any cleanup items not done]
```

## Severity

- Phase Completion Checklist item missing: **Critical**
- Integration requirement not done: **Critical**
- Cleanup task not done: **High**
- Partial implementation: **High**
- Missing tests for phase feature: **High**
