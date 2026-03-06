# Hattrick Clone - Game Formulas

This document describes all implemented game formulas. Formulas are added as each phase is completed.

---

## Phase 0-1: Core Models (No Formulas Yet)

Foundation phase. Formulas begin in Phase 2.

---

## Phase 2: Match Engine Formulas

### Rating Calculation (IRatingService)

TBD - implemented in Phase 2

**Components:**
- Base skill contribution by position
- Central crowding penalty
- Individual order modifiers
- Coach type modifier
- Home advantage
- Formation experience effect
- Team spirit effect
- Confidence effect
- Loyalty bonus
- Mother club bonus
- Form modifier

### Match Event Generation

TBD - implemented in Phase 2

**Attack event distribution:**
- Possession % = HomeRating / (HomeRating + AwayRating)
- HomeAttacks = round(18 × possession%)
- AwayAttacks = round(18 × (1 - possession%))

**Goal probability:**
- `goalProb = attackRating / (attackRating + defenseRating) × conversionFactor`

**Tactical zone distribution:**
- Normal: 30% left, 40% center, 30% right
- AIM (All In Midfield): 15% left, 70% center, 15% right
- AOW (All Out Wings): 42.5% left, 15% center, 42.5% right
- Pressing: 30/40/30 but fewer total events
- Counter Attack: 30/40/30 with Passing×2 vs Defending
- Long Shots: 30/40/30 + extra low-prob attempts
- Play Creatively: 30/40/30 with Unpredictable bonuses

**Specialty Interactions:**
- Quick: +attack% when CA active; nullified if opponent has Quick
- Head: Corner specialist bonus
- Technical: +15% conversion on 1v1
- Powerful: +10% defending when man-marking
- Unpredictable: +8% to highest skill when focused on
- Resilient: 30% chance to shake off injury

**Pullback Mechanic** (when leading ≥ 2)
- Each goal of lead: attack ×0.91, defense ×1.075
- Cap at goal difference 8
- Exceptions: MOTS attitude, cup final, final league round

**Underestimation Mechanic**
TBD - team quality gap, confidence, attitude trigger
- If triggered: reduce favored team's midfield/attack for first half
- Half-time recovery: 100% if losing, 67% if drawing, 33% if 1-goal lead

**Stamina Degradation** (minutes 60-90)
- `factor = 1 - (1 - stamina/20) × 0.3 × ((minute - 60) / 30)`
- Pressing tactic: degradation starts at minute 45

### Injury & Cards

TBD - implemented in Phase 2

- **Injury rate:** 0.4/90 per minute base (modified by medic + assistants)
- **Yellow card rate:** ~0.03 per attack event
- **Red card:** direct red or accumulate 2 yellows → red + suspension

---

## Phase 4: Training System Formulas

TBD - implemented in Phase 4

### Weekly Training Gain

**Base formula:**
```
baseGain = (cappedMinutes / 90) × (intensity / 100) × trainingTypeFactor
applied = baseGain × ageMult × coachMult × assistantMult
```

**Age multiplier:**
```
ageMult = 1 / (1 + 0.04 × max(0, age - 17))
```

**Coach multiplier:**
| Level | Name | Multiplier |
|-------|------|-----------|
| 1 | Disastrous | 0.70 |
| 2 | Wretched | 0.78 |
| 3 | Poor | 0.84 |
| 4 | Weak | 0.90 |
| 5 | Inadequate | 0.94 |
| 6 | Passable | 0.97 |
| 7 | Solid | 1.00 |
| 8 | Excellent | 1.07 |

**Assistant coaches bonus (combined level 0-10):**
| Combined | Training Bonus |
|---------|---------------|
| 0 | +0% |
| 2 | +5% |
| 4 | +10% |
| 6 | +15% |
| 8 | +20% |
| 10 | +25% |

