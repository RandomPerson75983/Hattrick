# CLAUDE.md - Hattrick Clone: Football Manager Project Rules

**This file is auto-loaded by Claude Code and all sub-agents. Follow ALL rules below.**

## Core Principles

**Don't be lazy. Don't defer any issues you find.**

## Project Overview

This is a .NET Hattrick clone - a single-player football management simulation game. Tech stack: C#, .NET 10, MAUI Blazor. Always run full test suite after changes and ensure all tests pass before reporting completion.

## Bug Fixing - REQUIRED WORKFLOW

**NEVER attempt fixes based on assumptions. Reproduce first, then fix.**

1. **Create a reproduction test FIRST** - Write a failing test that demonstrates the exact bug before writing any fix code
2. **Trace the full code path** - Read all relevant files and identify the root cause, not just the symptom
3. **Address the FULL root cause in a single attempt** - Do not make incremental partial fixes
4. **Run the full test suite** - Verify the fix AND check for regressions
5. **If the first fix doesn't work** - Step back and re-examine the architecture rather than patching

```
BAD: "I think the bug is in X, let me try changing Y"
GOOD: "Let me write a test that reproduces this, trace the execution, then fix the root cause"
```

For straightforward bugs, go directly to implementation. Do NOT enter plan mode or over-engineer solutions for simple fixes. Reserve planning for multi-step features or architectural changes.

## Code Reviews - REQUIRED

Run `/review` to spawn 23 specialized agents. Each agent reads its focus file from `Docs/CODE_REVIEW_AGENTS/`:

| # | Focus | Top Issues |
|---|-------|------------|
| 01 | Magic Numbers | Literals → named constants |
| 02 | Null Safety | `.Value` checks, error handling |
| 03 | Thread Safety | Locks, IRandomProvider, no DateTime.Now |
| 04 | Blazor Patterns | CultureInfo, async void try-catch |
| 05 | Test Quality | Naming, bUnit v2, weak assertions, formula testing |
| 06 | Game State | Player roster, team data, season state |
| 07 | Repository Patterns | IReadOnlyList, locking |
| 08 | Architecture | DI, interfaces, separation |
| 09 | Plan Compliance | All items implemented |
| 10 | Logic Errors | Wrong conditions, inverted logic |
| 11 | Edge Cases | Empty collections, zero values |
| 12 | Performance | O(n²), LINQ in loops, allocations |
| 13 | LINQ Gotchas | Deferred execution, closures |
| 14 | Save/Load | DTO mapping, data loss |
| 15 | Resources | IDisposable, event leaks, caches |
| 16 | Assertion Quality | Weak assertions, formula testing, component isolation |
| 17 | Error Handling | Swallowed exceptions, missing try-catch, wrong types |
| 18 | Duplicate Code | Copy-pasted logic, redundant methods, repeated patterns |
| 19 | Dead Code | Unused imports, unreachable code, commented-out blocks |
| 20 | Over-Engineering | Premature abstractions, unnecessary wrappers, excess complexity |
| 21 | Security Basics | Hardcoded secrets, exposed internals, insecure defaults |
| 22 | Deduplication | Post-review: removes duplicate findings across agents |
| 23 | Validation | Post-review: verifies findings against actual code, removes false positives |

All agents write findings to `.claude/review-findings/{timestamp}/` (timestamped per run) for compaction resilience.

Track and report: "Review complete. Total: X issues found, Y confirmed, Z fixed."

## Implementation Plans - REQUIRED

Follow the implementation plan exactly. Do not skip steps or phases. Before marking any milestone complete, cross-reference every item in the plan and confirm each was implemented.

## Refactoring - REQUIRED

When renaming or refactoring across the codebase, do a COMPLETE grep for ALL occurrences including:
- Page titles and window titles
- Breadcrumbs and navigation labels
- File paths and namespaces
- Comments and documentation
- Test assertions and mock data

Verify NO occurrences remain before reporting completion.

## Startup

1. Read `Docs/STATUS.md` - current status, test count, what's next (~40 lines)
2. Read the current implementation plan (e.g., `Docs/IMPLEMENTATION_PLAN.md`)
3. Run tests: `dotnet test Hattrick.Tests/Hattrick.Tests.csproj`
4. `Docs/DEVELOPMENT_RULES.md` has full examples/rationale if needed - but this file contains all the rules.

