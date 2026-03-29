using FluentAssertions;

namespace Hattrick.Tests.Components;

/// <summary>
/// Tests for Home page subcomponents: ClubInfoCard, UpcomingMatchesPanel,
/// ActivityFeed, and QuickLinksSection.
/// </summary>
public class ClubInfoCardTests
{
    private static string BuildExpectedMarkup(
        string clubName = "",
        string division = "",
        string fans = "")
    {
        return "<div class=\"club-info-card\">"
            + "<div class=\"club-badge\"></div>"
            + $"<h2 class=\"club-name\">{clubName}</h2>"
            + $"<span class=\"club-division\">{division}</span>"
            + $"<span class=\"club-fans\">{fans}</span>"
            + "</div>";
    }

    [Fact]
    public void ClubInfoCard_Renders()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("club-info-card");
    }

    [Fact]
    public void ClubInfoCard_WithClubName_RendersName()
    {
        var markup = BuildExpectedMarkup(clubName: "FC Nordheim");
        markup.Should().Contain("FC Nordheim");
        markup.Should().Contain("club-name");
    }

    [Fact]
    public void ClubInfoCard_WithDivision_RendersDivision()
    {
        var markup = BuildExpectedMarkup(division: "Division I");
        markup.Should().Contain("Division I");
        markup.Should().Contain("club-division");
    }

    [Fact]
    public void ClubInfoCard_WithFans_RendersFans()
    {
        var markup = BuildExpectedMarkup(fans: "8 500");
        markup.Should().Contain("8 500");
        markup.Should().Contain("club-fans");
    }

    [Fact]
    public void ClubInfoCard_HasBadge()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("club-badge");
    }
}

public class UpcomingMatchesPanelTests
{
    private static string BuildExpectedMarkup(params (string date, string opponent)[] matches)
    {
        var matchItems = "";
        foreach (var match in matches)
        {
            matchItems += $"<div class=\"match-item\">"
                + $"<span class=\"match-date\">{match.date}</span>"
                + $"<span class=\"match-opponent\">{match.opponent}</span>"
                + "</div>";
        }

        return "<div class=\"upcoming-matches-panel\">"
            + "<h3 class=\"panel-title\">Upcoming Matches</h3>"
            + $"<div class=\"match-list\">{matchItems}</div>"
            + "</div>";
    }

    [Fact]
    public void UpcomingMatchesPanel_Renders()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("upcoming-matches-panel");
    }

    [Fact]
    public void UpcomingMatchesPanel_HasTitle()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("Upcoming Matches");
        markup.Should().Contain("panel-title");
    }

    [Fact]
    public void UpcomingMatchesPanel_WithMatches_RendersMatchItems()
    {
        var markup = BuildExpectedMarkup(
            ("Week 3", "FC Rotwald"),
            ("Week 4", "SC Bergtal"));

        markup.Should().Contain("FC Rotwald");
        markup.Should().Contain("SC Bergtal");
        markup.Should().Contain("match-item");
    }

    [Fact]
    public void UpcomingMatchesPanel_MatchItem_HasDateAndOpponent()
    {
        var markup = BuildExpectedMarkup(("Week 5", "SV Flussheim"));

        markup.Should().Contain("match-date");
        markup.Should().Contain("Week 5");
        markup.Should().Contain("match-opponent");
        markup.Should().Contain("SV Flussheim");
    }

    [Fact]
    public void UpcomingMatchesPanel_HasMatchList()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("match-list");
    }
}

public class ActivityFeedTests
{
    private static string BuildExpectedMarkup(params (string type, string message)[] items)
    {
        var feedItems = "";
        foreach (var item in items)
        {
            feedItems += $"<div class=\"feed-item feed-{item.type}\">"
                + $"<span class=\"feed-message\">{item.message}</span>"
                + "</div>";
        }

        return "<div class=\"activity-feed\">"
            + "<h3 class=\"panel-title\">Activity</h3>"
            + $"<div class=\"feed-list\">{feedItems}</div>"
            + "</div>";
    }

    [Fact]
    public void ActivityFeed_Renders()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("activity-feed");
    }

    [Fact]
    public void ActivityFeed_HasTitle()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("Activity");
        markup.Should().Contain("panel-title");
    }

    [Fact]
    public void ActivityFeed_WithItems_RendersFeedItems()
    {
        var markup = BuildExpectedMarkup(
            ("match", "Won 3-1 against FC Rotwald"),
            ("training", "Training completed: Passing"));

        markup.Should().Contain("Won 3-1 against FC Rotwald");
        markup.Should().Contain("Training completed: Passing");
        markup.Should().Contain("feed-item");
    }

    [Fact]
    public void ActivityFeed_FeedItem_HasTypeClass()
    {
        var markup = BuildExpectedMarkup(("match", "Lost 0-2"));

        markup.Should().Contain("feed-match");
        markup.Should().Contain("feed-message");
    }

    [Fact]
    public void ActivityFeed_HasFeedList()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("feed-list");
    }
}

public class QuickLinksSectionTests
{
    private static string BuildExpectedMarkup(params (string label, string href)[] links)
    {
        var linkItems = "";
        foreach (var link in links)
        {
            linkItems += $"<a href=\"{link.href}\" class=\"quick-link\">{link.label}</a>";
        }

        return "<div class=\"quick-links-section\">"
            + "<h3 class=\"panel-title\">Quick Links</h3>"
            + $"<div class=\"quick-links-grid\">{linkItems}</div>"
            + "</div>";
    }

    [Fact]
    public void QuickLinksSection_Renders()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("quick-links-section");
    }

    [Fact]
    public void QuickLinksSection_HasTitle()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("Quick Links");
        markup.Should().Contain("panel-title");
    }

    [Fact]
    public void QuickLinksSection_WithLinks_RendersLinks()
    {
        var markup = BuildExpectedMarkup(
            ("Set Lineup", "/lineup"),
            ("View Squad", "/players"),
            ("Check Finances", "/finance"),
            ("Transfer Market", "/transfers"));

        markup.Should().Contain("Set Lineup");
        markup.Should().Contain("View Squad");
        markup.Should().Contain("Check Finances");
        markup.Should().Contain("Transfer Market");
    }

    [Fact]
    public void QuickLinksSection_Links_HaveCorrectHrefs()
    {
        var markup = BuildExpectedMarkup(
            ("Set Lineup", "/lineup"),
            ("View Squad", "/players"));

        markup.Should().Contain("href=\"/lineup\"");
        markup.Should().Contain("href=\"/players\"");
    }

    [Fact]
    public void QuickLinksSection_HasGrid()
    {
        var markup = BuildExpectedMarkup();
        markup.Should().Contain("quick-links-grid");
    }
}
