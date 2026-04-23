using Bunit;
using FluentAssertions;
using Hattrick.Core.Models;
using Hattrick.Components.Shared.Lineup;

namespace Hattrick.Tests.Components;

/// <summary>
/// bUnit tests for the BenchPanel.razor component.
///
/// BenchPanel is a horizontal row below the pitch for displaying substitute players:
/// - Container div with class "bench-panel" and dark green background
/// - Each substitute in a "bench-player" wrapper
/// - Uses PlayerAvatar component with Size=Small
/// - Clicking a bench player fires OnPlayerClick with that MatchLineupSlot
/// - Handles 0-3 bench players (per TeamLineup.MaxSubstituteCount)
///
/// Props:
/// - BenchPlayers (IReadOnlyList&lt;MatchLineupSlot&gt;): Substitute players (IsStarter=false)
/// - OnPlayerClick (EventCallback&lt;MatchLineupSlot&gt;): Callback when a bench player is clicked
///
/// Expected HTML structure:
///   div.bench-panel
///   └── div.bench-player (one per substitute)
///       └── PlayerAvatar (Size=Small)
/// </summary>
public class BenchPanelTests : BunitContext
{
    // Test data constants
    private static readonly Guid SubPlayerId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid SubPlayerId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid SubPlayerId3 = Guid.Parse("33333333-3333-3333-3333-333333333333");

    /// <summary>
    /// Creates a bench with 3 substitutes (maximum allowed).
    /// </summary>
    private static IReadOnlyList<MatchLineupSlot> CreateFullBench()
    {
        return new List<MatchLineupSlot>
        {
            new(SubPlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: false),
            new(SubPlayerId2, Position.CentralDefender, IndividualOrder.Normal, isStarter: false),
            new(SubPlayerId3, Position.Forward, IndividualOrder.Normal, isStarter: false),
        };
    }

    /// <summary>
    /// Creates a bench with 2 substitutes.
    /// </summary>
    private static IReadOnlyList<MatchLineupSlot> CreatePartialBench()
    {
        return new List<MatchLineupSlot>
        {
            new(SubPlayerId1, Position.InnerMidfielder, IndividualOrder.Normal, isStarter: false),
            new(SubPlayerId2, Position.Winger, IndividualOrder.Normal, isStarter: false),
        };
    }

    /// <summary>
    /// Creates a bench with 1 substitute.
    /// </summary>
    private static IReadOnlyList<MatchLineupSlot> CreateMinimalBench()
    {
        return new List<MatchLineupSlot>
        {
            new(SubPlayerId1, Position.WingBack, IndividualOrder.Normal, isStarter: false),
        };
    }

