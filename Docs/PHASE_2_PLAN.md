# Phase 2: Team Creation & Lineup Management

**Goal:** Basic team and squad generation so the game has data to work with. Lineup management fully functional. Services work for any team (human or AI).

**Deliverable:** A team with a generated squad of players exists. Player can set starting XI, positions, orders, substitution plan via the Lineup Manager UI.

---

## Basic Team Creation

Minimal team/squad generation so lineup and match phases have real data. Full division generation is in Phase 5.

- `ITeamGenerationService` - generate a single team with a squad of ~25 players
- `IPlayerGenerationService` - create individual players with name, age, skills, specialty
- Basic name generation (placeholder names, refined in Phase 5)
- Hardcoded skill distributions by position
- Generate one test team on app startup for development/testing

---

## Key Models

- **MatchLineupSlot:** Position, IndividualOrder, IsStarter, Substitution plan
- **TeamLineup:** Formation, Tactic, Attitude, 11 starters + 3 subs

## Services (all methods take `Guid teamId` as first parameter)

- `ILineupService` - lineup validation, auto-suggest, substitution plan, man marking

---

## Lineup Manager UI

### Overall Page Structure

```
LineupPage.razor (root component)
├── Header Section
│   ├── Team/Match Info Bar (team name, match type, date)
│   └── Tab Navigation Bar
│       ├── Load (Tab 1)
│       ├── Lineup (Tab 2)
│       ├── Team Orders (Tab 3)
│       ├── Penalty Takers (Tab 4)
│       ├── Review (Tab 5)
│       └── Send Orders (Tab 6)
│
├── Main Content Area (flexbox: pitch + right panel)
│   ├── FormationPitch.razor (left 60%)
│   │   ├── Pitch SVG/Canvas
│   │   ├── Player avatars positioned by formation
│   │   └── Formation label (e.g., "4-3-3")
│   │
│   └── RightPanel.razor (right 40%, content varies by tab)
│       ├── [Tab-specific content]
│
└── Bench Section (full width below)
    └── BenchPanel.razor
        ├── Substitute player avatars
        └── Available player list
```

### Core Layout Components

#### `LineupPage.razor`
- Container, tab state management, overall layout
- State: `currentTab: int` (0-5 for which tab is active)

#### `TabNavigation.razor`
- Display 6 tabs, handle tab switching
- Props: `CurrentTab: int`, `OnTabChanged: EventCallback<int>`
- Styling: Green background, active tab highlighted

#### `FormationPitch.razor`
- Display green football pitch with player positions
- Props: `Formation: string`, `Players: List<PlayerInLineup>`, `IsInteractive: bool`, click/drag callbacks
- SVG/Canvas pitch with field markings, goal area, center circle
- Player avatars at their positions, formation name label

#### `BenchPanel.razor`
- Horizontal row of substitute players below pitch
- Dark green background, smaller than pitch players

#### `PlayerAvatar.razor`
- Small visual representation of a player (circular or square, responsive)
- Props: `Player`, `Size`, `ShowName`, `IsSelected`, `OnClick`

### Tab-Specific Right Panel Components

#### Tab 1: `LoadTabPanel.razor`
- Load saved lineups, match type selection
- "Submitted line-up" section, "Start with empty line-up" button
- Radio buttons: Arena Matches, Friendly matches, Cup matches, Dog matches
- Saved line-ups list, previous line-ups list

#### Tab 2: `LineupTabPanel.razor`
- Formation selector dropdown (4-4-2, 4-3-3, etc.)
- Tactics selector dropdown (Normal, Pressing, Counter, etc.)
- Attitude selector dropdown (Normal, PlayItCool, MatchOfTheSeason)
- Team spirit display, Set Pieces Taker selector
- Individual order controls, substitution plan buttons

#### Tab 3: `TeamOrdersTabPanel.razor`
- Configure substitutions, behavioral changes, position swaps
- Buttons: "Substitution", "Behavioral change", "Position swap"
- List of current orders being set

#### Tab 4: `PenaltyTakersTabPanel.razor`
- Pitch showing numbered slots 1-11 (penalty order positions)
- Player list with sort dropdown, skill bars, selection state

#### Tab 5: `ReviewTabPanel.razor`
- Pitch showing player rating percentages (colored boxes)
- Match details: Formation, Tactics, Team Attitude, Team Spirit, Set Pieces Taker, Man-marking
- Upcoming/Previous matches list
- Save options: "Save as default line-up", "Save as" text field

#### Tab 6: `SendOrdersTabPanel.razor`
- Summary of lineup being sent
- Confirmation message, "Send Orders" button, Cancel button

### Data Structures

```csharp
public record PlayerInLineup(
    string PlayerId,
    string Name,
    int JerseyNumber,
    string Position,          // "GK", "CB", "LB", "IM", "FW", etc.
    bool IsStarter,
    int SubstitutionOrder,    // 1, 2, 3, or null
    List<int> SkillLevels     // 8 skills, each 1-20
);

public record FormationInfo(
    string Name,              // "4-3-3"
    List<PlayerInLineup> Players
);

public record MatchInfo(
    string TeamName,
    string MatchType,         // "Arena", "Friendly", "Cup"
    DateTime MatchDate
);
```

### Layout Specifications

- **Pitch:** Responsive, 60% of container on desktop, full width on mobile. Aspect ratio 9:16.
- **Colors:** Pitch dark green (`#2d5016`), white field markings, green buttons (primary), gray (secondary)
- **Responsiveness:** Desktop: pitch left 60%, panel right 40%. Tablet: stack vertically. Mobile: full width, scrollable.

### File Structure

```
Hattrick/Components/Pages/
└── Lineup.razor

Hattrick/Components/Shared/Lineup/
├── TabNavigation.razor
├── FormationPitch.razor
├── BenchPanel.razor
├── PlayerAvatar.razor
├── LoadTabPanel.razor
├── LineupTabPanel.razor
├── TeamOrdersTabPanel.razor
├── PenaltyTakersTabPanel.razor
├── ReviewTabPanel.razor
└── SendOrdersTabPanel.razor

Hattrick/Components/Shared/Lineup/Styles/
└── lineup.css
```
