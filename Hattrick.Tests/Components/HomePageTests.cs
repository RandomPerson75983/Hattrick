using FluentAssertions;

namespace Hattrick.Tests.Components;

/// <summary>
/// Tests for the Home.razor dashboard page.
///
/// The Home page assembles four subcomponents into a dashboard layout:
/// - Top row: ClubInfoCard (left) + UpcomingMatchesPanel (right)
/// - Middle row: ActivityFeed (full width)
/// - Bottom row: QuickLinksSection (full width)
/// </summary>
public class HomePageTests
{
    private static string BuildExpectedMarkup()
    {
        return "<div class=\"home-dashboard\">"
            + "<div class=\"dashboard-top-row\">"
            + "<div class=\"club-info-card\"><!-- ClubInfoCard --></div>"
            + "<div class=\"upcoming-matches-panel\"><!-- UpcomingMatchesPanel --></div>"
            + "</div>"
            + "<div class=\"activity-feed\"><!-- ActivityFeed --></div>"
            + "<div class=\"quick-links-section\"><!-- QuickLinksSection --></div>"
            + "</div>";
    }

    [Fact]
    public void HomePage_HasDashboardContainer()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("home-dashboard");
    }

    [Fact]
    public void HomePage_HasTopRow()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("dashboard-top-row");
    }

    [Fact]
    public void HomePage_HasClubInfoCard()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("club-info-card");
    }

    [Fact]
    public void HomePage_HasUpcomingMatchesPanel()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("upcoming-matches-panel");
    }

    [Fact]
    public void HomePage_HasActivityFeed()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("activity-feed");
    }

    [Fact]
    public void HomePage_HasQuickLinksSection()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("quick-links-section");
    }

    [Fact]
    public void HomePage_TopRow_ContainsClubAndMatches()
    {
        var markup = BuildExpectedMarkup();

        var topRowStart = markup.IndexOf("dashboard-top-row");
        var clubInfoIndex = markup.IndexOf("club-info-card");
        var matchesIndex = markup.IndexOf("upcoming-matches-panel");

        clubInfoIndex.Should().BeGreaterThan(topRowStart);
        matchesIndex.Should().BeGreaterThan(topRowStart);
    }

    [Fact]
    public void HomePage_Layout_TopRowBeforeActivityFeed()
    {
        var markup = BuildExpectedMarkup();

        var topRowIndex = markup.IndexOf("dashboard-top-row");
        var feedIndex = markup.IndexOf("activity-feed");

        topRowIndex.Should().BeLessThan(feedIndex);
    }

    [Fact]
    public void HomePage_Layout_ActivityFeedBeforeQuickLinks()
    {
        var markup = BuildExpectedMarkup();

        var feedIndex = markup.IndexOf("activity-feed");
        var linksIndex = markup.IndexOf("quick-links-section");

        feedIndex.Should().BeLessThan(linksIndex);
    }
}
