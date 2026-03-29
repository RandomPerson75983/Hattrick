# Phase 6: Training System

**Goal:** Weekly training updates player skills per Hattrick formulas. All match types counted (league, cup, friendly).

**Deliverable:** Players gain skills at correct rates. Age, coach, assistants affect speed.

---

## Formula

```
baseGain = (cappedMinutes / 90) × (intensity / 100) × trainingTypeFactor
applied = baseGain × ageMult × coachMult × assistantMult
```

## Key Rules

- Friendlies provide full training minutes (critical for cup-eliminated teams)
- Cap minutes at 90 for training purposes
- Age factor: 1 / (1 + 0.04 × max(0, age - 17))
- Training type eligibility matrix (GK, Wingers, IMs, etc.)
- Stamina allocation affects current form
- Skill sub-level tracking (7.73 = skill 7, 73% toward skill 8)
