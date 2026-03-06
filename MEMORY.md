# Hattrick Clone Project Memory

## Project Overview
- **Goal:** Build a single-player Hattrick clone based on the official game rules
- **Scope:** 4-6 week MVP (per feasibility assessment after reading official manual)
- **Status:** Project initialization phase

## Documentation
- **Official Manual:** `Introduction » Manual » Help » Hattrick.pdf` (59 pages)
  - Comprehensive game documentation
  - All official systems and rules
- **Community Wiki Archive:** `wiki_archive/` (48 pages from Nov 2022 Wayback Machine)
  - Contains reverse-engineered formulas not in official manual
  - Key pages: Training.html, Match.html, League.html, Youth.html, Achievements.html

## Game Systems Summary (from manual review)

### Core Game Mechanics
- **Match Engine:** Probability-based with 3 team ratings (Attack, Midfield, Defense)
- **Player Skills:** 8 basic skills (Stamina, Playmaking, Scoring, Winger, Goalkeeping, Passing, Defending, Set Pieces)
- **Player Progression:** Age-based skill decay, training system with 11 training types
- **League:** Pyramid-based, 16-week seasons (14 league + 1 qualifier + 1 break), promotion/relegation

### Key Systems
- **Training:** 11 training types, intensity levels (Light/Normal/Intense/Extreme), coach bonuses
- **Transfers:** Bidding system with agent fees, mother club (2%) and previous club (3%) commissions
- **Economy:** Wages, stadium maintenance, sponsorship income
- **Youth System:** Youth development and player progression
- **Teams:** Formation selection, tactics, individual player orders, team attitude, team spirit/confidence
- **Special Events:** Weather effects, injuries, player specialties, tiredness-based errors

### Simplifications (single-player removes)
- No multiplayer balance concerns
- No adaptive AI opponents needed
- No anti-cheat systems required
- No social features needed

## Project Structure (to be created)
```
/c/Projects/hattrick/
├── MEMORY.md (this file)
├── Introduction » Manual » Help » Hattrick.pdf
├── wiki_archive/ (community formulas)
├── docs/ (to create - architecture, formulas, design)
├── src/ (implementation)
├── tests/ (test suite)
└── [additional folders as project evolves]
```

## Phase 0: Foundation & Infrastructure — COMPLETE ✅

**Status:** All infrastructure in place. Tests: 54 passing. Solution builds without errors.

### Deliverables Completed
1. **Documentation:**
   - STATUS.md (project state tracking)
   - IMPLEMENTATION_PLAN.md (10-phase overview)
   - DEVELOPMENT_RULES.md (coding standards with examples)
   - FORMULAS.md (skeleton for game formulas, filled per phase)

2. **Solution Structure:**
   - Hattrick.slnx (modern solution format)
   - Hattrick/ (MAUI Blazor app, net10.0-* multi-platform targets)
   - Hattrick.Core/ (Class library, net10.0, all business logic)
   - Hattrick.Tests/ (xUnit test suite, net10.0)

3. **Infrastructure Services (all in Hattrick.Core/Services/):**
   - IRandomProvider + RandomProvider (wraps Random.Shared for testability)
   - IDateTimeProvider + DateTimeProvider (testable time access)
   - ISaveGameService + SaveGameService (atomic JSON serialization)
   - ISaveSlotService + SaveSlotService (slot 1-99 manual, 100-109 auto-save with round-robin)
   - IGameStateService + GameStateService (session state holder)

4. **DI Setup:**
   - MauiProgram.cs registers all services as Singletons
   - All services use constructor injection (no direct instantiation)

5. **UI Shell:**
   - MainLayout.razor with sidebar navigation
   - NavMenu.razor with links to 8 game sections
   - Home.razor (MainMenu page with New/Continue/Load/Settings/About buttons)
   - Placeholder pages: Season, Lineup, Match, Training, Finance, Transfers, Youth, Cup

6. **Tests (54 total, all passing):**
   - RandomProviderTests (6 tests)
   - DateTimeProviderTests (5 tests)
   - SaveGameServiceTests (8 tests - async I/O, round-trip, edge cases)
   - SaveSlotServiceTests (13 tests - slot management, rotation, boundaries)
   - GameStateServiceTests (8 tests - state management, reset)
   - DependencyInjectionTests (3 tests - DI container validation)

### Build Status
- Solution compiles: ✅ Success
- Test suite: ✅ 54/54 passing
- No compiler warnings
- No runtime errors

### Architecture Decisions Locked
- Hattrick.Core is pure business logic (.NET Standard compatible)
- MAUI app is platform-specific UI layer
- Tests reference only Core, not MAUI app (cleaner test isolation)
- All services are Singletons (session lifetime appropriate for single-player game)

### Next Phase (Phase 1: Core Data Models + Lineup Management)
Ready to implement:
- Player, Team, Season, MatchFixture, MatchResult models
- Repositories (in-memory with thread-safe locks)
- LineupService with validation
- UI components for lineup/formation/substitution management

## Known Conventions & Patterns (for future phases)

### Testing Pattern
```csharp
[Fact]
public void MethodName_Scenario_ExpectedResult() { }
```

### Service Pattern
- Always interface + implementation pair
- Constructor injection only
- All public async methods use CancellationToken parameter

### Save/Load Pattern
- Atomic write: temp file → rename (prevents corruption on crash)
- JSON serialization via System.Text.Json
- CamelCase property names, indented for readability

### Save Slot Convention
- Slots 1-99: Manual saves (user-created, never deleted automatically)
- Slots 100-109: Auto-save rotation (10 total, round-robin overwrite)
- File paths: `{appdata}/Hattrick/Saves/save_slot_NNN.json`

## Notes for Future Sessions
- This is a completely separate project from Baseball Manager
- The user has played Hattrick since 2004 and knows the game deeply
- Focus on accuracy to official rules and community-discovered formulas
- Feasibility assessment shows this is very doable in 4-6 weeks