    /// <summary>
    /// Creates an empty bench (no substitutes).
    /// </summary>
    private static IReadOnlyList<MatchLineupSlot> CreateEmptyBench()
    {
        return new List<MatchLineupSlot>();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Container rendering
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void BenchPanel_Rendered_HasBenchPanelContainer()
    {
        // Arrange & Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, CreateFullBench()));

        // Assert
        cut.Find(".bench-panel").Should().NotBeNull();
    }

    [Fact]
    public void BenchPanel_WithEmptyBench_StillRendersBenchPanelContainer()
    {
        // Arrange & Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, CreateEmptyBench()));

        // Assert - Container should exist even with no players
        cut.Find(".bench-panel").Should().NotBeNull();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Bench player count - handles 0-3 players
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void BenchPanel_WithThreeSubstitutes_ShowsThreeBenchPlayers()
    {
        // Arrange & Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, CreateFullBench()));

        // Assert
        var benchPlayers = cut.FindAll(".bench-player");
        benchPlayers.Should().HaveCount(3);
    }

    [Fact]
    public void BenchPanel_WithTwoSubstitutes_ShowsTwoBenchPlayers()
    {
        // Arrange & Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, CreatePartialBench()));

        // Assert
        var benchPlayers = cut.FindAll(".bench-player");
        benchPlayers.Should().HaveCount(2);
    }

    [Fact]
    public void BenchPanel_WithOneSubstitute_ShowsOneBenchPlayer()
    {
        // Arrange & Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, CreateMinimalBench()));

        // Assert
        var benchPlayers = cut.FindAll(".bench-player");
        benchPlayers.Should().HaveCount(1);
    }

    [Fact]
    public void BenchPanel_WithNoSubstitutes_ShowsNoBenchPlayers()
    {
        // Arrange & Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, CreateEmptyBench()));

        // Assert
        var benchPlayers = cut.FindAll(".bench-player");
        benchPlayers.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PlayerAvatar usage with Small size
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void BenchPanel_BenchPlayers_ContainPlayerAvatar()
    {
        // Arrange & Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, CreateFullBench()));

        // Assert - Each bench player should contain a player-avatar
        var avatars = cut.FindAll(".player-avatar");
        avatars.Should().HaveCount(3);
    }

    [Fact]
    public void BenchPanel_PlayerAvatars_HaveSmallSize()
    {
        // Arrange & Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, CreateFullBench()));

        // Assert - All avatars should be small
        var avatars = cut.FindAll(".player-avatar");
        foreach (var avatar in avatars)
        {
            avatar.ClassList.Should().Contain("player-avatar-small");
        }
    }

    [Fact]
    public void BenchPanel_PlayerAvatars_DoNotHaveMediumOrLargeSize()
    {
        // Arrange & Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, CreateFullBench()));

        // Assert - Should NOT have medium or large classes
        var avatars = cut.FindAll(".player-avatar");
        foreach (var avatar in avatars)
        {
            avatar.ClassList.Should().NotContain("player-avatar-medium");
            avatar.ClassList.Should().NotContain("player-avatar-large");
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Click handler - OnPlayerClick
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void BenchPanel_ClickBenchPlayer_FiresOnPlayerClick()
    {
        // Arrange
        MatchLineupSlot? clickedSlot = null;
        var bench = CreateFullBench();

        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, bench)
            .Add(p => p.OnPlayerClick, EventCallback.Factory.Create<MatchLineupSlot>(this, slot => clickedSlot = slot)));

        // Act
        var benchPlayers = cut.FindAll(".bench-player");
        benchPlayers[0].Click();

        // Assert - Verify callback fired with correct slot data
        clickedSlot.Should().NotBeNull();
        clickedSlot!.PlayerId.Should().Be(bench[0].PlayerId);
        clickedSlot.Position.Should().Be(bench[0].Position);
    }

    [Fact]
    public void BenchPanel_ClickBenchPlayer_ReturnsCorrectSlot()
    {
        // Arrange
        MatchLineupSlot? clickedSlot = null;
        var bench = CreateFullBench();

        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, bench)
            .Add(p => p.OnPlayerClick, EventCallback.Factory.Create<MatchLineupSlot>(this, slot => clickedSlot = slot)));

        // Act - Click the first bench player
        var benchPlayers = cut.FindAll(".bench-player");
        benchPlayers[0].Click();

        // Assert - Should return a slot that is NOT a starter (substitute)
        clickedSlot.Should().NotBeNull();
        clickedSlot!.IsStarter.Should().BeFalse();
    }

    [Fact]
    public void BenchPanel_ClickDifferentBenchPlayers_ReturnsCorrectSlotEachTime()
    {
        // Arrange
        var clickedSlots = new List<MatchLineupSlot>();
        var bench = CreateFullBench();

        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, bench)
            .Add(p => p.OnPlayerClick, EventCallback.Factory.Create<MatchLineupSlot>(this, slot => clickedSlots.Add(slot))));

        // Act - Click all three bench players (re-find after each click to avoid stale references)
        cut.FindAll(".bench-player")[0].Click();
        cut.FindAll(".bench-player")[1].Click();
        cut.FindAll(".bench-player")[2].Click();

