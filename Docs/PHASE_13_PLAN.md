# Phase 13: Multi-Division League (Divisions 1-3)

**Goal:** Expand from single division to a 3-tier league pyramid. Promotion and relegation between divisions.

**Deliverable:** League structure with Division 1, 2, and 3. Teams promote/relegate at season end. AI teams generated for all divisions with skill levels appropriate to division tier.

---

## Structure

- Division 1: 8 teams (strongest)
- Division 2: 8 teams (mid-tier)
- Division 3: 8 teams (weakest)
- Total: 24 teams per league

## Promotion/Relegation

- Division 1: bottom 2 relegate to Division 2
- Division 2: top 2 promote to Division 1, bottom 2 relegate to Division 3
- Division 3: top 2 promote to Division 2

## Services

- `ILeagueStructureService` - manage multi-division hierarchy
- Extend `ISeasonInitService` - generate teams across all 3 divisions with division-appropriate skill levels
- Extend `ISeasonEndService` - process promotion/relegation across divisions

## League Cups

TBD — league cup competitions across divisions. Details to be determined when we reach this phase.

## Skill Scaling by Division

- Division 1: average skill ~solid-excellent (8-9)
- Division 2: average skill ~passable-solid (7-8)
- Division 3: average skill ~inadequate-passable (6-7)
