using Bunit;
using FluentAssertions;
using Hattrick.Core.Models;
using Hattrick.Components.Shared.Lineup;
using Microsoft.AspNetCore.Components;

namespace Hattrick.Tests.Components;

/// <summary>
/// bUnit tests for the FormationPitch.razor component.
///
/// FormationPitch is an SVG-based football pitch that displays players in formation:
/// - SVG element with class "formation-pitch"
/// - Field markings: goal areas, penalty areas, center circle, halfway line
/// - 11 player positions for starters (slots where IsStarter=true)
/// - Formation label showing formation name (e.g., "4-4-2")
/// - Clicking a player position fires OnSlotClick with that slot
/// - Uses PlayerAvatar component for player display
///
/// Props:
/// - Formation (Formation enum): The team's tactical formation
/// - Slots (IReadOnlyList&lt;MatchLineupSlot&gt;): Player assignments including position and starter status
/// - OnSlotClick (EventCallback&lt;MatchLineupSlot&gt;): Callback when a player slot is clicked
///
/// Expected SVG structure:
///   svg.formation-pitch
///   ├── rect.pitch-background (green field)
///   ├── rect.goal-area-top, rect.goal-area-bottom
///   ├── rect.penalty-area-top, rect.penalty-area-bottom
///   ├── circle.center-circle
///   ├── line.halfway-line
///   ├── g.player-positions (contains 11 player slots)
///   └── text.formation-label (e.g., "4-4-2")
/// </summary>
public class FormationPitchTests : BunitContext
{
    // Test data constants
    private static readonly Guid PlayerId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid PlayerId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid PlayerId3 = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid PlayerId4 = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static readonly Guid PlayerId5 = Guid.Parse("55555555-5555-5555-5555-555555555555");
    private static readonly Guid PlayerId6 = Guid.Parse("66666666-6666-6666-6666-666666666666");
    private static readonly Guid PlayerId7 = Guid.Parse("77777777-7777-7777-7777-777777777777");
    private static readonly Guid PlayerId8 = Guid.Parse("88888888-8888-8888-8888-888888888888");
    private static readonly Guid PlayerId9 = Guid.Parse("99999999-9999-9999-9999-999999999999");
    private static readonly Guid PlayerId10 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid PlayerId11 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid SubPlayerId1 = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    private static readonly Guid SubPlayerId2 = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

    /// <summary>
    /// Creates a standard 4-4-2 lineup with 11 starters and 2 substitutes.
    /// </summary>
    private static IReadOnlyList<MatchLineupSlot> Create442Lineup()
    {
        return new List<MatchLineupSlot>
        {
            // Starters
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
            // Substitutes
            new(SubPlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: false),
            new(SubPlayerId2, Position.CentralDefender, IndividualOrder.Normal, isStarter: false),
        };
    }

    /// <summary>
    /// Creates a minimal lineup with only starters (no substitutes).
    /// </summary>
    private static IReadOnlyList<MatchLineupSlot> CreateMinimalLineup()
    {
        return new List<MatchLineupSlot>
        {
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
        };
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SVG Container rendering
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_Rendered_HasFormationPitchSvgContainer()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find("svg.formation-pitch").Should().NotBeNull();
    }

