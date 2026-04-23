# Hattrick Clone - Project Status

**Date:** April 23, 2026
**Current Phase:** 2 (Team Creation & Lineup Management) — SPRINT 2 COMPLETE ✅
**Overall Progress:** 35% (Phase 2 Sprint 2 done)

## Project Stats

- **Solution:** Hattrick.slnx with 3 projects
- **Tests:** 940 passing (Phase 0-1 + Phase 2 Sprint 1-2)
- **Compilation:** OK
- **App Runs:** Yes (Players page shows generated team with 25 players)

## Phase 0 - COMPLETE ✅

### Infrastructure Foundation
All foundation infrastructure complete:
- ✅ Solution structure (3 projects: Hattrick, Hattrick.Core, Hattrick.Tests)
- ✅ MAUI Blazor shell with navigation
- ✅ DI container setup (all services registered)
- ✅ Infrastructure services:
  - IRandomProvider / RandomProvider
  - IDateTimeProvider / DateTimeProvider
  - ISaveGameService / SaveGameService (atomic writes)
  - ISaveSlotService / SaveSlotService (10 auto-save slots)
  - IGameStateService / GameStateService
- ✅ 54 tests passing (infrastructure)

## Phase 1 - SPRINT 1 COMPLETE ✅

### Enums & Value Types (All 4 Quartets Complete)

**Quartet 1: Core Skill Enums** ✅
- SkillType enum: 8 members (Keeper, Defending, Playmaking, Winger, Scoring, Passing, SetPieces, Stamina)
- SkillLevel enum: 20 levels (values 1-20)
- SkillLevelDisplayNames static class: full name mappings (non-existent to utopian)
- Tests: 27 passing

**Quartet 2: Position & Order Enums** ✅
- Position enum: 6 members (Keeper, CentralDefender, WingBack, InnerMidfielder, Winger, Forward)
- IndividualOrder enum: 5 members (Normal, Offensive, Defensive, TowardsMiddle, TowardsWing)
- Tests: 21 passing

**Quartet 3: Tactics & Formation Enums** ✅
- Formation enum: 10 members (Formation442-Formation253, values 0-9)
- Tactic enum: 7 members (Normal, Pressing, CounterAttack, AttackInMiddle, AttackOnWings, PlayCreatively, LongShots)
- TeamAttitude enum: 3 members (PlayItCool, Normal, MatchOfTheSeason)
- Tests: 42 passing

**Quartet 4: Specialty & Personality Enums** ✅
- Specialty enum: 8 members (None, Technical, Quick, Head, Powerful, Unpredictable, Resilient, Support)
- PlayerPersonality enum: 6 members (Nice, Nasty, Leader, Loner, Temperamental, Calm)
- CoachType enum: 3 members (Offensive, Defensive, Balanced)
- Tests: 53 passing (comprehensive: member existence, values, string conversion, error handling)

**Phase 1 Sprint 1 Summary:**
- 4 quartets completed
- 142 enum tests + 141 infrastructure tests = 283 total tests passing
- All enums in Hattrick.Core/Models folder
- All enums follow Hattrick game rules and terminology
- Complete TDD workflow: test-writer → test-verifier → coder → verifier → commit

## Phase 1 - Sprint 2 COMPLETE ✅

### Player Model & Repository (2 Quartets)

**Quartet 1: Player Model** ✅
- Player class with 24 properties (Id, TeamId, Name, Age, Skills dictionary, enums, etc.)
- Mutable class with auto-generated Id, Skills dictionary initialized
- Tests: 93 passing

**Quartet 2: IPlayerRepository + PlayerRepository** ✅
- IPlayerRepository interface with 5 methods: GetByTeamId, GetById, Add, Update, Remove
- Thread-safe with System.Threading.Lock, returns IReadOnlyList snapshots
- Registered as Singleton in DI
- Tests: 33 passing (including 4 concurrent thread-safety tests)

**Sprint 2 Summary:** 2 quartets, 126 new tests, 312 total passing

## Phase 1 - Sprint 3 COMPLETE ✅

### Team Model & Repository (2 Quartets)

**Quartet 1: Team Model** ✅
- Team class with 14 properties (Id, Name, IsHumanControlled, Budget, Fans, FanClubSize, TeamSpirit, Confidence, CoachType, CoachLevel, AssistantCoachLevel, DoctorLevel, SpokespersonLevel, FinancialDirectorLevel)
- Mutable POCO following Player.cs pattern, CoachLevel defaults to 1
- CoachTypeEnumTests.cs created for ordinal stability tests
- Tests: 73 passing

**Quartet 2: ITeamRepository + TeamRepository** ✅
- ITeamRepository interface with 4 methods: GetById, GetAll, Add, Update (no Remove — teams permanent)
- Thread-safe with System.Threading.Lock, returns IReadOnlyList snapshots
- Registered as Singleton in DI
- Tests: 30 passing (including 3 concurrent thread-safety tests + reflection test for no Remove)

**Sprint 3 Summary:** 2 quartets, 103 new tests, 415 total passing

## Phase 1 - Sprint 4 COMPLETE ✅

### Player List UI (2 Quartets)

