# Phase 12: Achievements & Polish

**Goal:** Achievement system. Formation experience. Weather. Man marking.

**Deliverable:** All major achievements trackable. Formation confusion works. Weather affects gates.

---

## Services

- `IAchievementService` - track 44 official Hattrick achievements
- `IFormationExperienceService` - gain +1/match, cap at Excellent (8), reduce -1 per 4 weeks unused
- `IWeatherService` - 4 weather types affect attendance (Sunny +10%, Rain -15%, Snow -25%)

## Polish

- Man marking: assign player to mark opponent (powerful defender: +10%, technical target: -8% to highest skill)
