# Main Layout & Home Page Design Plan

**Goal:** Establish a consistent layout structure that matches Hattrick's design. All pages (Home, Season, Lineup, Match, Training, Finance, Transfers, Youth, Cup) will use this layout.

**Deliverable:** Redesigned MainLayout.razor, NavMenu.razor, and Home.razor. Consistent look across all pages. Ready for Phase 1a to use as base.

---

## Overall Page Structure

Based on Hattrick screenshots, the layout is:

```
┌─────────────────────────────────────────────────────┐
│  Header Bar (logo, team name, stats bar)            │
├──────────────┬──────────────────────┬──────────────┤
│              │                      │              │
│   Left       │   Main Content       │   Right      │
│   Sidebar    │   Area               │   Panel      │
│ (Nav Menu)   │   (Page specific)    │   (Info)     │
│              │                      │              │
│              │                      │              │
└──────────────┴──────────────────────┴──────────────┘
```

### Widths (Desktop)
- Left sidebar: 180-200px (fixed or collapsible)
- Main content: Flexible (rest of space)
- Right panel: 200-250px (varies by page, sometimes hidden)

### Responsive
- Tablet: Left sidebar collapses to icons, reduced widths
- Mobile: Left sidebar slides out as drawer, right panel below main content

---

## Component Structure

### `MainLayout.razor`
**Responsibility:** Root layout wrapper, provides consistent structure for all pages

**Structure:**
```razor
<div class="app-container">
    <header class="app-header">
        <HeaderBar />
    </header>

    <div class="app-body">
        <nav class="left-sidebar">
            <NavMenu />
        </nav>

        <main class="main-content">
            @Body
        </main>

        @if (ShowRightPanel)
        {
            <aside class="right-panel">
                <RightPanelContent />
            </aside>
        }
    </div>
</div>
```

**Styling:**
- `app-container`: full viewport height, flexbox column
- `app-header`: fixed height (~60px), contains Hattrick logo + team info
- `app-body`: flex row, grows to fill remaining space
- `left-sidebar`: fixed width, scrollable
- `main-content`: flex-grow, scrollable
- `right-panel`: fixed width, scrollable

**Props/State:**
- `ShowRightPanel: bool` (pages set this to show/hide right panel)
- `RightPanelContent: RenderFragment?` (pages provide their own right panel content)
- `PageTitle: string` (display in header or breadcrumb)

---

### `HeaderBar.razor`
**Responsibility:** Top bar with logo, team info, user stats

**Content (left to right):**
1. **Logo/Home button** - "hattrick" text + icon, clickable to home
2. **Team info section**
   - Team name (clickable to team page)
   - Match info or current status (small text)
3. **Spacer/center area** - Can hold breadcrumb or page title
4. **Quick stats bar** (right side)
   - Budget / Fans / Team Spirit indicators
   - Small icons with numbers
5. **User menu** (far right)
   - Settings icon
   - Logout/User menu

**Styling:**
- Background: Dark gray or navy (`#1a1a1a`)
- Text: White/light gray
- Height: 60px
- Logo: Left-aligned, 20px padding
- Stats: Right-aligned, 20px padding
- Responsive: Hide some stats on mobile

---

### `NavMenu.razor` (Redesigned)
**Responsibility:** Left sidebar navigation, matches Hattrick's nav structure

**Structure:**
```
├── Club Section
│   ├── My Office (Home)
│   ├── Players
│   └── Youth
│
├── Match Section
│   ├── Lineup
│   ├── Match
│   └── Review
│
├── Development Section
│   ├── Training
│   ├── Transfers
│   └── Cup
│
├── Management Section
│   ├── Season
│   ├── Finance
│   └── [Other sections]
│
└── Footer Section
    ├── Settings
    ├── Help
    └── Logout
```

**Components:**
- `NavMenu.razor` - Container with sections
- `NavSection.razor` - Collapsible section header
- `NavItem.razor` - Individual nav link

**NavItem.razor Props:**
- `Icon: string` - Icon name/class (FontAwesome or custom)
- `Label: string` - Display text
- `Href: string` - Route
- `IsActive: bool` - Highlight current page
- `OnClick: EventCallback` - Optional custom action

**Styling:**
- Background: Medium green (`#3a7d3a` or similar)
- Text: Light/white
- Active item: Bright green highlight or darker background
- Hover: Slight color change
- Icons: 20x20px, left-aligned
- Labels: Hidden on mobile (icons only)
- Sections: Collapsible on mobile

---

### `Home.razor` (Redesigned - "My Office")
**Responsibility:** Main dashboard page matching Hattrick's home screen

**Layout (Main Content Area):**

```
┌─────────────────────────────────┐
│  My Office (Page Title/Tab)     │
├─────────────────────────────────┤
│                                 │
│  Club Info Card (left)          │  Upcoming Matches (right)
│  - Logo/Badge                   │  - Next 3 matches
│  - Club name                    │  - Dates, opponents
│  - Division                     │  - Formation used
│  - Fan count                    │
│                                 │
├─────────────────────────────────┤
│                                 │
│  Recent Activity / News Feed    │
│  - Match results                │
│  - Training updates             │
│  - Player news                  │
│  - League standings snippet     │
│                                 │
├─────────────────────────────────┤
│                                 │
│  Quick Links / Shortcuts        │
│  - Set Lineup                   │
│  - View Squad                   │
│  - Check Finances               │
│  - Browse Transfer Market       │
│                                 │
└─────────────────────────────────┘
```

**Components:**
- `ClubInfoCard.razor` - Team details
- `UpcomingMatchesPanel.razor` - Next matches
- `ActivityFeed.razor` - Recent events/news
- `QuickLinksSection.razor` - Shortcuts to common actions

