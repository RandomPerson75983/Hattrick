using Bunit;
using FluentAssertions;
using Hattrick.Core.Models;
using Hattrick.Core.Services;
using Hattrick.Components.Shared.Lineup;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Hattrick.Tests.Components;

/// <summary>
/// bUnit tests for the LineupPageContent.razor component.
///
/// LineupPageContent is the main layout component for lineup management:
/// - Container div with class "lineup-page"
/// - Contains TabNavigation for workflow steps
/// - Contains FormationPitch for starter positions
/// - Contains BenchPanel for substitute players
/// - State: currentTab (0-5) for tab navigation
/// - Data: TeamLineup from ILineupPageService.GetLineupForTeam()
///
/// Props:
/// - TeamId (Guid): The team whose lineup to display
///
/// The component has ZERO business logic - all data from ILineupPageService.
///
/// Expected HTML structure:
///   div.lineup-page
///   ├── TabNavigation (CurrentTab bound to state)
///   ├── FormationPitch (Formation + starter Slots from lineup)
///   └── BenchPanel (bench players filtered from lineup Slots)
/// </summary>
public class LineupPageTests : BunitContext
{
    // Tab index constants (mirrors TabNavigation)
    private const int TabLoad = 0;
    private const int TabLineup = 1;
    private const int TabTeamOrders = 2;
    private const int TabPenaltyTakers = 3;
    private const int TabReview = 4;
    private const int TabSendOrders = 5;

    // Test data
    private static readonly Guid TestTeamId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid PlayerId1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid PlayerId2 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid PlayerId3 = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    private static readonly Guid PlayerId4 = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    private static readonly Guid PlayerId5 = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
    private static readonly Guid PlayerId6 = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
    private static readonly Guid PlayerId7 = Guid.Parse("00000000-1111-2222-3333-444444444444");
    private static readonly Guid PlayerId8 = Guid.Parse("00000000-2222-3333-4444-555555555555");
    private static readonly Guid PlayerId9 = Guid.Parse("00000000-3333-4444-5555-666666666666");
    private static readonly Guid PlayerId10 = Guid.Parse("00000000-4444-5555-6666-777777777777");
    private static readonly Guid PlayerId11 = Guid.Parse("00000000-5555-6666-7777-888888888888");
    private static readonly Guid SubPlayerId1 = Guid.Parse("11110000-0000-0000-0000-000000000001");
    private static readonly Guid SubPlayerId2 = Guid.Parse("11110000-0000-0000-0000-000000000002");
    private static readonly Guid SubPlayerId3 = Guid.Parse("11110000-0000-0000-0000-000000000003");

