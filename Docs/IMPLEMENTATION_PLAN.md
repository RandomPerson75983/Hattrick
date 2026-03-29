# Hattrick Clone - Implementation Plan

## Overall Build Order & Dependency Map

```
Phase 0: Foundation & Infrastructure
    └── Phase 1: Core Data Models + Lineup Management
            ├── Phase 2: Match Engine (most complex)
            │       └── Phase 3: Season & League + AI Agents + Friendlies
            │               └── Phase 5: Economy
            │                       └── Phase 6: Transfer Market
            ├── Phase 4: Training System (needs match minutes data)
            └── Phase 7: Youth System (needs player model + economy)
Phase 8: Player Experience, Loyalty & Form (layer on top of match + training)
Phase 9: Cup Competition
Phase 10: Achievements & Polish
```

**Critical insight:** Match engine is the center of everything. Phases 0-2 must be solid before anything else.

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

## Phase 0: Foundation & Infrastructure

**Goal:** Runnable MAUI Blazor app with navigation, DI, save/load skeleton, and test project.

**Deliverable:** App launches, navigates between blank pages. JSON save/load works. All tests green.

### Infrastructure Services
- `IRandomProvider` / `RandomProvider` - wraps all randomness
- `ISaveGameService` / `SaveGameService` - atomic JSON write (temp → rename)
- `ISaveSlotService` / `SaveSlotService` - slots 1-99 manual, 100+ auto
- `IGameStateService` / `GameStateService` - in-memory holder
- `IDateTimeProvider` / `DateTimeProvider` - no DateTime.Now in services

### UI Shell
- MainLayout with nav menu
- MainMenuPage (New Game / Continue / Settings)
- Placeholder pages for each major section

---

## Phase 1: Core Data Models + Lineup Management

**Goal:** All domain models defined. Repositories operational. Lineup management fully functional. API-first: all services take `teamId`.

**Deliverable:** All models create/read/update correctly. Player can set starting XI, positions, orders, substitution plan. Services work for any team (human or AI).

### Key Models
- **Player:** Id, TeamId, Name, Age, Skills (1-20 scale), Specialty, Form, TSI, Injury status
- **MatchLineupSlot:** Position, IndividualOrder, IsStarter, Substitution plan
- **TeamLineup:** Formation, Tactic, Attitude, 11 starters + 3 subs
- **Team:** Id, Name, IsHumanControlled, Budget, Fans, TeamSpirit, DefaultLineup, Staff
- **Season:** CurrentWeek, CurrentTurn (1-4), LeagueTable, Schedules
- **MatchFixture/MatchResult:** Fixtures + results with full event logs
- **YouthSquad:** Investment level + youth players
- **TurnType enum:** PreMidweek, PostMidweek, PreWeekend, EndOfWeek

### Snapshot DTOs (Hattrick.Core/Models/Snapshots/)
- **TeamSnapshot** — team name, budget, fans, spirit, staff summary
- **PlayerSnapshot** — all player attributes for AI consumption
- **LeagueTableSnapshot** — standings, points, GD
- **MatchFixtureSnapshot** — upcoming match info
- **MatchResultSnapshot** — completed match summary

### Repositories (all in-memory, thread-safe with `Lock`)
- IPlayerRepository
- ITeamRepository
- ISeasonRepository
- IMatchRepository
- IYouthSquadRepository

### Services (all methods take `Guid teamId` as first parameter)
- `ILineupService` - lineup validation, auto-suggest, substitution plan, man marking
- `IGameStateQueryService` - read-only snapshots for AI agents (skeleton, grows each phase)

### API-First Requirements
- Every service method signature: `ReturnType MethodName(Guid teamId, ...params)`
- `IGameStateService` extended with `HumanPlayerTeamId` and `CurrentTurn` properties

---

## Phase 2: Match Engine

**Goal:** Full event-based 90-minute match simulation. Accurate to Hattrick formulas.

**Deliverable:** Two teams can play a full match. Events generated. Results match expected probability distributions (10K+ simulations).

