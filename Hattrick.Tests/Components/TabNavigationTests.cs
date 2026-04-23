using Bunit;
using FluentAssertions;
using Hattrick.Components.Shared.Lineup;

namespace Hattrick.Tests.Components;

/// <summary>
/// bUnit tests for the TabNavigation.razor component.
///
/// TabNavigation is a 6-tab navigation bar for lineup/match order screens:
/// - Tab 0: Load
/// - Tab 1: Lineup
/// - Tab 2: Team Orders
/// - Tab 3: Penalty Takers
/// - Tab 4: Review
/// - Tab 5: Send Orders
///
/// Props:
/// - CurrentTab (int 0-5): The currently active tab index
/// - OnTabChanged (EventCallback&lt;int&gt;): Callback fired when a tab is clicked
///
/// Expected behavior:
/// - Renders all 6 tabs with correct labels
/// - Active tab (matching CurrentTab) has "active" CSS class
/// - Clicking a tab fires OnTabChanged with that tab's index
/// - Green background styling
/// </summary>
public class TabNavigationTests : BunitContext
{
    // Tab indices as constants for clarity
    private const int TabLoad = 0;
    private const int TabLineup = 1;
    private const int TabTeamOrders = 2;
    private const int TabPenaltyTakers = 3;
    private const int TabReview = 4;
    private const int TabSendOrders = 5;