**Training type eligibility:**
| Type | Full trainees | Partial trainees | Notes |
|------|--------------|-----------------|-------|
| Goalkeeping | GK | – | GK-only |
| Crossing (Winger) | Winger | WB (½ rate) | |
| Scoring | Forward | – | |
| Playmaking | IM | Winger (½ rate) | |
| Wing attacks | Forward + Winger | – | |
| Defending | CD + WB | – | |
| Short passes | IM + Winger + Forward | – | |
| Through passes | CD + IM + Winger | – | |
| Defensive positions | GK + CD + IM + Winger | – | Very slow |
| Shooting | All outfield | – | Trains Scoring + SetPieces |
| Set pieces | All who played | – | SP taker + GK ×1.25 |

**Stamina training:**
```
staminaEffect = staminaPercent/100 × (intensity/100) × minutesFactor × ageMult
minutesFactor:
  - 90+ min: 1.0
  - < 90 min: 0.75 + (min/90 × 0.25)
  - 0 min: 0.5
  - injured: 0.0
```

---

## Phase 5: Economy Formulas

TBD - implemented in Phase 5

### Wage Calculation

**Player wages:**
```
baseWage = 250 USD/week per player
variable = TSI × 0.5  (approximate, to calibrate)
specialty = +10% if specialty
foreign = +20% if foreign player
totalWage = (baseWage + variable) × (1 + specialty%) × (1 + foreign%)
```

**Staff wages:**
| Position | Base Cost Range |
|----------|------------------|
| Coach | varies by level |
| Assistant Coach | varies by level |
| Medic | varies by level |
| Psychologist | varies by level |
| Financial Director | varies by level |

See official Hattrick manual for exact costs.

### Gate Receipts

**Attendance:**
```
attendance = fans × attendanceFactor
  - League match: 1.0
  - Cup final: 1.3
  - Friendly: 0.6
```

**Income:**
```
income = attendance × ticketRevenue (fixed rate per seat)
```

**Weather modifiers:**
- Sunny: +10%
- Overcast: ±0%
- Rain: -15%
- Snow: -25%

### League Prize Money

TBD - use divisional cup prize money levels from manual. Exact amounts tunable.

### Season-End Processing

- Membership fees: 30 USD × fans
- Promotion/Relegation fans adjustment: ±10%
- Debt limits:
  - Warn at -200K
  - Restrict spending at -200K
  - Bankruptcy at -500K

---

## Phase 6: Transfer Market Formulas

TBD - implemented in Phase 6

### Player Valuation

**Market value:**
```
baseValue = f(TSI, age)
ageFactor:
  - Peaks at age 24
  - Reduces ~5% per year above 28
  - < 20: potential premium
```

**Commissions (total 8% deducted):**
- Mother club commission: 2%
- Previous club commission: 3%
- Agent fee: 3%

---

## Phase 8: Player Experience, Loyalty & Form

TBD - implemented in Phase 8

### Experience Gain

**XP by match type (per 90 minutes, scale by actual minutes):**
| Type | XP/90min |
|------|---------|
| League | 3.5 |
| Cup | 7.0 |
| Qualifier | 7.0 |
| Friendly (domestic) | 0.35 |
| International Friendly | 0.7 |

- Level progression: ~100 XP per level (1-20)

### Loyalty Bonus

**Skill bonus:**
```
loyaltyBonus = min(daysAtClub / 252, 1.0)  → 0 to +1.0 skill levels on all skills
motherClubBonus = +0.5 skill levels (permanent, does not reset)
```

Applied in RatingService for match calculations.

### Form Modifier

**Current form modifies skill contributions:**
```
formMultiplier = 1.0 + (currentForm - 10) / 100  → ±10% to skills
```

---

## Phase 10: Polish Formulas

TBD - implemented in Phase 10

### Formation Experience

- Gain: +1 per match using formation
- Cap: Excellent (8) — no confusion above
- Loss: -1 per 4 weeks not using
- Confusion check: if experience < Excellent, chance of confusion event

### Skill Calculation (TSI)

TBD - Total Skill Index formula (exponential weighting per Hattrick community reverse-engineering)

---

## Reference Sources

- **Official Hattrick Manual:** Introduction » Manual » Help » Hattrick.pdf (in project root)
- **Community Wiki Archive:** wiki_archive/ (community reverse-engineered formulas)
- **Game Mechanics Wiki:** Hattrick.org forums and achievement system documentation

---

*This document is updated as each phase is implemented. Formulas are verified against statistical tests before approval.*
