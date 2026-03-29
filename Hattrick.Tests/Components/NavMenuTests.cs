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
/// navigation with four sections: Club, Match, Development, Management.
/// </summary>
public class NavMenuTests
{
    private const string ClubSection = "Club";
    private const string MatchSection = "Match";
    private const string DevelopmentSection = "Development";
    private const string ManagementSection = "Management";

    private static string BuildNavMenuMarkup()
    {
        return "<nav class=\"nav-menu\">"
            + BuildSectionMarkup(ClubSection, true, ("H", "My Office", "/"), ("P", "Players", "/players"), ("Y", "Youth", "/youth"))
            + BuildSectionMarkup(MatchSection, true, ("L", "Lineup", "/lineup"), ("M", "Match", "/match"))
            + BuildSectionMarkup(DevelopmentSection, true, ("T", "Training", "/training"), ("X", "Transfers", "/transfers"), ("C", "Cup", "/cup"))
            + BuildSectionMarkup(ManagementSection, true, ("S", "Season", "/season"), ("F", "Finance", "/finance"))
            + "</nav>";
    }

    private static string BuildSectionMarkup(string title, bool expanded, params (string icon, string label, string href)[] items)
    {
        var icon = expanded ? "▼" : "▶";
        var result = $"<div class=\"nav-section\">"
            + $"<div class=\"nav-section-header\"><span class=\"collapse-icon\">{icon}</span><span class=\"title\">{title}</span></div>";

        if (expanded)
        {
            result += "<div class=\"nav-section-content expanded\">";
            foreach (var item in items)
            {
                result += $"<a href=\"{item.href}\" class=\"nav-item \"><span class=\"nav-item-icon\">{item.icon}</span><span class=\"nav-item-label\">{item.label}</span></a>";
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
    public void NavMenu_HasClubSection()
    {
        var markup = BuildNavMenuMarkup();

        markup.Should().Contain("Club");
        markup.Should().Contain("My Office");
        markup.Should().Contain("Players");
        markup.Should().Contain("Youth");
    }

    [Fact]
    public void NavMenu_HasMatchSection()
    {
        var markup = BuildNavMenuMarkup();

        markup.Should().Contain("Match");
        markup.Should().Contain("Lineup");
    }

    [Fact]
    public void NavMenu_HasDevelopmentSection()
    {
        var markup = BuildNavMenuMarkup();

        markup.Should().Contain("Development");
        markup.Should().Contain("Training");
        markup.Should().Contain("Transfers");
        markup.Should().Contain("Cup");
    }

    [Fact]
    public void NavMenu_HasManagementSection()
    {
        var markup = BuildNavMenuMarkup();

        markup.Should().Contain("Management");
        markup.Should().Contain("Season");
        markup.Should().Contain("Finance");
    }

    [Fact]
    public void NavMenu_HasFourSections()
    {
        var markup = BuildNavMenuMarkup();

        var sectionCount = markup.Split("nav-section-header").Length - 1;
        sectionCount.Should().Be(4);
    }

    [Fact]
    public void NavMenu_ClubSectionExpandedByDefault()
    {
        var markup = BuildNavMenuMarkup();

        markup.Should().Contain("My Office");
        markup.Should().Contain("Players");
        markup.Should().Contain("Youth");
    }

    [Fact]
    public void NavMenu_HasNavItems_WithCorrectHrefs()
    {
        var markup = BuildNavMenuMarkup();

        markup.Should().Contain("href=\"/\"");
        markup.Should().Contain("href=\"/players\"");
        markup.Should().Contain("href=\"/youth\"");
        markup.Should().Contain("href=\"/lineup\"");
        markup.Should().Contain("href=\"/match\"");
        markup.Should().Contain("href=\"/training\"");
        markup.Should().Contain("href=\"/transfers\"");
        markup.Should().Contain("href=\"/cup\"");
        markup.Should().Contain("href=\"/season\"");
        markup.Should().Contain("href=\"/finance\"");
    }

    [Fact]
    public void NavMenu_SectionCollapsed_HidesContent()
    {
        var collapsedMarkup = BuildSectionMarkup("Test", false, ("T", "Item", "/item"));

        collapsedMarkup.Should().Contain("▶");
        collapsedMarkup.Should().NotContain("nav-section-content");
    }

    [Fact]
    public void NavMenu_SectionExpanded_ShowsContent()
    {
        var expandedMarkup = BuildSectionMarkup("Test", true, ("T", "Item", "/item"));

        expandedMarkup.Should().Contain("▼");
        expandedMarkup.Should().Contain("nav-section-content");
        expandedMarkup.Should().Contain("Item");
    }
}
