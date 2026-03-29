# Phase 1: Players & Teams

**Goal:** Core Player and Team models defined. Repositories operational. Player list and player detail pages functional. API-first: all services take `teamId`.

**Deliverable:** Players and teams create/read/update correctly. User can view player list and individual player details. Services work for any team (human or AI).

---

## Reference Material

- **Screenshots:** `Docs/Players Page.png` (player list), `Docs/player page.png` (player detail)
- **Wiki:** `wiki_archive/Training.html` (skills, training types), `wiki_archive/Match.html` (positions, formations, tactics, specialties)
- **Manual:** `Introduction » Manual » Help » Hattrick.pdf` (official rules, skill levels, specialties, individual orders)
- **Formulas:** `Docs/FORMULAS.md`

---

## Sprint 1: Enums & Value Types

No dependencies. Everything else builds on these.

**Recommended Model:** Haiku | **Recommended Effort:** Low

### Quartet 1: Core Skill Enums

SkillType enum:
```
Keeper, Defending, Playmaking, Winger, Scoring, Passing, SetPieces, Stamina
```

SkillLevel enum (0-20, using official Hattrick denominations as member names):
```
0: NonExistent         11: Brilliant
1: Disastrous          12: Magnificent
2: Wretched            13: WorldClass
3: Poor                14: Supernatural
4: Weak                15: Titanic
5: Inadequate          16: ExtraTerrestrial
6: Passable            17: Mythical
7: Solid               18: Magical
8: Excellent           19: Utopian
9: Formidable          20: Divine
10: Outstanding
```
Sub-levels (shown in match ratings): very low, low, high, very high

### Quartet 2: Position & Order Enums

Position enum:
```
Keeper, CentralDefender, WingBack, InnerMidfielder, Winger, Forward
```

IndividualOrder enum:
```
Normal, Offensive, Defensive, TowardsMiddle, TowardsWing
```

### Quartet 3: Tactics & Formation Enums

Formation enum:
```
442, 352, 433, 343, 541, 451, 532, 523, 550, 253
```

Tactic enum:
```
Normal, Pressing, CounterAttack, AttackInMiddle, AttackOnWings, PlayCreatively, LongShots
```

TeamAttitude enum:
```
PlayItCool, Normal, MatchOfTheSeason
```

### Quartet 4: Specialty & Personality Enums

Specialty enum:
```
None, Technical, Quick, Head, Powerful, Unpredictable, Resilient, Support
```

PlayerPersonality enum:
```
Nice, Nasty, Leader, Loner, Temperamental, Calm
```

CoachType enum:
```
Offensive, Defensive, Balanced
```

---

## Sprint 2: Player Model & Repository

Depends on: Sprint 1 (enums)

**Recommended Model:** Sonnet | **Recommended Effort:** Medium

### Quartet 1: Player Model

```csharp
Player:
  Id: Guid
  TeamId: Guid
  Name: string (first + last)
  Age: int (years)
  AgeDays: int (days within current year)
  Skills: Dictionary<SkillType, double> (sub-level tracking, e.g., 7.73)
  Specialty: Specialty
  Form: int (1-8)
  Stamina: int (1-9)
  Experience: int (1-20)
  Loyalty: double (0.0-1.0)
  IsMotherClub: bool
  TSI: int (Total Skill Index)
  InjuryWeeks: int (0 = healthy, >0 = weeks remaining)
  RedCard: bool
  YellowCards: int (0-2)
  JerseyNumber: int
  Wage: decimal
  Personality: PlayerPersonality
  Leadership: int (1-8)
  HTMS: int (Hattrick Total Match Score, ability points)
  Potential: int
  BestPosition: Position
  NativeCountryId: int
```

### Quartet 2: IPlayerRepository + PlayerRepository

- In-memory, thread-safe with `private readonly Lock _lock = new()`
- Returns `IReadOnlyList<Player>`
- Methods:
  - `GetByTeamId(Guid teamId)`
  - `GetById(Guid playerId)`
  - `Add(Player player)`
  - `Update(Player player)`
  - `Remove(Guid playerId)`
- Register as singleton in DI

---

## Sprint 3: Team Model & Repository

Depends on: Sprint 1 (enums)

**Recommended Model:** Sonnet | **Recommended Effort:** Medium

### Quartet 1: Team Model

```csharp
Team:
  Id: Guid
  Name: string
  IsHumanControlled: bool
  Budget: decimal
  Fans: int
  FanClubSize: int
  TeamSpirit: double (0-10)
  Confidence: double (0-10)
  CoachType: CoachType (Offensive, Defensive, Balanced)
  CoachLevel: int (1-8)
  AssistantCoachLevel: int (0-10)
  DoctorLevel: int (0-5)
  SpokespersonLevel: int (0-5)
  FinancialDirectorLevel: int (0-5)
```

### Quartet 2: ITeamRepository + TeamRepository

- In-memory, thread-safe with `private readonly Lock _lock = new()`
- Returns `IReadOnlyList<Team>`
- Methods:
  - `GetById(Guid teamId)`
  - `GetAll()`
  - `Add(Team team)`
  - `Update(Team team)`
- Register as singleton in DI

---

## Sprint 4: Player List UI

Depends on: Sprints 2-3 (repos)

**Recommended Model:** Sonnet | **Recommended Effort:** Medium-High

Reference: `Docs/Players Page.png`

### Quartet 1: Player List Page - Core Layout

Matches the Hattrick player list layout:
- Page title: "Your {count} players"
- Skill Table view (default)
- Each player row shows:
  - Player avatar/portrait
  - Name (linked to detail page)
  - Nationality flag
  - Age (years and days)
  - TSI
  - Wage
  - Specialty icon
  - Form bar
  - All 7 skill bars (Keeper through Set Pieces) with color coding
  - Best position
  - Last rating date

### Quartet 2: Player List Page - Sidebar Stats

Right sidebar:
- Team Total section (TSI, Wage, Estimated Value, Nationalities, Injured, Red cards, Yellow Cards)
- Team Average section (TSI, Wage, Estimated Value, Form, Stamina, Exp)

---

## Sprint 5: Player Detail & Team Overview UI

Depends on: Sprints 2-3 (repos)

**Recommended Model:** Sonnet | **Recommended Effort:** Medium-High

### Quartet 1: Player Detail Page - Header & Stats

Reference: `Docs/player page.png`

- Player header: jersey number, name, player ID
- Player info section:
  - Age (years and days), next birthday
  - Description line (e.g., "A sympathetic guy who is calm and unfavorable")
  - Personality, experience, leadership
  - Owner info (team name, since date)
  - Best position with rating
- Stats section:
  - TSI, Wage
  - All 7 skills with level names and color-coded bars
  - Form bar, Stamina bar
  - HTMS Ability / Potential

### Quartet 2: Player Detail Page - Sidebar & Tabs

- Right sidebar:
  - Owner actions
  - Player functions
  - Recommended Positions (pitch graphic showing best positions)
  - Player Stats (career goals, assists, etc.)
- Tab navigation below:
  - Matches, Transfers, Salaries, Injuries, Milestones, Development

### Quartet 3: Team Overview Page

Reference: `Docs/Home Page.png` (sidebar shows team info)

- Team name and badge
- Manager info
- Key stats: TeamSpirit, Confidence, Fans
- Staff levels display
- Budget summary
- Link to player list

---

## API-First Requirements

- Every service method signature: `ReturnType MethodName(Guid teamId, ...params)`
- Repositories registered as singletons in DI
- Services registered as scoped in DI
