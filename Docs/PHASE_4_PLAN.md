# Phase 4: Season & League System + AI Agents + Friendly Matches

**Goal:** Full 16-week season runs with 4 turns/week. AI agents manage 7 opponent teams. League table tracks. Promotion/relegation works. Friendlies fill gaps.

**Deliverable:** Complete full season week 1-16. League table accurate. AI teams make reasonable decisions. Game works without LLM (rules-based default).

---

## Models

- **Season:** CurrentWeek, CurrentTurn (1-4), LeagueTable, Schedules
- **TurnType enum:** PreMidweek, PostMidweek, PreWeekend, EndOfWeek

## Snapshot DTOs (Hattrick.Core/Models/Snapshots/)

- **TeamSnapshot** — team name, budget, fans, spirit, staff summary
- **PlayerSnapshot** — all player attributes for AI consumption
- **LeagueTableSnapshot** — standings, points, GD
- **MatchFixtureSnapshot** — upcoming match info
- **MatchResultSnapshot** — completed match summary

## Repositories (all in-memory, thread-safe with `Lock`)

- ISeasonRepository

## Services (all methods take `Guid teamId` as first parameter)

- `IGameStateQueryService` - read-only snapshots for AI agents (skeleton, grows each phase)
- `IGameStateService` extended with `HumanPlayerTeamId` and `CurrentTurn` properties

## Services

- `IScheduleService` - 14-week round-robin league, week 15 qualifier, week 16 break
- `IFriendlyService(teamId, ...)` - schedule midweek friendlies (training value, 50/50 gate split, 0.35 XP/min vs league 3.5)
- `ILeagueService` - track standings, tiebreakers
- `ISeasonAdvanceService` - **4-turn-per-week pipeline** (AdvanceTurn(), CurrentTurn 1-4)
- `ISeasonEndService` - promotion/relegation, prize money, new season prep
- `ISeasonInitService` - generate 7 AI teams for division

## AI Agent Services (Hattrick.Core/Ai/)

- `IAiManagerOrchestrator` - coordinates AI decisions per turn type for each AI team
  - `ProcessPreMatchDecisions(teamId)` — lineup, tactics, attitude
  - `ProcessManagementDecisions(teamId)` — react to injuries, transfers
  - `ProcessEndOfWeekDecisions(teamId)` — training, staff, youth
- `IRulesBasedAi` - deterministic fallback AI (default, always works)
- `ILlmClient` / `OllamaLlmClient` - calls local Ollama server (optional upgrade)
- `IToolDefinitionProvider` - maps service methods to LLM tool call schemas
- `IToolExecutor` - dispatches LLM tool calls to domain service methods
- `IAiSettings` - LLM endpoint, model, enabled flag (stored in game save)

## Key Mechanic: Friendly Matches

Teams eliminated from cup MUST arrange friendlies each midweek to maintain training minutes for all players.
