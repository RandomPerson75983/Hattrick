# Hattrick Clone - Implementation Plan

## Overall Build Order & Dependency Map

```
Phase 0: Foundation & Infrastructure
    └── Phase 1: Players & Teams
            └── Phase 2: Team Creation & Lineup Management
                    └── Phase 3: Match Engine (most complex)
                            └── Phase 4: Season & League + AI Agents + Friendlies
                                    └── Phase 5: Single Division Creation
                                            ├── Phase 7: Economy
                                            │       └── Phase 8: Transfer Market (Simplified)
                                            └── Phase 11: Cup Competition
                            ├── Phase 6: Training System (needs match minutes data)
                            └── Phase 9: Youth System (needs player model + economy)
Phase 10: Player Experience, Loyalty & Form (layer on top of match + training)
Phase 12: Achievements & Polish
    └── Phase 13: Multi-Division League (Div 1-3)
            └── Phase 14: Country Creation
                    └── Phase 15: Expanded Transfer Market
```

**Critical insight:** Match engine is the center of everything. Phases 0-5 must be solid before anything else.

## Core Architecture: API-First Design for AI Agents

The 7 non-player teams in each league are managed by AI agents (local LLM via Ollama, with rules-based fallback). Every manager action is exposed through team-agnostic service methods that both the Blazor UI and AI agents call.

### Key Principles
- **All service methods take `Guid teamId`** as first parameter — works for any team
- **Domain services ARE the API** — no separate API layer, no monolithic facade
- **Rules-based AI is the default** — game works without any LLM server
- **LLM (Ollama) is optional upgrade** — enabled in settings for smarter AI opponents
- **4 turns per week** — lets AI react to injuries/cards between matches

### Game Loop: 4 Turns Per Week
```
TURN 1: Pre-Midweek Match
  ├── Human/AI: Set lineup and tactics
  ├── Human/AI: Transfer activity (if window open)
  └── Simulate midweek match (cup/friendly)

TURN 2: Post-Midweek / Management
  ├── Human/AI: React to injuries, adjust lineup, browse transfers
  └── No match simulated

TURN 3: Pre-Weekend Match
  ├── Human/AI: Set lineup and tactics for league match
  ├── Human/AI: Transfer activity (if window open)
  └── Simulate weekend match (league)

TURN 4: End of Week
  ├── Human/AI: Training, staff, youth, economy decisions
  ├── Apply training gains
  ├── Process economy (wages, income, maintenance)
  ├── Update form, injuries heal, suspensions clear
  └── Auto-save
```

### AI Integration Architecture (Hattrick.Core/Ai/)
- `IAiManagerOrchestrator` — coordinates AI decisions per turn type
- `IRulesBasedAi` — deterministic fallback (default, no LLM needed)
- `ILlmClient` / `OllamaLlmClient` — calls Ollama via OpenAI-compatible API
- `IToolDefinitionProvider` — maps service methods to LLM tool schemas
- `IToolExecutor` — dispatches LLM tool calls to domain services
- `IAiSettings` — LLM endpoint, model name, enabled flag

### Game State Query Layer
- `IGameStateQueryService` — read-only snapshots for AI consumption
- Snapshot DTOs in `Hattrick.Core/Models/Snapshots/` — plain records, JSON-serializable

---

## Phase Plans

| Phase | Focus | Plan |
|-------|-------|------|
| 0 | Foundation & Infrastructure | [PHASE_0_PLAN.md](PHASE_0_PLAN.md) ✅ |
| 1 | Players & Teams | [PHASE_1_PLAN.md](PHASE_1_PLAN.md) |
| 2 | Team Creation & Lineup Management | [PHASE_2_PLAN.md](PHASE_2_PLAN.md) |
| 3 | Match Engine | [PHASE_3_PLAN.md](PHASE_3_PLAN.md) |
| 4 | Season & League + AI Agents + Friendlies | [PHASE_4_PLAN.md](PHASE_4_PLAN.md) |
| 5 | Single Division Creation | [PHASE_5_PLAN.md](PHASE_5_PLAN.md) |
| 6 | Training System | [PHASE_6_PLAN.md](PHASE_6_PLAN.md) |
| 7 | Economy | [PHASE_7_PLAN.md](PHASE_7_PLAN.md) |
| 8 | Transfer Market (Simplified) | [PHASE_8_PLAN.md](PHASE_8_PLAN.md) |
| 9 | Youth System | [PHASE_9_PLAN.md](PHASE_9_PLAN.md) |
| 10 | Experience, Loyalty & Form | [PHASE_10_PLAN.md](PHASE_10_PLAN.md) |
| 11 | Cup Competition | [PHASE_11_PLAN.md](PHASE_11_PLAN.md) |
| 12 | Achievements & Polish | [PHASE_12_PLAN.md](PHASE_12_PLAN.md) |
| 13 | Multi-Division League (Div 1-3) | [PHASE_13_PLAN.md](PHASE_13_PLAN.md) |
| 14 | Country Creation | [PHASE_14_PLAN.md](PHASE_14_PLAN.md) |
| 15 | Expanded Transfer Market | [PHASE_15_PLAN.md](PHASE_15_PLAN.md) |

---

## Testing Strategy

- **TDD:** RED → GREEN → REFACTOR for every feature
- **Framework:** xUnit + FluentAssertions v7 + NSubstitute
- **Statistical tests:** 1,000+ samples minimum; if flaky, increase samples not assertions
- **Test naming:** `MethodName_Scenario_ExpectedResult()`
- **All tests must pass** before reporting phase complete

---

## Code Quality Rules (see CLAUDE.md for full details)

1. No magic numbers (every literal → named constant)
2. CultureInfo.InvariantCulture on all `.ToString("format")`
3. Null-check before `.Value`
4. `async void` handlers wrapped in try-catch
5. IRandomProvider for all randomness
6. IDateTimeProvider for all time (no DateTime.Now)
7. IReadOnlyList returns from repositories
8. Thread-safe locks on all repositories
9. Constructor injection only
10. Functions ≤ 30 lines, classes ≤ 300 lines

---

## Git Commit Format

```
feat: Short description    (new feature)
fix: Short description     (bug fix)
test: Short description    (adding tests)
refactor: Short description (no behavior change)
```

---

## Timeline Estimate
```
Week 1:   Phase 0 + Phase 1 + Phase 2
Week 2-3: Phase 3 (budget extra time)
Week 3:   Phase 4 + Phase 5
Week 4:   Phase 6 + Phase 7
Week 4-5: Phase 8 + Phase 9
Week 5-6: Phase 10 + Phase 11
Week 6:   Phase 12
Week 7:   Phase 13 + Phase 14 + Phase 15
```

---

*See individual phase plans in Docs/PHASE_0_PLAN.md through Docs/PHASE_15_PLAN.md for detailed specifications.*
