# Verifier Checklist (23 checks)

## Tests & Build
1. All tests pass
2. No regressions in related tests
3. Test naming follows `MethodName_Scenario_ExpectedResult`

## Assertion Quality (CRITICAL)
4. Assertions test the REQUIREMENT, not just "something exists"
5. Bounded values use `.BeInRange()`, not `.BeGreaterThan(0)`
6. Statistical tests use 1000+ samples with proper tolerance
7. Boundary values tested (min, max, off-by-one)
8. NO weak assertions: `.NotBeNull()` alone, `.BeOfType<T>()` without value checks

## Formula/Calculation
9. Tests use KNOWN VALUES with hand-calculated expected results
10. Each formula component is tested (X+Y+Z → verify Z affects output)
11. Test inputs use ALL formula components with non-zero values

## DI & Architecture
12. **DI MATCH**: Test ServiceCollection setup matches MauiProgram.cs (Singleton vs Scoped)
13. Services have interfaces, inject via constructor
14. Business logic in services, not Blazor components

## Safety
15. No magic numbers — use named constants
16. Null checks before `.Value` access
17. `ArgumentNullException.ThrowIfNull` on required parameters
18. No `DateTime.Now` — use injected dates or ICalendarService

## Thread Safety
19. Repositories use `Lock _lock` (not `object`)
20. Use `IRandomProvider`, never `Random.Shared` or `new Random()`

## Blazor
21. `CultureInfo.InvariantCulture` on all `.ToString("format")`
22. `async void` handlers wrapped in try-catch
23. `@key` directives on loops

## Logic Correctness (CRITICAL — compare code to handoff algorithm)
24. Implementation matches handoff algorithm/spec step-by-step — no skipped steps, no extras
25. Collection filters bounded correctly (`.Take(count - threshold)` not `.Where()` returning all)
26. Loop termination prevents over-iteration (can't release/remove more than intended)
27. Zero-value iterations handled (e.g., releasing player with no contract frees $0 — does the loop make progress?)
28. Removed/released entities in valid final state (in a repo, not limbo — no orphaned players)
29. Running totals not double-counted across re-fetches from in-memory repos
30. Guard conditions sufficient at BOTH block level AND item level
31. Lineup/rotation cleared before removing players from team

## Data
32. DTOs map all new fields in SaveGameMapper
33. Player uses `PlayerId` not `Id`
34. Roster changes sync both `Team.Players` AND repository