    /// <summary>
    /// Creates a standard 4-4-2 lineup with 11 starters and 3 substitutes.
    /// </summary>
    private static TeamLineup CreateTestLineup()
    {
        return new TeamLineup
        {
            TeamId = TestTeamId,
            Formation = Formation.Formation442,
            Tactic = Tactic.Normal,
            Attitude = TeamAttitude.PlayItCool,
            Slots = new List<MatchLineupSlot>
            {
                // 11 Starters
                new(PlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: true),
                new(PlayerId2, Position.WingBack, IndividualOrder.Normal, isStarter: true),
                new(PlayerId3, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
                new(PlayerId4, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
                new(PlayerId5, Position.WingBack, IndividualOrder.Normal, isStarter: true),
                new(PlayerId6, Position.Winger, IndividualOrder.Normal, isStarter: true),
                new(PlayerId7, Position.InnerMidfielder, IndividualOrder.Normal, isStarter: true),
                new(PlayerId8, Position.InnerMidfielder, IndividualOrder.Normal, isStarter: true),
                new(PlayerId9, Position.Winger, IndividualOrder.Normal, isStarter: true),
                new(PlayerId10, Position.Forward, IndividualOrder.Normal, isStarter: true),
                new(PlayerId11, Position.Forward, IndividualOrder.Normal, isStarter: true),
                // 3 Substitutes
                new(SubPlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: false),
                new(SubPlayerId2, Position.CentralDefender, IndividualOrder.Normal, isStarter: false),
                new(SubPlayerId3, Position.Forward, IndividualOrder.Normal, isStarter: false),
            }
        };
    }

    /// <summary>
    /// Creates an empty lineup (no slots).
    /// </summary>
    private static TeamLineup CreateEmptyLineup()
    {
        return new TeamLineup
        {
            TeamId = TestTeamId,
            Formation = Formation.Formation442,
            Slots = new List<MatchLineupSlot>()
        };
    }

    /// <summary>
    /// Sets up mock ILineupPageService returning the specified lineup.
    /// </summary>
    private ILineupPageService SetupMockService(TeamLineup lineup)
    {
        var mockService = Substitute.For<ILineupPageService>();
        mockService.GetLineupForTeam(Arg.Any<Guid>()).Returns(lineup);
        mockService.GetStarters(Arg.Any<Guid>()).Returns(lineup.Slots.Where(s => s.IsStarter).ToList());
        mockService.GetBenchPlayers(Arg.Any<Guid>()).Returns(lineup.Slots.Where(s => !s.IsStarter).ToList());
        Services.AddSingleton(mockService);
        return mockService;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Page container
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void LineupPageContent_Rendered_HasLineupPageContainer()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        cut.Find(".lineup-page").Should().NotBeNull();
    }

    [Fact]
    public void LineupPageContent_Container_IsDiv()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var container = cut.Find(".lineup-page");
        container.TagName.Should().Be("DIV");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // TabNavigation component presence
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void LineupPageContent_Rendered_ContainsTabNavigation()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        cut.Find(".tab-navigation").Should().NotBeNull();
    }

    [Fact]
    public void LineupPageContent_TabNavigation_HasGreenBackground()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var tabNav = cut.Find(".tab-navigation");
        tabNav.ClassList.Should().Contain("bg-green");
    }