        // Assert - Should have 3 different clicks, all substitutes
        clickedSlots.Should().HaveCount(3);
        clickedSlots.Should().AllSatisfy(slot => slot.IsStarter.Should().BeFalse());
    }

    [Fact]
    public void BenchPanel_ClickSameBenchPlayerMultipleTimes_FiresCallbackEachTime()
    {
        // Arrange
        var clickCount = 0;
        var bench = CreateMinimalBench();

        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, bench)
            .Add(p => p.OnPlayerClick, EventCallback.Factory.Create<MatchLineupSlot>(this, _ => clickCount++)));

        // Act - Click the same player multiple times
        var benchPlayer = cut.Find(".bench-player");
        benchPlayer.Click();
        benchPlayer.Click();
        benchPlayer.Click();

        // Assert
        clickCount.Should().Be(3);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Structure validation - bench-player wrapper contains PlayerAvatar
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void BenchPanel_Layout_PlayerAvatarInsideBenchPlayerWrapper()
    {
        // Arrange & Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, CreateFullBench()));

        // Assert - Each bench-player wrapper should contain a player-avatar
        var benchPlayers = cut.FindAll(".bench-player");
        foreach (var benchPlayer in benchPlayers)
        {
            benchPlayer.QuerySelector(".player-avatar").Should().NotBeNull();
        }
    }

    [Fact]
    public void BenchPanel_Layout_BenchPlayersInsideContainer()
    {
        // Arrange & Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, CreateFullBench()));

        // Assert - All bench-player elements should be inside bench-panel
        var container = cut.Find(".bench-panel");
        var benchPlayersInContainer = container.QuerySelectorAll(".bench-player");
        benchPlayersInContainer.Should().HaveCount(3);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Position display - PlayerAvatar shows correct position for each sub
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void BenchPanel_WithKeeperSubstitute_ShowsGKPositionBadge()
    {
        // Arrange - Bench with a keeper substitute
        var bench = new List<MatchLineupSlot>
        {
            new(SubPlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: false),
        };

        // Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, bench));

        // Assert
        cut.Find(".position-badge").TextContent.Should().Be("GK");
    }

    [Fact]
    public void BenchPanel_WithForwardSubstitute_ShowsFWPositionBadge()
    {
        // Arrange
        var bench = new List<MatchLineupSlot>
        {
            new(SubPlayerId1, Position.Forward, IndividualOrder.Normal, isStarter: false),
        };

        // Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, bench));

        // Assert
        cut.Find(".position-badge").TextContent.Should().Be("FW");
    }

    [Fact]
    public void BenchPanel_WithMixedPositions_ShowsCorrectBadgesForAll()
    {
        // Arrange - Bench with GK, CD, FW
        var bench = CreateFullBench(); // GK, CD, FW

        // Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, bench));

        // Assert - Should have position badges for all three positions
        var badges = cut.FindAll(".position-badge");
        badges.Should().HaveCount(3);
        badges.Select(b => b.TextContent).Should().Contain("GK");
        badges.Select(b => b.TextContent).Should().Contain("CD");
        badges.Select(b => b.TextContent).Should().Contain("FW");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Theory tests - all valid bench sizes
    // ─────────────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void BenchPanel_VariousBenchSizes_RendersCorrectNumberOfPlayers(int benchSize)
    {
        // Arrange
        var bench = Enumerable.Range(0, benchSize)
            .Select(i => new MatchLineupSlot(
                Guid.NewGuid(),
                Position.CentralDefender,
                IndividualOrder.Normal,
                isStarter: false))
            .ToList();

        // Act
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, bench));

        // Assert
        var benchPlayers = cut.FindAll(".bench-player");
        benchPlayers.Should().HaveCount(benchSize);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // No callback provided - clicking should not throw
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void BenchPanel_ClickWithoutCallback_DoesNotThrow()
    {
        // Arrange
        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, CreateMinimalBench()));

        // Act & Assert - Should not throw when callback is not provided
        var action = () => cut.Find(".bench-player").Click();
        action.Should().NotThrow();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Boundary test - verify slots are NOT starters
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void BenchPanel_AllSlots_AreSubstitutes()
    {
        // Arrange
        var clickedSlots = new List<MatchLineupSlot>();
        var bench = CreateFullBench();

        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, bench)
            .Add(p => p.OnPlayerClick, EventCallback.Factory.Create<MatchLineupSlot>(this, slot => clickedSlots.Add(slot))));

        // Act - Click all bench players (re-find after each click to avoid stale references)
        var benchPlayerCount = cut.FindAll(".bench-player").Count;
        for (var i = 0; i < benchPlayerCount; i++)
        {
            cut.FindAll(".bench-player")[i].Click();
        }

        // Assert - All clicked slots should be substitutes (IsStarter = false)
        clickedSlots.Should().HaveCount(3);
        clickedSlots.Should().AllSatisfy(slot =>
        {
            slot.IsStarter.Should().BeFalse("bench players should not be starters");
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Click propagation - clicking avatar inside wrapper fires callback
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void BenchPanel_ClickPlayerAvatar_FiresOnPlayerClick()
    {
        // Arrange
        MatchLineupSlot? clickedSlot = null;
        var bench = CreateMinimalBench();

        var cut = Render<BenchPanel>(parameters => parameters
            .Add(p => p.BenchPlayers, bench)
            .Add(p => p.OnPlayerClick, EventCallback.Factory.Create<MatchLineupSlot>(this, slot => clickedSlot = slot)));

        // Act - Click directly on the avatar
        cut.Find(".player-avatar").Click();

        // Assert - Should still fire callback
        clickedSlot.Should().NotBeNull();
    }
}