**Right Panel (if shown):**
- Team stats summary
- Budget/Finances overview
- Recent match results

**Styling:**
- Background: Light gray (#f5f5f5)
- Cards: White with subtle shadow
- Spacing: Consistent padding between sections
- Responsive: Single column on mobile, sections stack

---

## Navigation Flow

### Pages & Routes
```
/                    → Home (My Office)
/season              → Season/League Management
/lineup              → Lineup Manager (all 6 tabs)
/match               → Match View/Review
/training            → Training Configuration
/finance             → Financial Management
/transfers           → Transfer Market
/youth               → Youth Squad Management
/cup                 → Cup Competition
/settings            → Settings (future)
/help                → Help/About (future)
```

### NavMenu Active State
- When user is on `/lineup`, the Lineup nav item highlights
- When on `/season`, Season item highlights
- Home icon always highlights when on `/`

---

## Color Scheme & Styling

### Primary Colors
- **Dark Background:** #1a1a1a (header, borders)
- **Nav Green:** #3a7d3a (left sidebar)
- **Accent Green:** #4CAF50 (highlights, buttons, active states)
- **Light Background:** #f5f5f5 (main content area)
- **Card Background:** #ffffff (cards, panels)
- **Text Primary:** #333333 (dark gray, main text)
- **Text Secondary:** #666666 (lighter gray, labels)
- **Text Light:** #ffffff (on dark backgrounds)

### Hattrick Yellow (accent)
- **Yellow:** #FFD700 or #FFC107 (for important info, badges)
- Used sparingly for alerts, match days, important dates

### Typography
- **Font:** Sans-serif (Arial, Helvetica, or system font)
- **Header:** 20-24px, bold
- **Section titles:** 16-18px, semi-bold
- **Body text:** 14px, normal
- **Small text:** 12px, normal

---

## Responsive Breakpoints

### Desktop (1200px+)
- 3-column layout: sidebar | main | right-panel
- Left sidebar fully visible with labels
- Right panel visible (when applicable)
- Full header with all elements

### Tablet (768px - 1199px)
- 2-column layout: sidebar-icons | main
- Left sidebar collapses to icons only (80px)
- Right panel hidden or moved below main content
- Header simplified (hide some stats)

### Mobile (< 768px)
- 1-column layout: main content
- Left sidebar hidden, opens as drawer on menu click
- Right panel below main content (full width, scrollable)
- Header simplified (logo + hamburger menu only)

---

## CSS Structure

```
Hattrick/Components/Shared/Layout/
├── MainLayout.razor
├── MainLayout.razor.css
├── HeaderBar.razor
├── HeaderBar.razor.css
├── NavMenu.razor
├── NavMenu.razor.css
├── NavSection.razor
├── NavSection.razor.css
├── NavItem.razor
└── NavItem.razor.css

Hattrick/Components/Pages/
├── Home.razor (redesigned)
├── Home.razor.css
├── ClubInfoCard.razor
├── UpcomingMatchesPanel.razor
├── ActivityFeed.razor
└── QuickLinksSection.razor
```

---

## File Changes Required

### New Files
- `HeaderBar.razor` + `.css`
- `NavSection.razor` + `.css`
- `NavItem.razor` + `.css`
- `ClubInfoCard.razor`
- `UpcomingMatchesPanel.razor`
- `ActivityFeed.razor`
- `QuickLinksSection.razor`
- `Home.razor.css` (new styling)

### Modified Files
- `MainLayout.razor` (complete redesign)
- `MainLayout.razor.css` (new)
- `NavMenu.razor` (redesign to use sections/items)
- `NavMenu.razor.css` (new)
- `Home.razor` (redesign)

### Deleted Files
- Current placeholder pages can be removed once redesigned

---

## Implementation Order

1. **Create CSS foundation**
   - Global styles (colors, typography)
   - Layout utilities (flexbox, grid)
   - Responsive mixins

2. **Build HeaderBar.razor**
   - Logo, team name, stats
   - User menu placeholder
   - Responsive behavior

3. **Build NavMenu structure**
   - NavSection.razor
   - NavItem.razor
   - Populate with menu items

4. **Redesign MainLayout.razor**
   - Integrate header, nav, main content, right panel
   - Responsive layout with flexbox
   - CSS grid for mobile drawer

5. **Redesign Home.razor**
   - ClubInfoCard.razor
   - UpcomingMatchesPanel.razor
   - ActivityFeed.razor
   - QuickLinksSection.razor

6. **Polish & Responsive Testing**
   - Mobile drawer (hamburger menu)
   - Tablet layout
   - Desktop layout
   - Cross-browser testing

---

## Acceptance Criteria

- ✅ Header displays with logo, team info, stats
- ✅ Left sidebar navigation fully functional
- ✅ Nav items highlight active page correctly
- ✅ Home page shows club info, matches, activity feed
- ✅ Layout is 3-column on desktop
- ✅ Layout adapts to tablet (sidebar collapses)
- ✅ Layout adapts to mobile (sidebar becomes drawer)
- ✅ All pages can use MainLayout without modification
- ✅ Right panel can be shown/hidden per page
- ✅ Color scheme matches Hattrick aesthetic
- ✅ No console errors
- ✅ Ready for Phase 1a to build on top

---

## Notes

- **Placeholder Data:** All components use placeholder data (no services yet)
- **No Business Logic:** Just layout and display structure
- **Reusable:** NavMenu and Header are reused on every page
- **Future Phase 1b:** Wire up services to populate real data
- **Phase 1a Uses This:** LineupPage.razor will use MainLayout and NavMenu
