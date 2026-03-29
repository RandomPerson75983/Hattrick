# Phase 3: Match Engine

**Goal:** Full event-based 90-minute match simulation. Accurate to Hattrick formulas.

**Deliverable:** Two teams can play a full match. Events generated. Results match expected probability distributions (10K+ simulations).

---

## Models

- **MatchFixture/MatchResult:** Fixtures + results with full event logs

## Repositories (all in-memory, thread-safe with `Lock`)

- IMatchRepository

## Key Services

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

## Statistical Validation (1,000+ samples required)

- 10,000 matches between equal teams → 2.3-2.7 goals/match
- Home wins ~45-50%, away ~27-32%, draws ~20-25%
- Pressing tactic reduces goals vs Normal
- Specialty events fire at expected rates
- Pullback measurably reduces scorelines