## Architecture Rules

- **Blazor components have ZERO business logic.** Components call services. Services call repositories. Never put calculations, simulation logic, or data manipulation in .razor files.
- **All services have interfaces.** `ITrainingService` -> `TrainingService`. Register in DI. Inject via constructor.
- **Constructor injection only.** Never `new Service()` or `new Repository()` inside another class.
- **Functions <= 30 lines, classes <= 300 lines.** If longer, decompose. One method = one job.
- **All gameplay is in-memory, all persistence is JSON.** No database - SQLite/EF Core were removed. In-memory repositories hold gameplay state; JSON files on disk handle saves. See `ISaveGameService`. NavMenu save/load uses `ISaveSlotService` (not `ISaveGameService` directly). Save slots 1-99 are manual, 100+ are auto-save sub-slots. File writes use atomic write-to-temp-then-rename pattern.
- **All names are fictional.** No real football team names, player names, or city names. Ever.
- **Never `.Result` or `.Wait()` on async methods.** Always `await`. Only exception: MAUI startup bootstrap.
- **Game formulas must match Hattrick rules.** Check wiki_archive/ and official manual for accuracy. Document formula sources in comments.

## TDD (Test-Driven Development) - REQUIRED

```
RED: Write a failing test FIRST
GREEN: Write minimal code to make it pass
REFACTOR: Refactor while keeping tests green
```

- Every bug fix MUST include a regression test that would have caught it.
- Statistical tests use 1000+ sample sizes. If flaky, increase samples - don't loosen assertions.
- FluentAssertions v7.x only (Apache 2.0). **DO NOT upgrade to v8+** (commercial license).
- Use NSubstitute if mocking is needed, not Moq.
- Test naming: `MethodName_Scenario_ExpectedResult()`

## Code Review Quick Checklist

Top 10 rules (details in `Docs/CODE_REVIEW_AGENTS/*.md`):

1. **No magic numbers** - Every literal → named constant
2. **CultureInfo.InvariantCulture** - On ALL `.ToString("format")`
3. **Null-check before .Value** - Never rely on `!` alone
4. **async void try-catch** - Wrap ALL async void handlers
5. **IRandomProvider** - Never `Random.Shared` or `new Random()`
6. **DateTime parameter** - Never `DateTime.Now` in services
7. **IReadOnlyList returns** - Not `List<T>` from repos
8. **Lock on repos** - `private readonly Lock _lock = new();`
9. **Test naming** - `MethodName_Scenario_ExpectedResult()`
10. **bUnit v2** - `BunitContext` + `Render<T>()`, not v1 API

## Git Commits

```
feat: Short description    (new feature)
fix: Short description     (bug fix)
test: Short description    (adding tests)
refactor: Short description (no behavior change)
```

## Project Structure

```
C:\Projects\hattrick\
├── CLAUDE.md                # This file (auto-loaded)
├── MEMORY.md                # Session memory and game systems reference
├── Docs\
│   ├── STATUS.md            # Current state (~40 lines, read every session)
│   ├── IMPLEMENTATION_PLAN.md # Current implementation plan
│   ├── CODE_REVIEW_AGENTS\  # Specialized agent instructions (23 files)
│   ├── DEVELOPMENT_RULES.md # Full reference with examples
│   └── FORMULAS.md          # Game formulas and calculations
├── Introduction » Manual » Help » Hattrick.pdf  # Official game manual
├── wiki_archive/            # Community wiki with reverse-engineered formulas
├── Hattrick/                # Main MAUI Blazor project
│   ├── Models/
│   ├── Services/
│   ├── Repositories/        # All in-memory (no database)
│   └── Components/
└── Hattrick.Tests/
```

## Sub-Agent Instructions

When spawning a sub-agent via Task tool, include:
```
Read C:\Projects\hattrick\CLAUDE.md before starting work.
Follow all coding standards in that file.
```

## Game-Specific Rules

- **Match engine results:** Test against Hattrick formulas with 1000+ sample matches for statistical accuracy
- **Skill progression:** Must follow age curves and training formulas from official manual + wiki
- **Season mechanics:** Exactly 16-week seasons (14 league + 1 qualifier + 1 break)
- **Player aging:** Age-based decay formula - test against expected skill loss curves
- **Economy:** Wages, budget tracking - test for balance and non-negative states