### Key Services
- `IRatingService` - calculates team ratings from lineup + modifiers (home advantage, coach, loyalty, form, team spirit)
- `IMatchSimulatorService` - runs 90-minute event-based simulation
  - Attack event generation (distribution by possession %)
  - Goal probability = attackRating / (attackRating + defenseRating)
  - Specialty interactions (Quick, Technical, Head, Powerful, Unpredictable, Resilient)
  - Tactical zone distribution (AIM, AOW, etc.)
  - Stamina degradation (minutes 60-90)
  - Pullback mechanic (when leading ≥ 2)
  - Underestimation mechanic (quality gap risk)
  - Substitution processing
- `IMatchEventDescriptionService` - human-readable event text

### Statistical Validation (1,000+ samples required)
- 10,000 matches between equal teams → 2.3-2.7 goals/match
- Home wins ~45-50%, away ~27-32%, draws ~20-25%
- Pressing tactic reduces goals vs Normal
- Specialty events fire at expected rates
- Pullback measurably reduces scorelines

---

## Phase 3: Season & League System + AI Agents + Friendly Matches

**Goal:** Full 16-week season runs with 4 turns/week. AI agents manage 7 opponent teams. League table tracks. Promotion/relegation works. Friendlies fill gaps.

**Deliverable:** Complete full season week 1-16. League table accurate. AI teams make reasonable decisions. Game works without LLM (rules-based default).

### Services
- `IScheduleService` - 14-week round-robin league, week 15 qualifier, week 16 break
- `IFriendlyService(teamId, ...)` - schedule midweek friendlies (training value, 50/50 gate split, 0.35 XP/min vs league 3.5)
- `ILeagueService` - track standings, tiebreakers
- `ISeasonAdvanceService` - **4-turn-per-week pipeline** (AdvanceTurn(), CurrentTurn 1-4)
- `ISeasonEndService` - promotion/relegation, prize money, new season prep
- `ISeasonInitService` - generate 7 AI teams for division

### AI Agent Services (Hattrick.Core/Ai/)
- `IAiManagerOrchestrator` - coordinates AI decisions per turn type for each AI team
  - `ProcessPreMatchDecisions(teamId)` — lineup, tactics, attitude
  - `ProcessManagementDecisions(teamId)` — react to injuries, transfers
  - `ProcessEndOfWeekDecisions(teamId)` — training, staff, youth
- `IRulesBasedAi` - deterministic fallback AI (default, always works)
- `ILlmClient` / `OllamaLlmClient` - calls local Ollama server (optional upgrade)
- `IToolDefinitionProvider` - maps service methods to LLM tool call schemas
- `IToolExecutor` - dispatches LLM tool calls to domain service methods
- `IAiSettings` - LLM endpoint, model, enabled flag (stored in game save)

### Key Mechanic: Friendly Matches
Teams eliminated from cup MUST arrange friendlies each midweek to maintain training minutes for all players.

---

## Phase 4: Training System

**Goal:** Weekly training updates player skills per Hattrick formulas. All match types counted (league, cup, friendly).

**Deliverable:** Players gain skills at correct rates. Age, coach, assistants affect speed.

### Formula
```
baseGain = (cappedMinutes / 90) × (intensity / 100) × trainingTypeFactor
applied = baseGain × ageMult × coachMult × assistantMult
```

### Key Rules
- Friendlies provide full training minutes (critical for cup-eliminated teams)
- Cap minutes at 90 for training purposes
- Age factor: 1 / (1 + 0.04 × max(0, age - 17))
- Training type eligibility matrix (GK, Wingers, IMs, etc.)
- Stamina allocation affects current form
- Skill sub-level tracking (7.73 = skill 7, 73% toward skill 8)

---

## Phase 5: Economy

**Goal:** Weekly cash flow accurate. Season-end prize money distributed. Debt/bankruptcy works.

**Deliverable:** Budget changes correctly every week. Financial reports accurate.