**Quartet 1: PlayerDisplayService + Player Table Layout** ✅
- IPlayerDisplayService + PlayerDisplayService: FormatAge, GetSkillFloor, GetSkillBarPercent, GetSkillColorClass, GetPositionDisplay, GetSpecialtyDisplay
- Players.razor: skill table with all 7 skills (excl. Stamina), player name links, age/TSI/wage/specialty/form/best-position columns
- Registered as Singleton in DI
- Tests: 89 passing (54 service + 32 component + 3 DI)

**Quartet 2: PlayerStatsService + Sidebar Stats** ✅
- TeamTotals record: TotalTSI, TotalWage, TotalEstimatedValue (TSI×25), NationalityCount, InjuredCount, RedCardCount, YellowCardCount
- TeamAverages record: AvgTSI, AvgWage, AvgEstimatedValue, AvgAge, AvgForm, AvgStamina, AvgExperience
- IPlayerStatsService + PlayerStatsService: guards empty list (no divide-by-zero), EstimatedValueMultiplier=25 constant
- Players.razor sidebar with Team Total and Team Average sections
- Tests: 66 new (42 service + 24 component)

**Sprint 4 Summary:** 2 quartets, 155 new tests, 570 total passing

## Phase 2 - Sprint 1 COMPLETE ✅

### Team & Player Generation (4 Quartets)

**Quartet 1: PlayerGenerationService** ✅
- IPlayerGenerationService + PlayerGenerationService: Generate players with position-appropriate skills
- Age 17-32, Form 5-8, Stamina 5-8, Experience 1-5
- Position-specific skill distributions (Keeper high GK, Forward high Scoring, etc.)
- Uses IRandomProvider for all randomness
- Tests: 58 passing

**Quartet 2: TeamGenerationService** ✅
- ITeamGenerationService + TeamGenerationService: Generate team with 25 players
- Distribution: 3 GK, 6 DEF (4 CD + 2 WB), 8 MID (4 IM + 4 W), 8 FWD
- Returns (Team, IReadOnlyList<Player>) tuple
- Defaults: Budget 10M, TeamSpirit 5, Confidence 5, CoachLevel 5
- Tests: 46 passing

**Quartet 3: DevSeedService** ✅
- IDevSeedService + DevSeedService: Seeds development data on startup
- Creates human-controlled team "FC Development" with 25 players
- Idempotent (skips if HumanPlayerTeamId already set)
- Wired into MauiProgram.cs
- Tests: 32 passing

**Quartet 4: PlayersPageService + Players.razor Integration** ✅
- IPlayersPageService + PlayersPageService: Mediates between component and repositories
- Players.razor now uses service (not repository directly) per architecture rules
- Fixed Guid.Empty issue from Phase 1 code review
- Tests: 14 passing

**Phase 2 Sprint 1 Summary:** 4 quartets, 166 new tests, 736 total passing
**Gameplay:** App now displays real player data on Players page

## Phase 2 - Sprint 2 COMPLETE ✅

### Lineup Models & ILineupService (4 Quartets)

**Quartet 1: MatchLineupSlot Model** ✅
- Immutable record: PlayerId, Position, IndividualOrder (default Normal), IsStarter
- Supports `with` expression and value equality
- Tests: 35 passing

**Quartet 2: TeamLineup Model** ✅
- Mutable class: TeamId, Formation, Tactic, Attitude, Slots list, SetPiecesTakerId, CaptainId
- Constants: StarterCount=11, MaxSubstituteCount=3, MaxTotalSlots=14
- Tests: 64 passing

**Quartet 3: LineupService - Validation** ✅
- ILineupService + LineupService with ValidateLineup method
- 9 validation rules: 11 starters, 1 keeper, max 3 subs, no duplicates, player existence, captain/set pieces in lineup, no injured/red-carded
- LineupValidationResult record with IsValid and Errors
- Registered as Singleton in DI
- Tests: 55 passing

**Quartet 4: LineupService - AutoSuggest** ✅
- SuggestLineup method: picks best 11 for 4-4-2 by position/skill
- Excludes injured/red-carded, adds up to 3 subs, sets captain by Leadership
- Tests: 32 passing (87 total LineupService tests)

**Phase 2 Sprint 2 Summary:** 4 quartets, 204 new tests, 940 total passing

## Upcoming Work

### Phase 2 Sprint 3 (Next)
- Lineup Manager UI components
- FormationPitch, TabNavigation, PlayerAvatar components

### Phase 3 (Pending)
- Match Engine

### Phase 4 (Pending)
- Season & League + Friendlies

## Key Implementation Notes

- Tech Stack: C#, .NET 10, MAUI Blazor, xUnit, FluentAssertions v7, NSubstitute
- All services use constructor injection + interfaces
- No database: in-memory gameplay, JSON save files
- All gameplay code in services, zero logic in .razor components
- All names fictional (no real teams/players/cities)
- TDD discipline: tests written first, implementation follows, full verification

## Testing

Run tests with:
```bash
dotnet test Hattrick.Tests/Hattrick.Tests.csproj
```

## Architecture Rules Quick Ref

- Blazor components: ZERO business logic (call services)
- All services: have interfaces, registered in DI, injected via constructor
- No `new Service()` inside other classes
- Functions ≤ 30 lines, classes ≤ 300 lines
- All names fictional (no real teams/players/cities)
- Never `.Result` or `.Wait()` on async (always `await`)
- `IRandomProvider` for all randomness
- `IDateTimeProvider` for all time (no `DateTime.Now` in services)

## Contact & Help

See CLAUDE.md for all rules and coding standards.