    // ─────────────────────────────────────────────────────────────────────────
    // Container rendering
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void TabNavigation_Rendered_HasTabNavigationContainer()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad));

        // Assert
        cut.Find(".tab-navigation").Should().NotBeNull();
    }

    [Fact]
    public void TabNavigation_Rendered_HasGreenBackgroundClass()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad));

        // Assert
        cut.Find(".tab-navigation").ClassList.Should().Contain("bg-green");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Tab count and presence
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void TabNavigation_Rendered_HasExactlySixTabs()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs.Should().HaveCount(6);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Tab labels - verify each tab has correct text
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void TabNavigation_Tab0_HasLoadLabel()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabLoad].TextContent.Should().Contain("Load");
    }

    [Fact]
    public void TabNavigation_Tab1_HasLineupLabel()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabLineup].TextContent.Should().Contain("Lineup");
    }

    [Fact]
    public void TabNavigation_Tab2_HasTeamOrdersLabel()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabTeamOrders].TextContent.Should().Contain("Team Orders");
    }

    [Fact]
    public void TabNavigation_Tab3_HasPenaltyTakersLabel()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabPenaltyTakers].TextContent.Should().Contain("Penalty Takers");
    }

    [Fact]
    public void TabNavigation_Tab4_HasReviewLabel()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabReview].TextContent.Should().Contain("Review");
    }

    [Fact]
    public void TabNavigation_Tab5_HasSendOrdersLabel()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabSendOrders].TextContent.Should().Contain("Send Orders");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Active tab - CSS class based on CurrentTab prop
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void TabNavigation_CurrentTab0_LoadTabHasActiveClass()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabLoad].ClassList.Should().Contain("active");
    }

    [Fact]
    public void TabNavigation_CurrentTab1_LineupTabHasActiveClass()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLineup));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabLineup].ClassList.Should().Contain("active");
    }

    [Fact]
    public void TabNavigation_CurrentTab2_TeamOrdersTabHasActiveClass()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabTeamOrders));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabTeamOrders].ClassList.Should().Contain("active");
    }

    [Fact]
    public void TabNavigation_CurrentTab3_PenaltyTakersTabHasActiveClass()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabPenaltyTakers));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabPenaltyTakers].ClassList.Should().Contain("active");
    }

    [Fact]
    public void TabNavigation_CurrentTab4_ReviewTabHasActiveClass()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabReview));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabReview].ClassList.Should().Contain("active");
    }

    [Fact]
    public void TabNavigation_CurrentTab5_SendOrdersTabHasActiveClass()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabSendOrders));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabSendOrders].ClassList.Should().Contain("active");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Non-active tabs should NOT have active class
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void TabNavigation_CurrentTab0_OtherTabsDoNotHaveActiveClass()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabLineup].ClassList.Should().NotContain("active");
        tabs[TabTeamOrders].ClassList.Should().NotContain("active");
        tabs[TabPenaltyTakers].ClassList.Should().NotContain("active");
        tabs[TabReview].ClassList.Should().NotContain("active");
        tabs[TabSendOrders].ClassList.Should().NotContain("active");
    }

    [Fact]
    public void TabNavigation_CurrentTab3_OtherTabsDoNotHaveActiveClass()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabPenaltyTakers));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabLoad].ClassList.Should().NotContain("active");
        tabs[TabLineup].ClassList.Should().NotContain("active");
        tabs[TabTeamOrders].ClassList.Should().NotContain("active");
        tabs[TabReview].ClassList.Should().NotContain("active");
        tabs[TabSendOrders].ClassList.Should().NotContain("active");
    }

    [Fact]
    public void TabNavigation_CurrentTab5_OtherTabsDoNotHaveActiveClass()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabSendOrders));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[TabLoad].ClassList.Should().NotContain("active");
        tabs[TabLineup].ClassList.Should().NotContain("active");
        tabs[TabTeamOrders].ClassList.Should().NotContain("active");
        tabs[TabPenaltyTakers].ClassList.Should().NotContain("active");
        tabs[TabReview].ClassList.Should().NotContain("active");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Click handler - fires OnTabChanged with correct index
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void TabNavigation_ClickTab0_FiresOnTabChangedWith0()
    {
        // Arrange
        int? clickedTabIndex = null;
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLineup)
            .Add(p => p.OnTabChanged, EventCallback.Factory.Create<int>(this, index => clickedTabIndex = index)));

        // Act
        var tabs = cut.FindAll(".tab-item");
        tabs[TabLoad].Click();

        // Assert
        clickedTabIndex.Should().Be(TabLoad);
    }

    [Fact]
    public void TabNavigation_ClickTab1_FiresOnTabChangedWith1()
    {
        // Arrange
        int? clickedTabIndex = null;
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad)
            .Add(p => p.OnTabChanged, EventCallback.Factory.Create<int>(this, index => clickedTabIndex = index)));

        // Act
        var tabs = cut.FindAll(".tab-item");
        tabs[TabLineup].Click();

        // Assert
        clickedTabIndex.Should().Be(TabLineup);
    }

    [Fact]
    public void TabNavigation_ClickTab2_FiresOnTabChangedWith2()
    {
        // Arrange
        int? clickedTabIndex = null;
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad)
            .Add(p => p.OnTabChanged, EventCallback.Factory.Create<int>(this, index => clickedTabIndex = index)));

        // Act
        var tabs = cut.FindAll(".tab-item");
        tabs[TabTeamOrders].Click();

        // Assert
        clickedTabIndex.Should().Be(TabTeamOrders);
    }

    [Fact]
    public void TabNavigation_ClickTab3_FiresOnTabChangedWith3()
    {
        // Arrange
        int? clickedTabIndex = null;
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad)
            .Add(p => p.OnTabChanged, EventCallback.Factory.Create<int>(this, index => clickedTabIndex = index)));

        // Act
        var tabs = cut.FindAll(".tab-item");
        tabs[TabPenaltyTakers].Click();

        // Assert
        clickedTabIndex.Should().Be(TabPenaltyTakers);
    }

    [Fact]
    public void TabNavigation_ClickTab4_FiresOnTabChangedWith4()
    {
        // Arrange
        int? clickedTabIndex = null;
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad)
            .Add(p => p.OnTabChanged, EventCallback.Factory.Create<int>(this, index => clickedTabIndex = index)));

        // Act
        var tabs = cut.FindAll(".tab-item");
        tabs[TabReview].Click();

        // Assert
        clickedTabIndex.Should().Be(TabReview);
    }

    [Fact]
    public void TabNavigation_ClickTab5_FiresOnTabChangedWith5()
    {
        // Arrange
        int? clickedTabIndex = null;
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad)
            .Add(p => p.OnTabChanged, EventCallback.Factory.Create<int>(this, index => clickedTabIndex = index)));

        // Act
        var tabs = cut.FindAll(".tab-item");
        tabs[TabSendOrders].Click();

        // Assert
        clickedTabIndex.Should().Be(TabSendOrders);
    }

    [Fact]
    public void TabNavigation_ClickActiveTab_StillFiresOnTabChanged()
    {
        // Arrange - clicking the already active tab should still fire the callback
        int? clickedTabIndex = null;
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabReview)
            .Add(p => p.OnTabChanged, EventCallback.Factory.Create<int>(this, index => clickedTabIndex = index)));

        // Act
        var tabs = cut.FindAll(".tab-item");
        tabs[TabReview].Click();

        // Assert
        clickedTabIndex.Should().Be(TabReview);
    }

    [Fact]
    public void TabNavigation_ClickMultipleTabs_FiresCallbackEachTime()
    {
        // Arrange
        var clickedIndices = new List<int>();
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad)
            .Add(p => p.OnTabChanged, EventCallback.Factory.Create<int>(this, index => clickedIndices.Add(index))));

        // Act
        var tabs = cut.FindAll(".tab-item");
        tabs[TabLineup].Click();
        tabs[TabPenaltyTakers].Click();
        tabs[TabSendOrders].Click();

        // Assert
        clickedIndices.Should().HaveCount(3);
        clickedIndices.Should().ContainInOrder(TabLineup, TabPenaltyTakers, TabSendOrders);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Theory tests - all tabs with labels and indices
    // ─────────────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0, "Load")]
    [InlineData(1, "Lineup")]
    [InlineData(2, "Team Orders")]
    [InlineData(3, "Penalty Takers")]
    [InlineData(4, "Review")]
    [InlineData(5, "Send Orders")]
    public void TabNavigation_TabAtIndex_HasCorrectLabel(int tabIndex, string expectedLabel)
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[tabIndex].TextContent.Should().Contain(expectedLabel);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void TabNavigation_CurrentTabSetToIndex_ThatTabIsActive(int currentTab)
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, currentTab));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs[currentTab].ClassList.Should().Contain("active");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void TabNavigation_ClickTabAtIndex_FiresOnTabChangedWithCorrectIndex(int tabIndex)
    {
        // Arrange
        int? clickedTabIndex = null;
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad)
            .Add(p => p.OnTabChanged, EventCallback.Factory.Create<int>(this, index => clickedTabIndex = index)));

        // Act
        var tabs = cut.FindAll(".tab-item");
        tabs[tabIndex].Click();

        // Assert
        clickedTabIndex.Should().Be(tabIndex);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Boundary tests - CurrentTab validation
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void TabNavigation_CurrentTab0_IsValidMinimumBoundary()
    {
        // Arrange & Act - Tab 0 is the minimum valid index
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, 0));

        // Assert - Should render without throwing and have first tab active
        var tabs = cut.FindAll(".tab-item");
        tabs[0].ClassList.Should().Contain("active");
    }

    [Fact]
    public void TabNavigation_CurrentTab5_IsValidMaximumBoundary()
    {
        // Arrange & Act - Tab 5 is the maximum valid index
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, 5));

        // Assert - Should render without throwing and have last tab active
        var tabs = cut.FindAll(".tab-item");
        tabs[5].ClassList.Should().Contain("active");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Tab ordering - verify tabs appear in correct order
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void TabNavigation_TabsRenderedInCorrectOrder()
    {
        // Arrange & Act
        var cut = Render<TabNavigation>(parameters => parameters
            .Add(p => p.CurrentTab, TabLoad));

        // Assert - Verify all tabs appear in the correct sequential order
        var tabs = cut.FindAll(".tab-item");
        tabs[0].TextContent.Should().Contain("Load");
        tabs[1].TextContent.Should().Contain("Lineup");
        tabs[2].TextContent.Should().Contain("Team Orders");
        tabs[3].TextContent.Should().Contain("Penalty Takers");
        tabs[4].TextContent.Should().Contain("Review");
        tabs[5].TextContent.Should().Contain("Send Orders");
    }
}
