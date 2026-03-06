# Phase 1a: Lineup Manager UI Design Plan

**Goal:** Build the complete Lineup Manager interface matching the Hattrick design. No business logic yet—just UI structure, layout, and placeholder data.

**Deliverable:** Fully functional page with all 6 tabs, all visual components, proper layout, and navigation. Ready for Phase 1b to wire up data and logic.

---

## Overall Page Structure

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

---

## Component Breakdown

### 1. Core Layout Components

#### `LineupPage.razor`
- **Responsibility:** Container, tab state management, overall layout
- **Props:** None (loads from injected service)
- **State:** `currentTab: int` (0-5 for which tab is active)
- **Renders:**
  - Header info bar
  - Tab navigation
  - Active tab content
  - Bench panel

#### `TabNavigation.razor`
- **Responsibility:** Display 6 tabs, handle tab switching
- **Props:**
  - `CurrentTab: int`
  - `OnTabChanged: EventCallback<int>`
- **Content:** Tabs: Load, Lineup, Team Orders, Penalty Takers, Review, Send Orders
- **Styling:** Green background, active tab highlighted

#### `FormationPitch.razor`
- **Responsibility:** Display green football pitch with player positions
- **Props:**
  - `Formation: string` (e.g., "4-3-3")
  - `Players: List<PlayerInLineup>` (player ID, position on pitch)
  - `IsInteractive: bool` (drag/drop enabled on some tabs)
  - `OnPlayerClicked: EventCallback<PlayerInLineup>?`
  - `OnPlayerDragged: EventCallback<(PlayerId, Position)>?`
- **Renders:**
  - SVG/Canvas pitch with field markings
  - Goal area, center circle, etc.
  - Player avatars at their positions
  - Formation name/shape label (e.g., "4-3-3")
- **Styling:** Green field (CSS class), responsive sizing

#### `BenchPanel.razor`
- **Responsibility:** Show substitute players below pitch
- **Props:**
  - `SubstitutePlayers: List<PlayerInLineup>`
  - `OnPlayerClicked: EventCallback<PlayerInLineup>?`
- **Renders:**
  - Horizontal row of player avatars (subs)
  - Player jersey numbers, names (if space)
- **Styling:** Dark green background, smaller than pitch players

#### `PlayerAvatar.razor`
- **Responsibility:** Small visual representation of a player
- **Props:**
  - `Player: PlayerInLineup`
  - `Size: string` ("small", "medium", "large")
  - `ShowName: bool`
  - `IsSelected: bool`
  - `OnClick: EventCallback`
- **Renders:**
  - Player portrait/jersey number
  - Name label (optional)
  - Selection highlight (optional)
- **Styling:** Circular or square, responsive

---

### 2. Tab-Specific Right Panel Components

#### Tab 1: `LoadTabPanel.razor`
- **Responsibility:** Load saved lineups, match type selection
- **Content:**
  - "Submitted line-up" section
    - Show current lineup name/type
    - "Submitted" badge
  - "Start with empty line-up" button
  - "Standard" section
    - Radio buttons: Arena Matches, Friendly matches, Cup matches, Dog matches
  - "Saved line-ups" section
    - List of previously saved lineups with dates
    - Load buttons for each
  - "Previous line-ups" section
    - List of most recent lineups used

#### Tab 2: `LineupTabPanel.razor`
- **Responsibility:** Formation selection, tactics, individual orders
- **Content:**
  - Formation selector dropdown (4-4-2, 4-3-3, etc.)
  - Tactics selector dropdown (Normal, Pressing, Counter, etc.)
  - Attitude selector dropdown (Normal, PlayItCool, MatchOfTheSeason)
  - Team spirit display (if available)
  - Set Pieces Taker selector
  - Individual order controls for selected player
  - Substitution plan buttons

#### Tab 3: `TeamOrdersTabPanel.razor`
- **Responsibility:** Configure substitutions, behavioral changes, position swaps
- **Content:**
  - "New team order" section
    - Button: "Substitution"
    - Button: "Behavioral change"
    - Button: "Position swap"
  - "New team order" (expands when clicked to show options)
  - List of current orders being set

#### Tab 4: `PenaltyTakersTabPanel.razor`
- **Responsibility:** Select and order penalty takers
- **Content:**
  - Left: Pitch showing numbered slots 1-11 (penalty order positions)
  - Right: "Players" section
    - Sort dropdown (Category, TSI, etc.)
    - Player list with:
      - Player name
      - Jersey number
      - Position abbreviation
      - Skill bars (color-coded)
      - Selection state
    - Click to assign to penalty order

#### Tab 5: `ReviewTabPanel.razor`
- **Responsibility:** Show lineup summary with ratings
- **Content:**
  - Pitch showing player rating percentages (colored boxes)
  - Right panel with match details:
    - Formation
    - Tactics
    - Team Attitude
    - Team Spirit (if available)
    - Set Pieces Taker
    - Man-marking info
  - Upcoming/Previous matches list
  - Match details table (date, result, formation used)
  - Save options:
    - "Save as default line-up" checkbox
    - "Save as" text field
    - Save buttons

