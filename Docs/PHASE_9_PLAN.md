# Phase 9: Youth System (Old Squad)

**Goal:** Manager invests weekly. Pull one youth player per week. Promote to senior squad.

**Deliverable:** Youth generates players by investment level. Promotion works.

---

## Models

- **YouthSquad:** Investment level + youth players

## Repositories (all in-memory, thread-safe with `Lock`)

- IYouthSquadRepository

## Investment Mechanics

- Small (5K/wk): primary skill mean ~Weak (4-5)
- Medium (10K/wk): primary skill mean ~Inadequate-Passable (5-6)
- Large (20K/wk): primary skill mean ~Passable-Solid (6-7), peaks Excellent (8)+

## Services

- `IYouthService` - set investment, pull player (once/week), promote
