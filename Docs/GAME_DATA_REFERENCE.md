# Hattrick Game Data Reference
# Captured from live hattrick.org — March 29, 2026
# Purpose: Reference for implementing game systems
# Source: Live game UI screenshots + manual

---

## Player Skills (7 skills)

1. Keeper
2. Defending
3. Playmaking
4. Winger
5. Passing
6. Scoring
7. Set Pieces

### Skill Level Denominations (Appendix 2)
| Level | Name |
|-------|------|
| 0 | non-existent |
| 1 | disastrous |
| 2 | wretched |
| 3 | poor |
| 4 | weak |
| 5 | inadequate |
| 6 | passable |
| 7 | solid |
| 8 | excellent |
| 9 | formidable |
| 10 | outstanding |
| 11 | brilliant |
| 12 | magnificent |
| 13 | world class |
| 14 | supernatural |
| 15 | titanic |
| 16 | extra-terrestrial |
| 17 | mythical |
| 18 | magical |
| 19 | utopian |
| 20 | divine |

### Sub-levels (shown in match ratings)
Each level has sub-levels displayed as text: "very low", "low", "high", "very high"
Example: "passable - very high" = 6.75, "solid - low" = 7.25

### Player Attributes
- **Age**: X years and Y days, Next Birthday shown
- **TSI**: Total Skill Index (numeric, e.g., 2330)
- **Wage**: Weekly in US$ (e.g., 1,188 US$/week)
- **Speciality**: Quick, Technical, Head, Powerful, Unpredictable, Resilient, Support (or none)
- **Form**: 1-8 scale with text label (disastrous to divine)
- **Stamina**: 1-9 scale with text label
- **Experience**: text + numeric (e.g., weak (4))
- **Leadership**: text + numeric (e.g., weak (4))
- **Loyalty**: text + numeric (e.g., brilliant (11))
- **Personality**: "[adjective] guy who is [trait] and [trait]" (e.g., "sympathetic guy who is calm and infamous")

### Player Page Features
- Owner + join date
- Last rating + date + position played
- Recommended Positions widget
- Player Stats: Career goals, Season Cup Goals, Career assists, Assists for the team
- Player functions: Captain, Set piece taker icons
- Tabs: Matches (history with position + star rating), Transfers, Skills

---

## Match Engine / Ratings

### Match Rating Zones (7 sectors)
- **Midfield** (1 rating)
- **Right Defence** / **Central Defence** / **Left Defence** (3 ratings)
- **Right Attack** / **Central Attack** / **Left Attack** (3 ratings)

### Additional Match Ratings
- **Indirect Set Pieces**: Defence + Attack ratings

### Match Plan
- **Team Attitude**: Normal, PIC (Play it cool), MOTS (Match of the season)
- **Tactic**: Normal, Pressing, Counter-Attack, Attack in the Middle, Attack on Wings, Play Creatively, Long Shots
- **Tactic Skill**: text + numeric (depends on tactic chosen)
- **Style of Play**: Neutral (slider from defensive to offensive)

### Match Info
- Possession: First Half %, Second Half %
- Chance Distribution: total chances per team, broken down by Right/Central/Left/Other/Special Events
- Attendance by seat type with % fill (Terraces, Basic Seats, Seats Under Roof, VIP Seats)
- Match Takings in US$
- Highlights: goals, cards, injuries with minute

### Match Events
- Goals (with scorer name and minute)
- Yellow cards
- Red cards
- Injuries (with severity)
- Missed chances
- Weather effects
- Tired players
- Added time
- Best player per team

### Average Ratings (shown post-match)
- Total Player Experience
- Average Midfield
- Average Defence
- Average Attack
- Total Average

---

## Staff System

### Staff Types & Max Count
| Staff Type | Max | Effects |
|---|---|---|
| Coach | 1 | Training Speed, Leadership (affects team spirit) |
| Assistant Coach | 2 | Training Speed, Injury Risk reduction, Form |
| Medic | 1 | Recovery Speed, Injury Risk reduction |
| Sports Psychologist | 1 | Team Spirit, Confidence |
| Form Coach | 1 | Form bonus |
| Financial Director | 1 | Max Funds (working capital), Return/week from reserves |
| Tactical Assistant | 1 | Extra subs/orders, Style of play flexibility |

### Coach Details
- Has preferred style of play: attack, defence, or neutral
- Has Leadership skill (affects team spirit)
- Has Skill Level (affects training speed)
- Wage varies by level

### Staff Skill Levels (0-5) — Costs at 16-week contract
| Level | Cost/Week |
|-------|-----------|
| 0 | 0 US$ |
| 1 | 1,020 US$ |
| 2 | 2,040 US$ |
| 3 | 4,080 US$ |
| 4 | 8,160 US$ |
| 5 | 16,320 US$ |

Note: Shorter contracts cost MORE per week. Cost doubles each level.

### Sports Psychologist Effects by Level
| Level | Team Spirit | Confidence |
|-------|-------------|------------|
| 0 | No bonus | No bonus |
| 1 | +0.1 | +0.24 |
| 2 | +0.2 | +0.48 |
| 3 | +0.3 | +0.70 |
| 4 | +0.4 | +0.92 |
| 5 | +0.5 | +1.12 |

### Form Coach Effects by Level
| Level | Form |
|-------|------|
| 0 | No bonus |
| 1 | +0.2 |
| 2 | +0.4 |
| 3 | +0.6 |
| 4 | +0.8 |
| 5 | +1.0 |

