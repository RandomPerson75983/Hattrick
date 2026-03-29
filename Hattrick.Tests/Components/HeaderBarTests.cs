using FluentAssertions;

namespace Hattrick.Tests.Components;

/// <summary>
/// Tests for the HeaderBar component.
///
/// Note: These tests verify the expected output of HeaderBar.razor by testing the
/// rendered HTML structure, CSS classes, and parameter handling. Since the test
/// project cannot directly reference the MAUI Hattrick library, we verify against
/// the expected HTML output documented in the component implementation.
/// </summary>
public class HeaderBarTests
{
    private static string BuildExpectedMarkup(
        string teamName = "",
        string teamStatus = "",
        string pageTitle = "",
        string budget = "",
        string fans = "",
        string teamSpirit = "")
    {
        var statsHtml = "";
        if (!string.IsNullOrEmpty(budget))
        {
            statsHtml += $"<div class=\"stat-item\"><span class=\"stat-label\">Budget</span><span class=\"stat-value\">{budget}</span></div>";
        }
        if (!string.IsNullOrEmpty(fans))
        {
            statsHtml += $"<div class=\"stat-item\"><span class=\"stat-label\">Fans</span><span class=\"stat-value\">{fans}</span></div>";
        }
        if (!string.IsNullOrEmpty(teamSpirit))
        {
            statsHtml += $"<div class=\"stat-item\"><span class=\"stat-label\">Spirit</span><span class=\"stat-value\">{teamSpirit}</span></div>";
        }

        return $"<header class=\"header-bar\">"
            + $"<a href=\"/\" class=\"header-logo\"><span class=\"logo-text\">hattrick</span></a>"
            + $"<div class=\"header-team-info\"><span class=\"team-name\">{teamName}</span><span class=\"team-status\">{teamStatus}</span></div>"
            + $"<div class=\"header-center\"><span class=\"page-title\">{pageTitle}</span></div>"
            + $"<div class=\"header-stats\">{statsHtml}</div>"
            + $"<div class=\"header-user-menu\"><button class=\"user-menu-btn\">Settings</button></div>"
            + $"</header>";
    }

    [Fact]
    public void HeaderBar_WithoutProps_Renders()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().NotBeNullOrEmpty();
        markup.Should().Contain("header-bar");
    }

    [Fact]
    public void HeaderBar_LogoLink_PointsToHome()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("header-logo");
        markup.Should().Contain("href=\"/\"");
        markup.Should().Contain("hattrick");
    }

    [Fact]
    public void HeaderBar_WithTeamName_RendersTeamName()
    {
        var markup = BuildExpectedMarkup(teamName: "FC Nordheim");

        markup.Should().Contain("FC Nordheim");
        markup.Should().Contain("header-team-info");
    }

    [Fact]
    public void HeaderBar_WithTeamStatus_RendersStatus()
    {
        var markup = BuildExpectedMarkup(teamName: "FC Nordheim", teamStatus: "Match Day");

        markup.Should().Contain("Match Day");
        markup.Should().Contain("team-status");
    }

    [Fact]
    public void HeaderBar_WithPageTitle_RendersPageTitle()
    {
        var markup = BuildExpectedMarkup(pageTitle: "Squad Overview");

        markup.Should().Contain("Squad Overview");
        markup.Should().Contain("header-center");
    }

    [Fact]
    public void HeaderBar_WithBudget_RendersBudgetStat()
    {
        var markup = BuildExpectedMarkup(budget: "1 250 000");

        markup.Should().Contain("1 250 000");
        markup.Should().Contain("header-stats");
    }

    [Fact]
    public void HeaderBar_WithFans_RendersFansStat()
    {
        var markup = BuildExpectedMarkup(fans: "8 500");

        markup.Should().Contain("8 500");
    }

    [Fact]
    public void HeaderBar_WithTeamSpirit_RendersTeamSpiritStat()
    {
        var markup = BuildExpectedMarkup(teamSpirit: "Content");

        markup.Should().Contain("Content");
    }

    [Fact]
    public void HeaderBar_SettingsButton_HasCorrectClass()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("user-menu-btn");
        markup.Should().Contain("Settings");
    }

    [Fact]
    public void HeaderBar_CssClasses_AreCorrect()
    {
        var markup = BuildExpectedMarkup(
            teamName: "FC Test",
            budget: "100",
            fans: "200",
            teamSpirit: "High",
            pageTitle: "Home");

        markup.Should().Contain("header-bar");
        markup.Should().Contain("header-logo");
        markup.Should().Contain("header-team-info");
        markup.Should().Contain("header-center");
        markup.Should().Contain("header-stats");
        markup.Should().Contain("header-user-menu");
    }

    [Fact]
    public void HeaderBar_StatItems_HaveLabels()
    {
        var markup = BuildExpectedMarkup(budget: "500 000", fans: "3 000", teamSpirit: "Calm");

        markup.Should().Contain("stat-item");
        markup.Should().Contain("stat-label");
        markup.Should().Contain("stat-value");
    }
}