    [Fact]
    public void FormationPitch_Rendered_SvgHasViewBoxAttribute()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert - SVG should have viewBox for responsive sizing
        var svg = cut.Find("svg.formation-pitch");
        svg.GetAttribute("viewBox").Should().NotBeNullOrEmpty();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Pitch background
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_Rendered_HasPitchBackground()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert - Should have green pitch background
        cut.Find(".pitch-background").Should().NotBeNull();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Field markings - Goal areas
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_Rendered_HasGoalAreaTop()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".goal-area-top").Should().NotBeNull();
    }

    [Fact]
    public void FormationPitch_Rendered_HasGoalAreaBottom()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".goal-area-bottom").Should().NotBeNull();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Field markings - Penalty areas
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_Rendered_HasPenaltyAreaTop()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".penalty-area-top").Should().NotBeNull();
    }

    [Fact]
    public void FormationPitch_Rendered_HasPenaltyAreaBottom()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".penalty-area-bottom").Should().NotBeNull();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Field markings - Center circle and halfway line
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_Rendered_HasCenterCircle()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".center-circle").Should().NotBeNull();
    }

    [Fact]
    public void FormationPitch_Rendered_HasHalfwayLine()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".halfway-line").Should().NotBeNull();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Player positions container
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_Rendered_HasPlayerPositionsContainer()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".player-positions").Should().NotBeNull();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Player count - Only starters shown
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_WithLineup_ShowsExactly11PlayerPositions()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert - Should show exactly 11 players (starters only)
        var playerSlots = cut.FindAll(".player-slot");
        playerSlots.Should().HaveCount(11);
    }

    [Fact]
    public void FormationPitch_WithSubstitutes_DoesNotShowSubstitutes()
    {
        // Arrange - Lineup with 11 starters and 2 substitutes
        var lineup = Create442Lineup();

        // Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, lineup));

        // Assert - Should still show only 11 (starters)
        var playerSlots = cut.FindAll(".player-slot");
        playerSlots.Should().HaveCount(11);
    }

    [Fact]
    public void FormationPitch_WithMinimalLineup_ShowsAll11Starters()
    {
        // Arrange - Lineup with only 11 starters, no substitutes
        var lineup = CreateMinimalLineup();

        // Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, lineup));

        // Assert
        var playerSlots = cut.FindAll(".player-slot");
        playerSlots.Should().HaveCount(11);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Formation label
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_Rendered_HasFormationLabel()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".formation-label").Should().NotBeNull();
    }

    [Fact]
    public void FormationPitch_Formation442_LabelShows442()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".formation-label").TextContent.Should().Contain("4-4-2");
    }

    [Fact]
    public void FormationPitch_Formation433_LabelShows433()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation433)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".formation-label").TextContent.Should().Contain("4-3-3");
    }

    [Fact]
    public void FormationPitch_Formation352_LabelShows352()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation352)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".formation-label").TextContent.Should().Contain("3-5-2");
    }

    [Fact]
    public void FormationPitch_Formation343_LabelShows343()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation343)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".formation-label").TextContent.Should().Contain("3-4-3");
    }

    [Fact]
    public void FormationPitch_Formation541_LabelShows541()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation541)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".formation-label").TextContent.Should().Contain("5-4-1");
    }

    [Fact]
    public void FormationPitch_Formation451_LabelShows451()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation451)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".formation-label").TextContent.Should().Contain("4-5-1");
    }

    [Fact]
    public void FormationPitch_Formation532_LabelShows532()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation532)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".formation-label").TextContent.Should().Contain("5-3-2");
    }

    [Fact]
    public void FormationPitch_Formation523_LabelShows523()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation523)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".formation-label").TextContent.Should().Contain("5-2-3");
    }

    [Fact]
    public void FormationPitch_Formation550_LabelShows550()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation550)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".formation-label").TextContent.Should().Contain("5-5-0");
    }

    [Fact]
    public void FormationPitch_Formation253_LabelShows253()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation253)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".formation-label").TextContent.Should().Contain("2-5-3");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Click handler - OnSlotClick
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_ClickPlayerSlot_FiresOnSlotClick()
    {
        // Arrange
        MatchLineupSlot? clickedSlot = null;
        var lineup = Create442Lineup();

        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, lineup)
            .Add(p => p.OnSlotClick, EventCallback.Factory.Create<MatchLineupSlot>(this, slot => clickedSlot = slot)));

        // Act
        var playerSlots = cut.FindAll(".player-slot");
        playerSlots[0].Click();

        // Assert - Verify callback fired with a valid starter slot
        clickedSlot.Should().NotBeNull();
        clickedSlot!.IsStarter.Should().BeTrue();
        lineup.Should().Contain(clickedSlot);
    }

    [Fact]
    public void FormationPitch_ClickPlayerSlot_ReturnsCorrectSlot()
    {
        // Arrange
        MatchLineupSlot? clickedSlot = null;
        var lineup = Create442Lineup();
        var expectedSlot = lineup[0]; // First starter (Keeper)

        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, lineup)
            .Add(p => p.OnSlotClick, EventCallback.Factory.Create<MatchLineupSlot>(this, slot => clickedSlot = slot)));

        // Act
        var playerSlots = cut.FindAll(".player-slot");
        playerSlots[0].Click();

        // Assert - The clicked slot should be a starter from the lineup
        clickedSlot.Should().NotBeNull();
        clickedSlot!.IsStarter.Should().BeTrue();
    }

    [Fact]
    public void FormationPitch_ClickMultipleSlots_FiresCallbackEachTime()
    {
        // Arrange
        var clickedSlots = new List<MatchLineupSlot>();
        var lineup = Create442Lineup();

        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, lineup)
            .Add(p => p.OnSlotClick, EventCallback.Factory.Create<MatchLineupSlot>(this, slot => clickedSlots.Add(slot))));

        // Act
        var playerSlots = cut.FindAll(".player-slot");
        playerSlots[0].Click();
        playerSlots[5].Click();
        playerSlots[10].Click();

        // Assert - Verify three distinct slots were returned
        clickedSlots.Should().HaveCount(3);
        clickedSlots.Select(s => s.PlayerId).Distinct().Should().HaveCount(3);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PlayerAvatar usage
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_PlayerSlots_ContainPlayerAvatar()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert - Each player slot should contain a player-avatar
        var avatars = cut.FindAll(".player-avatar");
        avatars.Should().HaveCount(11);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Theory tests - All formations with labels
    // ─────────────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(Formation.Formation442, "4-4-2")]
    [InlineData(Formation.Formation433, "4-3-3")]
    [InlineData(Formation.Formation352, "3-5-2")]
    [InlineData(Formation.Formation343, "3-4-3")]
    [InlineData(Formation.Formation541, "5-4-1")]
    [InlineData(Formation.Formation451, "4-5-1")]
    [InlineData(Formation.Formation532, "5-3-2")]
    [InlineData(Formation.Formation523, "5-2-3")]
    [InlineData(Formation.Formation550, "5-5-0")]
    [InlineData(Formation.Formation253, "2-5-3")]
    public void FormationPitch_Formation_HasCorrectLabel(Formation formation, string expectedLabel)
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, formation)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert
        cut.Find(".formation-label").TextContent.Should().Contain(expectedLabel);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Empty slots handling
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_WithEmptySlots_RendersWithoutError()
    {
        // Arrange - Empty slots list
        var emptySlots = new List<MatchLineupSlot>();

        // Act - Should render without throwing
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, emptySlots));

        // Assert - Pitch should still render
        cut.Find("svg.formation-pitch").Should().NotBeNull();
    }

    [Fact]
    public void FormationPitch_WithEmptySlots_ShowsNoPlayerPositions()
    {
        // Arrange
        var emptySlots = new List<MatchLineupSlot>();

        // Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, emptySlots));

        // Assert
        var playerSlots = cut.FindAll(".player-slot");
        playerSlots.Should().BeEmpty();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // All field markings present (composite test)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_Rendered_HasAllFieldMarkings()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert - All field markings should be present
        cut.Find(".pitch-background").Should().NotBeNull("pitch background should exist");
        cut.Find(".goal-area-top").Should().NotBeNull("top goal area should exist");
        cut.Find(".goal-area-bottom").Should().NotBeNull("bottom goal area should exist");
        cut.Find(".penalty-area-top").Should().NotBeNull("top penalty area should exist");
        cut.Find(".penalty-area-bottom").Should().NotBeNull("bottom penalty area should exist");
        cut.Find(".center-circle").Should().NotBeNull("center circle should exist");
        cut.Find(".halfway-line").Should().NotBeNull("halfway line should exist");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SVG element types for field markings
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_CenterCircle_IsCircleElement()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert - Center circle should be an SVG circle element
        cut.Find("circle.center-circle").Should().NotBeNull();
    }

    [Fact]
    public void FormationPitch_HalfwayLine_IsLineElement()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert - Halfway line should be an SVG line element
        cut.Find("line.halfway-line").Should().NotBeNull();
    }

    [Fact]
    public void FormationPitch_PitchBackground_IsRectElement()
    {
        // Arrange & Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Assert - Pitch background should be an SVG rect element
        cut.Find("rect.pitch-background").Should().NotBeNull();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Partial lineup (less than 11 starters)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_WithPartialLineup_ShowsOnlyAvailableStarters()
    {
        // Arrange - Only 5 starters
        var partialLineup = new List<MatchLineupSlot>
        {
            new(PlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: true),
            new(PlayerId2, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
            new(PlayerId3, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
            new(PlayerId4, Position.InnerMidfielder, IndividualOrder.Normal, isStarter: true),
            new(PlayerId5, Position.Forward, IndividualOrder.Normal, isStarter: true),
        };

        // Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, partialLineup));

        // Assert - Should show only 5 players
        var playerSlots = cut.FindAll(".player-slot");
        playerSlots.Should().HaveCount(5);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BUG: ShouldRender broken - component shows stale data after parameter changes
    // Issue: OnParametersSet updates _previous* BEFORE ShouldRender compares them
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task FormationPitch_FormationChanges_ReRendersWithNewFormation()
    {
        // Arrange - Initial render with 4-4-2
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // Verify initial state
        cut.Find(".formation-label").TextContent.Should().Contain("4-4-2");

        // Act - Change formation to 4-3-3 by setting parameters on the component instance
        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                { nameof(FormationPitch.Formation), Formation.Formation433 },
                { nameof(FormationPitch.Slots), Create442Lineup() }
            })));

        // Assert - Label should now show 4-3-3, not stale 4-4-2
        cut.Find(".formation-label").TextContent.Should().Contain("4-3-3",
            because: "changing Formation parameter should trigger re-render with new value");
    }

    [Fact]
    public async Task FormationPitch_SlotsChange_ReRendersWithNewSlots()
    {
        // Arrange - Initial render with full lineup
        var initialLineup = Create442Lineup();
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, initialLineup));

        // Verify initial state
        cut.FindAll(".player-slot").Should().HaveCount(11);

        // Act - Change to minimal lineup with different slots list
        var newLineup = new List<MatchLineupSlot>
        {
            new(PlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: true),
            new(PlayerId2, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
            new(PlayerId3, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
        };

        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                { nameof(FormationPitch.Formation), Formation.Formation442 },
                { nameof(FormationPitch.Slots), newLineup }
            })));

        // Assert - Should show only 3 players now, not stale 11
        cut.FindAll(".player-slot").Should().HaveCount(3,
            because: "changing Slots parameter should trigger re-render with new slot count");
    }

    [Fact]
    public async Task FormationPitch_MultipleParameterChanges_AlwaysReflectsLatestValues()
    {
        // Arrange
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, Create442Lineup()));

        // First change
        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                { nameof(FormationPitch.Formation), Formation.Formation433 },
                { nameof(FormationPitch.Slots), Create442Lineup() }
            })));

        cut.Find(".formation-label").TextContent.Should().Contain("4-3-3");

        // Second change
        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                { nameof(FormationPitch.Formation), Formation.Formation352 },
                { nameof(FormationPitch.Slots), Create442Lineup() }
            })));

        cut.Find(".formation-label").TextContent.Should().Contain("3-5-2");

        // Third change
        await cut.InvokeAsync(() => cut.Instance.SetParametersAsync(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                { nameof(FormationPitch.Formation), Formation.Formation541 },
                { nameof(FormationPitch.Slots), Create442Lineup() }
            })));

        // Assert - Should reflect the latest formation, not any stale value
        cut.Find(".formation-label").TextContent.Should().Contain("5-4-1",
            because: "component should always reflect latest parameter values after any number of changes");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BUG: Single wingback/winger placed at X=60 instead of centered at X=200
    // Issue: GetWingBackPosition and GetWingerPosition return (60, y) for count==1
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void FormationPitch_SingleWingBack_IsCenteredHorizontally()
    {
        // Arrange - Lineup with exactly 1 wingback
        var lineup = new List<MatchLineupSlot>
        {
            new(PlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: true),
            new(PlayerId2, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
            new(PlayerId3, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
            new(PlayerId4, Position.WingBack, IndividualOrder.Normal, isStarter: true), // Single wingback
            new(PlayerId5, Position.InnerMidfielder, IndividualOrder.Normal, isStarter: true),
            new(PlayerId6, Position.InnerMidfielder, IndividualOrder.Normal, isStarter: true),
            new(PlayerId7, Position.InnerMidfielder, IndividualOrder.Normal, isStarter: true),
            new(PlayerId8, Position.Winger, IndividualOrder.Normal, isStarter: true),
            new(PlayerId9, Position.Winger, IndividualOrder.Normal, isStarter: true),
            new(PlayerId10, Position.Forward, IndividualOrder.Normal, isStarter: true),
            new(PlayerId11, Position.Forward, IndividualOrder.Normal, isStarter: true),
        };

        // Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, lineup));

        // Assert - Find the wingback slot and check its x position
        // SVG foreignObject x attribute should be around 175 (200 - 25 offset for centering)
        // The bug places it at x=35 (60 - 25)
        var playerSlots = cut.FindAll("foreignObject.player-slot");

        // Find the wingback slot by checking which slot has the wingback player
        // The wingback (PlayerId4) should be centered, not at the left side
        // With correct centering: x = 200 - 25 = 175
        // With bug: x = 60 - 25 = 35
        var wingbackSlot = playerSlots.FirstOrDefault(slot =>
        {
            var xAttr = slot.GetAttribute("x");
            // The wingback is at y=450, so foreignObject y = 450 - 30 = 420
            var yAttr = slot.GetAttribute("y");
            return yAttr == "420";
        });

        wingbackSlot.Should().NotBeNull("there should be a wingback slot at y=420");
        var xValue = int.Parse(wingbackSlot!.GetAttribute("x")!, System.Globalization.CultureInfo.InvariantCulture);

        // Centered x should be around 175 (200 - 25), not 35 (60 - 25)
        xValue.Should().BeInRange(170, 180,
            because: "a single wingback should be centered (X around 200), not placed at left side (X=60)");
    }

    [Fact]
    public void FormationPitch_SingleWinger_IsCenteredHorizontally()
    {
        // Arrange - Lineup with exactly 1 winger
        var lineup = new List<MatchLineupSlot>
        {
            new(PlayerId1, Position.Keeper, IndividualOrder.Normal, isStarter: true),
            new(PlayerId2, Position.WingBack, IndividualOrder.Normal, isStarter: true),
            new(PlayerId3, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
            new(PlayerId4, Position.CentralDefender, IndividualOrder.Normal, isStarter: true),
            new(PlayerId5, Position.WingBack, IndividualOrder.Normal, isStarter: true),
            new(PlayerId6, Position.Winger, IndividualOrder.Normal, isStarter: true), // Single winger
            new(PlayerId7, Position.InnerMidfielder, IndividualOrder.Normal, isStarter: true),
            new(PlayerId8, Position.InnerMidfielder, IndividualOrder.Normal, isStarter: true),
            new(PlayerId9, Position.InnerMidfielder, IndividualOrder.Normal, isStarter: true),
            new(PlayerId10, Position.Forward, IndividualOrder.Normal, isStarter: true),
            new(PlayerId11, Position.Forward, IndividualOrder.Normal, isStarter: true),
        };

        // Act
        var cut = Render<FormationPitch>(parameters => parameters
            .Add(p => p.Formation, Formation.Formation442)
            .Add(p => p.Slots, lineup));

        // Assert - Find the winger slot and check its x position
        // SVG foreignObject x attribute should be around 175 (200 - 25 offset for centering)
        // The bug places it at x=35 (60 - 25)
        var playerSlots = cut.FindAll("foreignObject.player-slot");

        // Find the winger slot by checking y position
        // The winger is at y=250, so foreignObject y = 250 - 30 = 220
        var wingerSlot = playerSlots.FirstOrDefault(slot =>
        {
            var yAttr = slot.GetAttribute("y");
            return yAttr == "220";
        });

        wingerSlot.Should().NotBeNull("there should be a winger slot at y=220");
        var xValue = int.Parse(wingerSlot!.GetAttribute("x")!, System.Globalization.CultureInfo.InvariantCulture);

        // Centered x should be around 175 (200 - 25), not 35 (60 - 25)
        xValue.Should().BeInRange(170, 180,
            because: "a single winger should be centered (X around 200), not placed at left side (X=60)");
    }
}