### Financial Director Effects by Level
| Level | Max Funds | Return/week |
|-------|-----------|-------------|
| 0 | 15,000,000 US$ | 100,000 US$ |
| 1 | 17,000,000 US$ | 200,000 US$ |
| 2 | 19,000,000 US$ | 400,000 US$ |
| 3 | 21,000,000 US$ | 600,000 US$ |
| 4 | 23,000,000 US$ | 800,000 US$ |
| 5 | 25,000,000 US$ | 1,000,000 US$ |

### Tactical Assistant Effects by Level
| Level | Extra subs/orders | Style of play flexibility |
|-------|-------------------|--------------------------|
| 0 | No bonus | No bonus |
| 1 | +1 | +20pp |
| 2 | +2 | +40pp |
| 3 | +3 | +60pp |
| 4 | +4 | +80pp |
| 5 | +5 | +100pp |

### Contract Lengths
- 1 to 16 weeks (one full season)
- Longer contract = lower weekly cost
- Severance fee to break early = 2x the savings vs shorter contract
- Cannot break in first or final week of contract

---

## Training System

### Training Types (11 total)
| # | Type | Positions Trained |
|---|------|-------------------|
| 1 | Keeper | Goalkeeper |
| 2 | Defending | Defenders |
| 3 | Playmaking | Midfielders |
| 4 | Winger | Wingers |
| 5 | Passing | Midfielders + Wingers |
| 6 | Scoring | Forwards |
| 7 | Set pieces | All who play |
| 8 | Defending (extended) | Keepers, Defenders + All Midfielders |
| 9 | Winger (extended) | Wingers + Attackers |
| 10 | Passing (extended) | Defenders + All Midfielders |
| 11 | Scoring and Set Pieces | Combo |

### Training Settings
- **Skill/Stamina split**: Slider (e.g., 90% skill / 10% stamina)
- **Training Intensity**: 0-100% slider
- Division average shown as dotted line

### Training Efficiency Factors
- Trainer Skill (coach level, e.g., 4)
- Assistant Coach levels (combined, e.g., 10)
- Total efficiency displayed as % (e.g., 92%)

### Training Preview
- Shows each player with their match positions (friendly + competitive)
- Training bar shows who receives training based on position played
- Training updates happen weekly (Thursday 20:30 shown)

---

## Stadium / Arena

### Seat Types
| Type | Build Cost | Revenue/seat | Maintenance/seat/week |
|------|-----------|-------------|----------------------|
| Terraces | 45 US$ | 7 US$ | 0.5 US$ |
| Basic Seating | 75 US$ | 10 US$ | 0.7 US$ |
| Seats Under Roof | 90 US$ | 19 US$ | 1.0 US$ |
| Seats in VIP Boxes | 300 US$ | 35 US$ | 2.5 US$ |

### Building
- Initialisation Cost: 10,000 US$ (per building project)
- Demolish cost: 6 US$ per seat
- **Build time: 7 days** (1 week)
- Can add or remove seats by type
- Percentage shown for each seat type

### Youth Stadium
- Separate from main stadium
- Total Capacity: 600 (in example)

---

## Finances

### Weekly Revenue Categories
- Match Takings (from home matches)
- Sponsors (weekly installment)
- Player Sales
- Commission (from transfers)
- Other

### Weekly Expense Categories
- Wages (player salaries)
- Stadium maintenance
- Stadium building (if construction in progress)
- Staff (coach + all staff)
- Youth expenses (facilities + scouts)
- New Signings (includes first week's wages)
- Other

### Financial Structure
- **Cash available**: Working capital (capped by Financial Director level)
- **Funds in Board reserves**: Excess capital held by board
- **Total capital in Club**: Cash + Reserves
- **Cash moved to reserves**: Amount board took this week
- **Projection to move to reserves**: Expected next transfer

### Sponsors
- Standard Sponsorship: Fixed amount per season
- Season Incentive: Bonus based on performance
- Paid out in equal weekly installments

### Reports Available
- Weekly Finances
- Seasonal Finances
- Youth Finances
- Transfer Activity

---

## Transfer Market

### Search Filters
- Age range (years + days, min/max)
- 4 skill filter slots (skill type + min level + max level)
- Speciality filter (icon-based selection)
- Max Bid
- Country of birth
- Continent
- TSI range
- Salary range
- Estimated Value range
- Max level for non-specified skills
- Sort by: Nearest to deadline first / Newest first

---

## Youth System

### Costs
- Youth team facilities: 5,000 US$/week
- Each scout: 5,000 US$/week
- Maximum 3 scouts

### Mechanics
- Call scouts once per week
- Each scout presents one prospect per call
- Only one youth prospect can be recruited per week
- Scouts have Search Criteria (country + region preferences)

### Youth Players
- Age shown as "X years and Y days"
- **"Can be promoted in X days"** — minimum age to promote to senior team
- Skills shown as **current/max potential** format (e.g., "3/3", "?/5")
- Some skills show "unknown" until revealed through matches
- Speciality shown if known
- Maximum 16 youth players on roster
- **Reveal Player Skills** feature available

### Youth Team Features
- Own league (Youth League)
- Own stadium
- Own matches (friendlies, league)
- Can be restarted (once per 3 seasons/48 weeks)
- Can be closed

---

## Match Report Structure

### Sections (in order)
1. Match overview (score, possession bars, chance distribution graphic)
2. Match Info sidebar (teams, type, date, venue, attendance by seat type, match takings)
3. Highlights sidebar (goals, cards, injuries with minutes)
4. Report text (play-by-play events with minute markers and icons)
5. Rating Details (7-zone ratings for both teams)
6. Indirect Set Pieces ratings
7. Match Plan (attitude, tactic, skill, style of play)
8. Average Ratings (experience, midfield, defence, attack, total)
9. Lineup tab (visual pitch with both formations)
10. Chat section
11. Community reactions (press announcements)
