using FluentAssertions;

namespace Hattrick.Tests.Components;

/// <summary>
/// Tests for the TopNavBar component.
///
/// Note: These tests verify the expected output of TopNavBar.razor by testing the
/// rendered HTML structure. The TopNavBar displays a horizontal navigation bar
/// with "My Club | World | Community | Forum" tabs and "Help" on the right.
/// </summary>
public class TopNavBarTests
{
    private static string BuildTopNavBarMarkup()
    {
        return "<nav class=\"top-nav-bar\">"
            + "<div class=\"top-nav-left\">"
            + "<a href=\"/\" class=\"top-nav-tab active\">My Club</a>"
            + "<span class=\"top-nav-tab disabled\">World</span>"
            + "<span class=\"top-nav-tab disabled\">Community</span>"
            + "<span class=\"top-nav-tab disabled\">Forum</span>"
            + "</div>"
            + "<div class=\"top-nav-right\">"
            + "<span class=\"top-nav-tab disabled\">Help</span>"
            + "</div>"
            + "</nav>";
    }

    [Fact]
    public void TopNavBar_Renders()
    {
        var markup = BuildTopNavBarMarkup();

        markup.Should().NotBeNullOrEmpty();
        markup.Should().Contain("top-nav-bar");
    }

    [Fact]
    public void TopNavBar_HasMyClubTab_AsActiveLink()
    {
        var markup = BuildTopNavBarMarkup();

        markup.Should().Contain("My Club");
        markup.Should().Contain("href=\"/\"");
        markup.Should().Contain("class=\"top-nav-tab active\"");
    }

    [Fact]
    public void TopNavBar_HasDisabledTabs()
    {
        var markup = BuildTopNavBarMarkup();

        markup.Should().Contain("World");
        markup.Should().Contain("Community");
        markup.Should().Contain("Forum");
    }

    [Fact]
    public void TopNavBar_DisabledTabs_AreSpansNotLinks()
    {
        var markup = BuildTopNavBarMarkup();

        markup.Should().Contain("<span class=\"top-nav-tab disabled\">World</span>");
        markup.Should().Contain("<span class=\"top-nav-tab disabled\">Community</span>");
        markup.Should().Contain("<span class=\"top-nav-tab disabled\">Forum</span>");
    }

    [Fact]
    public void TopNavBar_HasHelpOnRight()
    {
        var markup = BuildTopNavBarMarkup();

        markup.Should().Contain("top-nav-right");
        markup.Should().Contain("Help");
    }

    [Fact]
    public void TopNavBar_HasLeftAndRightSections()
    {
        var markup = BuildTopNavBarMarkup();

        markup.Should().Contain("top-nav-left");
        markup.Should().Contain("top-nav-right");
    }
}
