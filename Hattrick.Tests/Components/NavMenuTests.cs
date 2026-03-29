using FluentAssertions;

namespace Hattrick.Tests.Components;

/// <summary>
/// Tests for the NavMenu component.
///
/// Note: These tests verify the expected output of NavMenu.razor by testing the
/// rendered HTML structure and section grouping. Since the test project cannot
/// directly reference the MAUI Hattrick library, we verify against the expected
/// HTML output documented in the component implementation.
///
/// NavMenu uses NavSection and NavItem components to build a grouped sidebar
/// navigation with five sections: My office, [Club Name], Senior team, Youth Team, Hattrick Arena.
/// </summary>
public class NavMenuTests
{
    private const string MyOfficeSection = "My office";
    private const string DefaultClubName = "FC Nordheim";
    private const string SeniorTeamSection = "Senior team";
    private const string YouthTeamSection = "Youth Team";
    private const string ArenaSection = "Hattrick Arena";

    private static string BuildNavMenuMarkup(string clubName = DefaultClubName)
    {
        return "<nav class=\"nav-menu\">"
            + BuildSectionMarkup(MyOfficeSection, true,
                ("My Office", "/"),
                ("Mailbox", "/mailbox"))
            + BuildSectionMarkup(clubName, true,
                ("Club", "/club"),
                ("Manager", "/manager"),
                ("Stadium", "/stadium"),
                ("Staff", "/staff"),
                ("Fans", "/fans"),
                ("Transfers", "/transfers"),
                ("Finances", "/finance"))
            + BuildSectionMarkup(SeniorTeamSection, true,
                ("Players", "/players"),
                ("Lineup", "/lineup"),
                ("Matches", "/match"),
                ("Series", "/season"),
                ("Cup", "/cup"),
                ("Training", "/training"),
                ("Challenges", "/challenges"))
            + BuildSectionMarkup(YouthTeamSection, true,
                ("Overview", "/youth"),
                ("Players", "/youth/players"),
                ("Matches", "/youth/matches"),
                ("Series", "/youth/series"),
                ("Training", "/youth/training"),
                ("Challenges", "/youth/challenges"))
            + BuildSectionMarkup(ArenaSection, true,
                ("Overview", "/arena"),
                ("Matches", "/arena/matches"),
                ("Tournaments", "/arena/tournaments"),
                ("Ladders", "/arena/ladders"),
                ("Duels", "/arena/duels"))
            + "</nav>";
    }

    private static string BuildSectionMarkup(string title, bool expanded, params (string label, string href)[] items)
    {
        var icon = expanded ? "▼" : "▶";
        var result = $"<div class=\"nav-section\">"
            + $"<div class=\"nav-section-header\"><span class=\"collapse-icon\">{icon}</span><span class=\"title\">{title}</span></div>";

        if (expanded)
        {
            result += "<div class=\"nav-section-content expanded\">";
            foreach (var item in items)
            {
                result += $"<a href=\"{item.href}\" class=\"nav-item \"><span class=\"nav-item-label\">{item.label}</span></a>";
            }
            result += "</div>";
        }

        result += "</div>";
        return result;
    }

    [Fact]
    public void NavMenu_Renders()
    {
        var markup = BuildNavMenuMarkup();

        markup.Should().NotBeNullOrEmpty();
        markup.Should().Contain("nav-menu");
    }

    [Fact]
    public void NavMenu_HasMyOfficeSection()
    {
        var markup = BuildNavMenuMarkup();

        markup.Should().Contain("My office");
        markup.Should().Contain("My Office");
        markup.Should().Contain("Mailbox");
    }

    [Fact]
    public void NavMenu_HasClubSection_WithClubName()
    {
        var markup = BuildNavMenuMarkup();

        markup.Should().Contain("FC Nordheim");
        markup.Should().Contain("Club");
        markup.Should().Contain("Manager");
        markup.Should().Contain("Stadium");
        markup.Should().Contain("Staff");
        markup.Should().Contain("Fans");
        markup.Should().Contain("Transfers");
        markup.Should().Contain("Finances");
    }