    [Fact]
    public void LineupPageContent_TabNavigation_HasSixTabs()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var tabItems = cut.FindAll(".tab-item");
        tabItems.Should().HaveCount(6);
    }

    [Fact]
    public void LineupPageContent_TabNavigation_HasLoadTab()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs.Select(t => t.TextContent).Should().Contain("Load");
    }

    [Fact]
    public void LineupPageContent_TabNavigation_HasLineupTab()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs.Select(t => t.TextContent).Should().Contain("Lineup");
    }

    [Fact]
    public void LineupPageContent_TabNavigation_HasTeamOrdersTab()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs.Select(t => t.TextContent).Should().Contain("Team Orders");
    }

    [Fact]
    public void LineupPageContent_TabNavigation_HasPenaltyTakersTab()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs.Select(t => t.TextContent).Should().Contain("Penalty Takers");
    }

    [Fact]
    public void LineupPageContent_TabNavigation_HasReviewTab()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs.Select(t => t.TextContent).Should().Contain("Review");
    }

    [Fact]
    public void LineupPageContent_TabNavigation_HasSendOrdersTab()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var tabs = cut.FindAll(".tab-item");
        tabs.Select(t => t.TextContent).Should().Contain("Send Orders");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // FormationPitch component presence
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void LineupPageContent_Rendered_ContainsFormationPitch()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        cut.Find(".formation-pitch").Should().NotBeNull();
    }

    [Fact]
    public void LineupPageContent_FormationPitch_IsSvgElement()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var pitch = cut.Find(".formation-pitch");
        pitch.TagName.Should().Be("svg");
    }

    [Fact]
    public void LineupPageContent_FormationPitch_HasFormationLabel()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        cut.Find(".formation-label").Should().NotBeNull();
    }

    [Fact]
    public void LineupPageContent_FormationPitch_ShowsCorrectFormation()
    {
        // Arrange
        SetupMockService(CreateTestLineup()); // Uses 4-4-2

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        cut.Find(".formation-label").TextContent.Should().Contain("4-4-2");
    }

    [Fact]
    public void LineupPageContent_FormationPitch_HasPlayerPositions()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        cut.Find(".player-positions").Should().NotBeNull();
    }

    [Fact]
    public void LineupPageContent_FormationPitch_Shows11Starters()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var playerSlots = cut.FindAll(".player-slot");
        playerSlots.Should().HaveCount(11);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BenchPanel component presence
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void LineupPageContent_Rendered_ContainsBenchPanel()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        cut.Find(".bench-panel").Should().NotBeNull();
    }

    [Fact]
    public void LineupPageContent_BenchPanel_ShowsSubstitutes()
    {
        // Arrange
        SetupMockService(CreateTestLineup()); // Has 3 subs

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var benchPlayers = cut.FindAll(".bench-player");
        benchPlayers.Should().HaveCount(3);
    }

    [Fact]
    public void LineupPageContent_BenchPanel_HasSmallAvatars()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var avatars = cut.FindAll(".bench-panel .player-avatar");
        avatars.Should().AllSatisfy(a => a.ClassList.Should().Contain("player-avatar-small"));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Layout order - components in correct sequence
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void LineupPageContent_Layout_TabNavigationBeforeFormationPitch()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var markup = cut.Markup;
        var tabNavIndex = markup.IndexOf("tab-navigation", StringComparison.Ordinal);
        var pitchIndex = markup.IndexOf("formation-pitch", StringComparison.Ordinal);
        tabNavIndex.Should().BeLessThan(pitchIndex);
    }

    [Fact]
    public void LineupPageContent_Layout_FormationPitchBeforeBenchPanel()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var markup = cut.Markup;
        var pitchIndex = markup.IndexOf("formation-pitch", StringComparison.Ordinal);
        var benchIndex = markup.IndexOf("bench-panel", StringComparison.Ordinal);
        pitchIndex.Should().BeLessThan(benchIndex);
    }

    [Fact]
    public void LineupPageContent_Layout_AllComponentsInsideContainer()
    {
        // Arrange
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert - all components should be inside .lineup-page container
        var container = cut.Find(".lineup-page");
        container.QuerySelector(".tab-navigation").Should().NotBeNull();
        container.QuerySelector(".formation-pitch").Should().NotBeNull();
        container.QuerySelector(".bench-panel").Should().NotBeNull();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CurrentTab state - default and active tab display
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void LineupPageContent_InitialState_LineupTabIsActive()
    {
        // Arrange - Default tab should be Lineup (index 1)
        SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        var activeTabs = cut.FindAll(".tab-item.active");
        activeTabs.Should().HaveCount(1);
        activeTabs[0].TextContent.Should().Be("Lineup");
    }

    [Fact]
    public void LineupPageContent_ClickLoadTab_LoadTabBecomesActive()
    {
        // Arrange
        SetupMockService(CreateTestLineup());
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Act - Click the Load tab
        var tabs = cut.FindAll(".tab-item");
        tabs[TabLoad].Click();

        // Assert
        var activeTabs = cut.FindAll(".tab-item.active");
        activeTabs.Should().HaveCount(1);
        activeTabs[0].TextContent.Should().Be("Load");
    }

    [Fact]
    public void LineupPageContent_ClickTeamOrdersTab_TeamOrdersTabBecomesActive()
    {
        // Arrange
        SetupMockService(CreateTestLineup());
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Act - Click Team Orders tab
        var tabs = cut.FindAll(".tab-item");
        tabs[TabTeamOrders].Click();

        // Assert
        var activeTabs = cut.FindAll(".tab-item.active");
        activeTabs.Should().HaveCount(1);
        activeTabs[0].TextContent.Should().Be("Team Orders");
    }

    [Fact]
    public void LineupPageContent_ClickPenaltyTakersTab_PenaltyTakersTabBecomesActive()
    {
        // Arrange
        SetupMockService(CreateTestLineup());
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Act
        var tabs = cut.FindAll(".tab-item");
        tabs[TabPenaltyTakers].Click();

        // Assert
        var activeTabs = cut.FindAll(".tab-item.active");
        activeTabs.Should().HaveCount(1);
        activeTabs[0].TextContent.Should().Be("Penalty Takers");
    }

    [Fact]
    public void LineupPageContent_ClickReviewTab_ReviewTabBecomesActive()
    {
        // Arrange
        SetupMockService(CreateTestLineup());
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Act
        var tabs = cut.FindAll(".tab-item");
        tabs[TabReview].Click();

        // Assert
        var activeTabs = cut.FindAll(".tab-item.active");
        activeTabs.Should().HaveCount(1);
        activeTabs[0].TextContent.Should().Be("Review");
    }

    [Fact]
    public void LineupPageContent_ClickSendOrdersTab_SendOrdersTabBecomesActive()
    {
        // Arrange
        SetupMockService(CreateTestLineup());
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Act
        var tabs = cut.FindAll(".tab-item");
        tabs[TabSendOrders].Click();

        // Assert
        var activeTabs = cut.FindAll(".tab-item.active");
        activeTabs.Should().HaveCount(1);
        activeTabs[0].TextContent.Should().Be("Send Orders");
    }

    [Fact]
    public void LineupPageContent_TabSwitch_OnlyOneTabIsActive()
    {
        // Arrange
        SetupMockService(CreateTestLineup());
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Act - Click several tabs (re-fetch after each click due to DOM re-render)
        cut.FindAll(".tab-item")[TabLoad].Click();
        cut.FindAll(".tab-item")[TabPenaltyTakers].Click();
        cut.FindAll(".tab-item")[TabReview].Click();

        // Assert - Still only one active
        var activeTabs = cut.FindAll(".tab-item.active");
        activeTabs.Should().HaveCount(1);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Data loading from service
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void LineupPageContent_OnInit_CallsServiceWithTeamId()
    {
        // Arrange
        var mockService = SetupMockService(CreateTestLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        mockService.Received(1).GetLineupForTeam(TestTeamId);
    }

    [Fact]
    public void LineupPageContent_EmptyLineup_StillRendersAllContainers()
    {
        // Arrange
        SetupMockService(CreateEmptyLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert
        cut.Find(".lineup-page").Should().NotBeNull();
        cut.Find(".tab-navigation").Should().NotBeNull();
        cut.Find(".formation-pitch").Should().NotBeNull();
        cut.Find(".bench-panel").Should().NotBeNull();
    }

    [Fact]
    public void LineupPageContent_EmptyLineup_ShowsNoPlayerSlots()
    {
        // Arrange
        SetupMockService(CreateEmptyLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert - First verify structure exists, then check empty
        cut.Find(".formation-pitch").Should().NotBeNull();
        var playerSlots = cut.FindAll(".player-slot");
        playerSlots.Should().BeEmpty();
    }

    [Fact]
    public void LineupPageContent_EmptyLineup_ShowsNoBenchPlayers()
    {
        // Arrange
        SetupMockService(CreateEmptyLineup());

        // Act
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Assert - First verify structure exists, then check empty
        cut.Find(".bench-panel").Should().NotBeNull();
        var benchPlayers = cut.FindAll(".bench-player");
        benchPlayers.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Tab boundary tests
    // ─────────────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0, "Load")]
    [InlineData(1, "Lineup")]
    [InlineData(2, "Team Orders")]
    [InlineData(3, "Penalty Takers")]
    [InlineData(4, "Review")]
    [InlineData(5, "Send Orders")]
    public void LineupPageContent_ClickTab_ActivatesCorrectTab(int tabIndex, string expectedTabName)
    {
        // Arrange
        SetupMockService(CreateTestLineup());
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, TestTeamId));

        // Act
        var tabs = cut.FindAll(".tab-item");
        tabs[tabIndex].Click();

        // Assert
        var activeTabs = cut.FindAll(".tab-item.active");
        activeTabs.Should().HaveCount(1);
        activeTabs[0].TextContent.Should().Be(expectedTabName);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BUG: OnInitialized vs OnParametersSet - TeamId parameter changes
    // Issue: Component uses OnInitialized() which only runs once.
    // When TeamId parameter changes, data should reload but currently shows stale data.
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task LineupPageContent_WhenTeamIdChanges_ReloadsDataForNewTeam()
    {
        // Arrange - Create two different teams with different lineups
        var teamId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var teamId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var lineup1 = new TeamLineup
        {
            TeamId = teamId1,
            Formation = Formation.Formation442,
            Slots = new List<MatchLineupSlot>
            {
                new(PlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: true),
                new(PlayerId2, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
            }
        };

        var lineup2 = new TeamLineup
        {
            TeamId = teamId2,
            Formation = Formation.Formation433,
            Slots = new List<MatchLineupSlot>
            {
                new(PlayerId3, Position.Keeper, IndividualOrder.Normal, isStarter: true),
                new(PlayerId4, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
                new(PlayerId5, Position.Forward, IndividualOrder.Normal, isStarter: true),
            }
        };

        var mockService = Substitute.For<ILineupPageService>();
        mockService.GetLineupForTeam(teamId1).Returns(lineup1);
        mockService.GetLineupForTeam(teamId2).Returns(lineup2);
        mockService.GetStarters(teamId1).Returns(lineup1.Slots.Where(s => s.IsStarter).ToList());
        mockService.GetStarters(teamId2).Returns(lineup2.Slots.Where(s => s.IsStarter).ToList());
        mockService.GetBenchPlayers(teamId1).Returns(lineup1.Slots.Where(s => !s.IsStarter).ToList());
        mockService.GetBenchPlayers(teamId2).Returns(lineup2.Slots.Where(s => !s.IsStarter).ToList());
        Services.AddSingleton(mockService);

        // Act - Render with first team
        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, teamId1));

        // Verify initial state shows team1's formation (4-4-2)
        cut.Find(".formation-label").TextContent.Should().Contain("4-4-2");

        // Now change TeamId parameter to team2
        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                { nameof(LineupPageContent.TeamId), teamId2 }
            })));

        // Assert - Should now show team2's formation (4-3-3)
        // BUG: Currently this fails because OnInitialized doesn't re-run on parameter changes
        cut.Find(".formation-label").TextContent.Should().Contain("4-3-3");
    }

    [Fact]
    public async Task LineupPageContent_WhenTeamIdChanges_CallsServiceWithNewTeamId()
    {
        // Arrange
        var teamId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var teamId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var testLineup = CreateTestLineup();
        var mockService = Substitute.For<ILineupPageService>();
        mockService.GetLineupForTeam(Arg.Any<Guid>()).Returns(testLineup);
        mockService.GetStarters(Arg.Any<Guid>()).Returns(testLineup.Slots.Where(s => s.IsStarter).ToList());
        mockService.GetBenchPlayers(Arg.Any<Guid>()).Returns(testLineup.Slots.Where(s => !s.IsStarter).ToList());
        Services.AddSingleton(mockService);

        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, teamId1));

        // Clear received calls to track only the next call
        mockService.ClearReceivedCalls();

        // Act - Change TeamId parameter
        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                { nameof(LineupPageContent.TeamId), teamId2 }
            })));

        // Assert - Service should be called with the new team ID
        // BUG: Currently fails because OnInitialized doesn't re-run
        mockService.Received(1).GetLineupForTeam(teamId2);
    }

    [Fact]
    public async Task LineupPageContent_WhenTeamIdChanges_UpdatesStartersList()
    {
        // Arrange - Team1 has 2 starters, Team2 has 4 starters
        var teamId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var teamId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var lineup1 = new TeamLineup
        {
            TeamId = teamId1,
            Formation = Formation.Formation442,
            Slots = new List<MatchLineupSlot>
            {
                new(PlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: true),
                new(PlayerId2, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
            }
        };

        var lineup2 = new TeamLineup
        {
            TeamId = teamId2,
            Formation = Formation.Formation433,
            Slots = new List<MatchLineupSlot>
            {
                new(PlayerId3, Position.Keeper, IndividualOrder.Normal, isStarter: true),
                new(PlayerId4, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
                new(PlayerId5, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
                new(PlayerId6, Position.Forward, IndividualOrder.Normal, isStarter: true),
            }
        };

        var mockService = Substitute.For<ILineupPageService>();
        mockService.GetLineupForTeam(teamId1).Returns(lineup1);
        mockService.GetLineupForTeam(teamId2).Returns(lineup2);
        mockService.GetStarters(teamId1).Returns(lineup1.Slots.Where(s => s.IsStarter).ToList());
        mockService.GetStarters(teamId2).Returns(lineup2.Slots.Where(s => s.IsStarter).ToList());
        mockService.GetBenchPlayers(teamId1).Returns(lineup1.Slots.Where(s => !s.IsStarter).ToList());
        mockService.GetBenchPlayers(teamId2).Returns(lineup2.Slots.Where(s => !s.IsStarter).ToList());
        Services.AddSingleton(mockService);

        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, teamId1));

        // Initial state: 2 player slots
        var initialSlots = cut.FindAll(".player-slot");
        initialSlots.Should().HaveCount(2);

        // Act - Change to team2
        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                { nameof(LineupPageContent.TeamId), teamId2 }
            })));

        // Assert - Should now have 4 player slots
        // BUG: Currently fails because starters list isn't updated
        var updatedSlots = cut.FindAll(".player-slot");
        updatedSlots.Should().HaveCount(4);
    }

    [Fact]
    public async Task LineupPageContent_WhenTeamIdChanges_UpdatesBenchPlayersList()
    {
        // Arrange - Team1 has 1 bench player, Team2 has 3 bench players
        var teamId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var teamId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var lineup1 = new TeamLineup
        {
            TeamId = teamId1,
            Formation = Formation.Formation442,
            Slots = new List<MatchLineupSlot>
            {
                new(PlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: true),
                new(SubPlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: false),
            }
        };

        var lineup2 = new TeamLineup
        {
            TeamId = teamId2,
            Formation = Formation.Formation433,
            Slots = new List<MatchLineupSlot>
            {
                new(PlayerId3, Position.Keeper, IndividualOrder.Normal, isStarter: true),
                new(SubPlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: false),
                new(SubPlayerId2, Position.CentralDefender, IndividualOrder.Normal, isStarter: false),
                new(SubPlayerId3, Position.Forward, IndividualOrder.Normal, isStarter: false),
            }
        };

        // Pre-compute lists to avoid LINQ evaluation issues
        var starters1 = lineup1.Slots.Where(s => s.IsStarter).ToList();
        var starters2 = lineup2.Slots.Where(s => s.IsStarter).ToList();
        var bench1 = lineup1.Slots.Where(s => !s.IsStarter).ToList();
        var bench2 = lineup2.Slots.Where(s => !s.IsStarter).ToList();

        var mockService = Substitute.For<ILineupPageService>();
        mockService.GetLineupForTeam(teamId1).Returns(lineup1);
        mockService.GetLineupForTeam(teamId2).Returns(lineup2);
        mockService.GetStarters(teamId1).Returns(starters1);
        mockService.GetStarters(teamId2).Returns(starters2);
        mockService.GetBenchPlayers(teamId1).Returns(bench1);
        mockService.GetBenchPlayers(teamId2).Returns(bench2);
        Services.AddSingleton(mockService);

        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, teamId1));

        // Initial state: 1 bench player
        var initialBench = cut.FindAll(".bench-player");
        initialBench.Should().HaveCount(1);

        // Act - Change to team2
        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                { nameof(LineupPageContent.TeamId), teamId2 }
            })));

        // Assert - Should now have 3 bench players
        var updatedBench = cut.FindAll(".bench-player");
        updatedBench.Should().HaveCount(3);
    }

    [Fact]
    public async Task LineupPageContent_WhenTeamIdChangesToSameValue_DoesNotReloadData()
    {
        // Arrange - Optimization: same TeamId shouldn't trigger unnecessary reload
        var teamId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var testLineup = CreateTestLineup();
        var mockService = Substitute.For<ILineupPageService>();
        mockService.GetLineupForTeam(teamId).Returns(testLineup);
        mockService.GetStarters(teamId).Returns(testLineup.Slots.Where(s => s.IsStarter).ToList());
        mockService.GetBenchPlayers(teamId).Returns(testLineup.Slots.Where(s => !s.IsStarter).ToList());
        Services.AddSingleton(mockService);

        var cut = Render<LineupPageContent>(parameters => parameters
            .Add(p => p.TeamId, teamId));

        // Initial call count should be 1
        mockService.Received(1).GetLineupForTeam(teamId);
        mockService.ClearReceivedCalls();

        // Act - Set same TeamId again
        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                { nameof(LineupPageContent.TeamId), teamId }
            })));

        // Assert - Should NOT call service again for same team
        // This tests for proper parameter change detection
        mockService.DidNotReceive().GetLineupForTeam(Arg.Any<Guid>());
    }
}
