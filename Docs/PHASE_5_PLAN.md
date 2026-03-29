# Phase 5: Single Division Creation

**Goal:** Generate a complete 8-team division. Human team + 7 AI opponents with realistic squads. New Game flow functional.

**Deliverable:** Player can start a new game, name their team, and be placed into a division with 7 AI teams. All teams have full 25-player squads with appropriate skill distributions.

---

## Services

- `ITeamGenerationService(teamId, ...)` - generate a squad of ~25 players with realistic skill, age, and position distribution
- `IPlayerGenerationService` - create individual players with appropriate names, ages, skills, specialties
- `INameGenerationService` - fictional team and player name generation
- `IDivisionSetupService` - create a single division of 8 teams, assign to league

## New Game Flow

1. Player enters team name
2. System generates human team with starting squad
3. System generates 7 AI opponent teams with squads
4. Division created with all 8 teams
5. Season schedule generated (uses `IScheduleService` from Phase 4)
6. Game begins at Week 1, Turn 1

## Squad Generation Rules

- ~25 players per team
- Position distribution: 3 keepers, 5-6 defenders, 5-6 midfielders, 3-4 wingers, 3-4 forwards
- Age distribution: mix of 17-35, weighted toward 20-28 peak years
- Skills appropriate to division level
- Each player gets a specialty (or none) based on probability distribution
- Balanced overall team strength across the division (±10% variance)
