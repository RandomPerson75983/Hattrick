# Phase 10: Player Experience, Loyalty & Form

**Goal:** Experience, loyalty bonuses, form function correctly.

**Deliverable:** Long-serving players have measurable rating bonuses. Form affects performance.

---

## Services

- `IExperienceService` - XP gain per match (League 3.5, Cup 7.0, Friendly 0.35 per 90min)
- `ILoyaltyService` - loyalty bonus = min(daysAtClub / 252, 1.0) effective skill levels (up to +1); mother club = +0.5
- `IFormService` - current form modifies skill contributions ±10%