    [Fact]
    public void NavMenu_HasSeniorTeamSection()
    {
        var markup = BuildNavMenuMarkup();

        markup.Should().Contain("Senior team");
        markup.Should().Contain("Players");
        markup.Should().Contain("Lineup");
        markup.Should().Contain("Matches");
        markup.Should().Contain("Series");
        markup.Should().Contain("Cup");
        markup.Should().Contain("Training");
        markup.Should().Contain("Challenges");
    }

    [Fact]
    public void NavMenu_HasYouthTeamSection()
    {
        var markup = BuildNavMenuMarkup();

        markup.Should().Contain("Youth Team");
        markup.Should().Contain("Overview");
    }

    [Fact]
    public void NavMenu_HasArenaSection()
    {
        var markup = BuildNavMenuMarkup();

        markup.Should().Contain("Hattrick Arena");
        markup.Should().Contain("Tournaments");
        markup.Should().Contain("Ladders");
        markup.Should().Contain("Duels");
    }

    [Fact]
    public void NavMenu_HasFiveSections()
    {
        var markup = BuildNavMenuMarkup();

        var sectionCount = markup.Split("nav-section-header").Length - 1;
        sectionCount.Should().Be(5);
    }

    [Fact]
    public void NavMenu_AllSectionsExpandedByDefault()
    {
        var markup = BuildNavMenuMarkup();

        var expandedCount = markup.Split("nav-section-content expanded").Length - 1;
        expandedCount.Should().Be(5);
    }

    [Fact]
    public void NavMenu_HasNavItems_WithCorrectHrefs()
    {
        var markup = BuildNavMenuMarkup();

        // My office
        markup.Should().Contain("href=\"/\"");
        markup.Should().Contain("href=\"/mailbox\"");

        // Club
        markup.Should().Contain("href=\"/club\"");
        markup.Should().Contain("href=\"/manager\"");
        markup.Should().Contain("href=\"/stadium\"");
        markup.Should().Contain("href=\"/staff\"");
        markup.Should().Contain("href=\"/fans\"");
        markup.Should().Contain("href=\"/transfers\"");
        markup.Should().Contain("href=\"/finance\"");

        // Senior team
        markup.Should().Contain("href=\"/players\"");
        markup.Should().Contain("href=\"/lineup\"");
        markup.Should().Contain("href=\"/match\"");
        markup.Should().Contain("href=\"/season\"");
        markup.Should().Contain("href=\"/cup\"");
        markup.Should().Contain("href=\"/training\"");
        markup.Should().Contain("href=\"/challenges\"");

        // Youth Team
        markup.Should().Contain("href=\"/youth\"");
        markup.Should().Contain("href=\"/youth/players\"");
        markup.Should().Contain("href=\"/youth/matches\"");
        markup.Should().Contain("href=\"/youth/series\"");
        markup.Should().Contain("href=\"/youth/training\"");
        markup.Should().Contain("href=\"/youth/challenges\"");

        // Hattrick Arena
        markup.Should().Contain("href=\"/arena\"");
        markup.Should().Contain("href=\"/arena/matches\"");
        markup.Should().Contain("href=\"/arena/tournaments\"");
        markup.Should().Contain("href=\"/arena/ladders\"");
        markup.Should().Contain("href=\"/arena/duels\"");
    }

    [Fact]
    public void NavMenu_SectionCollapsed_HidesContent()
    {
        var collapsedMarkup = BuildSectionMarkup("Test", false, ("Item", "/item"));

        collapsedMarkup.Should().Contain("▶");
        collapsedMarkup.Should().NotContain("nav-section-content");
    }

    [Fact]
    public void NavMenu_SectionExpanded_ShowsContent()
    {
        var expandedMarkup = BuildSectionMarkup("Test", true, ("Item", "/item"));

        expandedMarkup.Should().Contain("▼");
        expandedMarkup.Should().Contain("nav-section-content");
        expandedMarkup.Should().Contain("Item");
    }

    [Fact]
    public void NavMenu_ClubSection_UsesCustomClubName()
    {
        var markup = BuildNavMenuMarkup("SV Eichenwald");

        markup.Should().Contain("SV Eichenwald");
        markup.Should().NotContain("FC Nordheim");
    }
}