#### Tab 6: `SendOrdersTabPanel.razor`
- **Responsibility:** Submit the lineup
- **Content:**
  - Summary of lineup being sent
  - Confirmation message
  - "Send Orders" button (prominent)
  - Cancel button

---

### 3. Data Structure (Props/State)

All components use placeholder `List<T>` with empty lists. No service calls yet.

```csharp
// Player in a lineup (not the full Player model)
public record PlayerInLineup(
    string PlayerId,
    string Name,
    int JerseyNumber,
    string Position,          // "GK", "CB", "LB", "IM", "FW", etc.
    bool IsStarter,
    int SubstitutionOrder,    // 1, 2, 3, or null
    List<int> SkillLevels     // 8 skills, each 1-20
);

// Formation info
public record FormationInfo(
    string Name,              // "4-3-3"
    List<PlayerInLineup> Players
);

// Match info
public record MatchInfo(
    string TeamName,
    string MatchType,         // "Arena", "Friendly", "Cup"
    DateTime MatchDate
);
```

---

## Layout Specifications

### Pitch Sizing
- **Responsive:** 60% of container on desktop, full width on mobile
- **Aspect ratio:** 9:16 (taller than wide, vertical pitch orientation)
- **Player positions:** SVG/Canvas coordinates mapped to formation shape
- **Formation shapes:** Pre-defined SVG paths for each formation (4-4-2, 4-3-3, 3-5-2, etc.)

### Colors & Styling
- **Pitch:** Dark green (`#2d5016` or similar)
- **Lines:** White field markings
- **Players:** Colored circles/avatars (default team colors)
- **Active tab:** Bright green highlight
- **Inactive tab:** Muted gray
- **Right panel:** Light gray background
- **Buttons:** Green (primary), gray (secondary)

### Responsiveness
- **Desktop:** Pitch left (60%), panel right (40%)
- **Tablet:** Stack vertically, pitch first
- **Mobile:** Pitch full width, panel below, scrollable
- **Bench:** Always at bottom, horizontal scroll if needed

---

## Component Interaction Flow

1. **User lands on LineupPage**
   - Default tab: "Load"
   - Shows current submitted lineup or empty state
   - Pitch shows current formation (or empty)

2. **User clicks "Lineup" tab**
   - Shows formation selector, tactics selector
   - Pitch is interactive (can drag players, if implemented)
   - Right panel shows formation controls

3. **User clicks "Penalty Takers" tab**
   - Pitch changes to show numbered 1-11 slots
   - Players list appears on right
   - User clicks players to assign to order

4. **User clicks "Review" tab**
   - Pitch shows player rating percentages
   - Right panel shows match summary

5. **User clicks "Send Orders" tab**
   - Shows confirmation
   - "Send Orders" button submits (not functional yet)

---

## File Structure

```
Hattrick/Components/Pages/
└── Lineup.razor (main page, renamed from placeholder)

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
└── lineup.css (formation colors, pitch styling, responsive)
```

---

## Implementation Order (Phase 1a)

1. **Create basic page structure** (Lineup.razor + TabNavigation.razor)
   - Tabs switch between panels
   - State management for active tab

2. **Build FormationPitch.razor**
   - SVG pitch with field markings
   - Placeholder positions for players
   - Formation label display

3. **Build PlayerAvatar.razor**
   - Small visual component
   - Reusable across all tabs

4. **Build BenchPanel.razor**
   - Horizontal row of substitute players

5. **Build each tab panel** (in order: Load, Lineup, Team Orders, Penalties, Review, Send)
   - Right-side content for each tab
   - Form controls, lists, selectors

6. **Wire layout together**
   - Responsive grid/flexbox
   - Proper sizing and alignment

7. **Style and polish**
   - Match Hattrick green aesthetic
   - Responsive breakpoints
   - Hover states, animations

---

## Acceptance Criteria

- ✅ All 6 tabs render and switch correctly
- ✅ Pitch displays with proper formation shape
- ✅ All tab-specific panels display their content
- ✅ Layout matches screenshot (pitch 60/40 split on desktop)
- ✅ No console errors
- ✅ Responsive on mobile/tablet
- ✅ No business logic (data is hardcoded/placeholder)
- ✅ Ready for Phase 1b to add services and real data

---

## Notes for Phase 1b Integration

Phase 1b will:
- Inject `ILineupService`, `IPlayerRepository`, `ITeamRepository`
- Replace placeholder `List<PlayerInLineup>` with real data from services
- Implement tab content population from actual game state
- Add drag/drop functionality to pitch
- Add form submissions and validation
- Wire up "Send Orders" button to save/submit

No changes to component structure needed—just replace placeholder data with service calls.