### Weekly Pipeline
1. Deduct player wages (base + TSI-based variable)
2. Deduct staff wages
3. If home match: add gate receipts
4. Add sponsorship installment
5. Check debt limits (warn -200K, restrict -200K, bankrupt -500K)

### Season-End
- Membership fees: 30 USD × fans
- League prize money by final position
- Cup prize money (Phase 9)
- Adjust fans ±10% (promotion/relegation)

### Services
- `ITSICalculationService` - Total Skill Index (exponential skill weighting)
- `IWageCalculationService` - player/staff wage calculation
- `IEconomyService` - weekly cash flow, season-end processing
- `IFinanceReportService` - financial reporting UI support

---

## Phase 6: Transfer Market (Simplified)

**Goal:** Buy from pool at fixed price. Sell at fixed price. No AI bidding.

**Deliverable:** Transfer market works. Player valuation sensible. AI manages squads.

### Transfer Windows
- Summer: week 15-16 + weeks 1-2 of new season
- Winter: week 8 (1-week window)

### Services (all methods take `Guid teamId`)
- `IPlayerValuationService` - market value = f(TSI, age); deduct 8% commissions
- `ITransferMarketService(teamId, ...)` - pool generation, buy/sell
- AI transfer logic handled by `IRulesBasedAi` + `IAiManagerOrchestrator` (Phase 3)
  - Adds transfer tool definitions to `ToolDefinitionProvider`
  - Adds transfer methods to `IRulesBasedAi`: buy weakness, sell old players

---

## Phase 7: Youth System (Old Squad)

**Goal:** Manager invests weekly. Pull one youth player per week. Promote to senior squad.

**Deliverable:** Youth generates players by investment level. Promotion works.

### Investment Mechanics
- Small (5K/wk): primary skill mean ~Weak (4-5)
- Medium (10K/wk): primary skill mean ~Inadequate-Passable (5-6)
- Large (20K/wk): primary skill mean ~Passable-Solid (6-7), peaks Excellent (8)+

### Services
- `IYouthService` - set investment, pull player (once/week), promote

---

## Phase 8: Player Experience, Loyalty & Form

**Goal:** Experience, loyalty bonuses, form function correctly.

**Deliverable:** Long-serving players have measurable rating bonuses. Form affects performance.

### Services
- `IExperienceService` - XP gain per match (League 3.5, Cup 7.0, Friendly 0.35 per 90min)
- `ILoyaltyService` - loyalty bonus = min(daysAtClub / 252, 1.0) effective skill levels (up to +1); mother club = +0.5
- `IFormService` - current form modifies skill contributions ±10%

---

## Phase 9: Cup Competition

**Goal:** 3-round knockout. Prize money distributed. Midweek slots filled.

**Deliverable:** Teams play cup. Bracket tracks. Prize money paid.

### Structure
- 8 teams → QF (week 3) → SF (week 7) → Final (week 12)
- Eliminated teams: subsequent midweek slots become friendly slots
- Cup XP: ×2 vs league

### Services
- `ICupService` - bracket generation, cup round processing, prize distribution

---

## Phase 10: Achievements & Polish

**Goal:** Achievement system. Formation experience. Weather. Man marking.

**Deliverable:** All major achievements trackable. Formation confusion works. Weather affects gates.

### Services
- `IAchievementService` - track 44 official Hattrick achievements
- `IFormationExperienceService` - gain +1/match, cap at Excellent (8), reduce -1 per 4 weeks unused
- `IWeatherService` - 4 weather types affect attendance (Sunny +10%, Rain -15%, Snow -25%)

### Polish
- Man marking: assign player to mark opponent (powerful defender: +10%, technical target: -8% to highest skill)

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
Week 1:   Phase 0 + Phase 1
Week 2-3: Phase 2 (budget extra time)
Week 3:   Phase 3
Week 4:   Phase 4 + Phase 5
Week 4-5: Phase 6 + Phase 7
Week 5-6: Phase 8
Week 6:   Phase 9 + Phase 10
```

---

*See individual phase plans in Docs/PHASE_0_PLAN.md through Docs/PHASE_10_PLAN.md for detailed specifications.*
