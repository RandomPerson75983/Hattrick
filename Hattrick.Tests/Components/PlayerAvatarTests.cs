using Bunit;
using FluentAssertions;
using Hattrick.Core.Models;
using Hattrick.Components.Shared.Lineup;

namespace Hattrick.Tests.Components;

/// <summary>
/// bUnit tests for the PlayerAvatar.razor component.
///
/// PlayerAvatar is a visual component for displaying a player in lineup views:
/// - Circular avatar showing jersey number (or initials if no jersey number)
/// - Position badge with color coding by position
/// - Clickable with selection state
/// - Three size variants (small, medium, large)
///
/// Expected HTML structure:
///   div.player-avatar.player-avatar-{size}[.selected]
///   └── div.avatar-circle.position-{color}
///       └── span.avatar-content (jersey number or initials)
///   └── span.position-badge (position abbreviation)
///
/// Position color mapping:
///   Keeper = yellow, Defender = blue, Midfielder = green, Forward = red
/// </summary>
public class PlayerAvatarTests : BunitContext
{
    private static readonly Guid DefaultPlayerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private const string DefaultPlayerName = "Max Schmidt";
    private const Position DefaultPosition = Position.Forward;

    // ─────────────────────────────────────────────────────────────────────────
    // Container rendering
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayerAvatar_Rendered_HasPlayerAvatarContainer()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition));

        // Assert
        cut.Find(".player-avatar").Should().NotBeNull();
    }

    [Fact]
    public void PlayerAvatar_Rendered_HasAvatarCircle()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition));

        // Assert
        cut.Find(".avatar-circle").Should().NotBeNull();
    }

    [Fact]
    public void PlayerAvatar_Rendered_HasAvatarContent()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition));

        // Assert
        cut.Find(".avatar-content").Should().NotBeNull();
    }

    [Fact]
    public void PlayerAvatar_Rendered_HasPositionBadge()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition));

        // Assert
        cut.Find(".position-badge").Should().NotBeNull();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Size variants - CSS classes
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayerAvatar_SizeSmall_HasSmallCssClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.Size, PlayerAvatarSize.Small));

        // Assert
        cut.Find(".player-avatar").ClassList.Should().Contain("player-avatar-small");
    }

    [Fact]
    public void PlayerAvatar_SizeMedium_HasMediumCssClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.Size, PlayerAvatarSize.Medium));

        // Assert
        cut.Find(".player-avatar").ClassList.Should().Contain("player-avatar-medium");
    }

    [Fact]
    public void PlayerAvatar_SizeLarge_HasLargeCssClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.Size, PlayerAvatarSize.Large));

        // Assert
        cut.Find(".player-avatar").ClassList.Should().Contain("player-avatar-large");
    }

    [Fact]
    public void PlayerAvatar_SizeSmall_DoesNotHaveMediumOrLargeClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.Size, PlayerAvatarSize.Small));

        // Assert
        var classList = cut.Find(".player-avatar").ClassList;
        classList.Should().NotContain("player-avatar-medium");
        classList.Should().NotContain("player-avatar-large");
    }

    [Fact]
    public void PlayerAvatar_SizeMedium_DoesNotHaveSmallOrLargeClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.Size, PlayerAvatarSize.Medium));

        // Assert
        var classList = cut.Find(".player-avatar").ClassList;
        classList.Should().NotContain("player-avatar-small");
        classList.Should().NotContain("player-avatar-large");
    }

    [Fact]
    public void PlayerAvatar_SizeLarge_DoesNotHaveSmallOrMediumClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.Size, PlayerAvatarSize.Large));

        // Assert
        var classList = cut.Find(".player-avatar").ClassList;
        classList.Should().NotContain("player-avatar-small");
        classList.Should().NotContain("player-avatar-medium");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Jersey number display
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayerAvatar_WithJerseyNumber_ShowsJerseyNumber()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.JerseyNumber, 10));

        // Assert
        cut.Find(".avatar-content").TextContent.Should().Be("10");
    }

    [Fact]
    public void PlayerAvatar_WithDifferentJerseyNumber_ShowsThatNumber()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.JerseyNumber, 23));

        // Assert
        cut.Find(".avatar-content").TextContent.Should().Be("23");
    }

    [Fact]
    public void PlayerAvatar_WithJerseyNumberOne_ShowsOne()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, "Goalkeeper")
            .Add(p => p.Position, Position.Keeper)
            .Add(p => p.JerseyNumber, 1));

        // Assert
        cut.Find(".avatar-content").TextContent.Should().Be("1");
    }

    [Fact]
    public void PlayerAvatar_WithJerseyNumber99_Shows99()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.JerseyNumber, 99));

        // Assert
        cut.Find(".avatar-content").TextContent.Should().Be("99");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Initials fallback when no jersey number
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayerAvatar_WithoutJerseyNumber_ShowsInitials()
    {
        // Arrange & Act - no JerseyNumber provided
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, "Max Schmidt")
            .Add(p => p.Position, DefaultPosition));

        // Assert - Should show "MS" for "Max Schmidt"
        cut.Find(".avatar-content").TextContent.Should().Be("MS");
    }

    [Fact]
    public void PlayerAvatar_WithoutJerseyNumber_SingleName_ShowsFirstTwoLetters()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, "Ronaldo")
            .Add(p => p.Position, DefaultPosition));

        // Assert - Should show "RO" for single-name player
        cut.Find(".avatar-content").TextContent.Should().Be("RO");
    }

    [Fact]
    public void PlayerAvatar_WithoutJerseyNumber_ThreePartName_ShowsFirstAndLastInitials()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, "Johan Van Berg")
            .Add(p => p.Position, DefaultPosition));

        // Assert - Should show "JB" (first and last name initials)
        cut.Find(".avatar-content").TextContent.Should().Be("JB");
    }

    [Fact]
    public void PlayerAvatar_WithoutJerseyNumber_LowercaseName_ShowsUppercaseInitials()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, "max schmidt")
            .Add(p => p.Position, DefaultPosition));

        // Assert - Initials should be uppercase regardless of input
        cut.Find(".avatar-content").TextContent.Should().Be("MS");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Position colors - CSS classes
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayerAvatar_PositionKeeper_HasYellowColorClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, Position.Keeper));

        // Assert
        cut.Find(".avatar-circle").ClassList.Should().Contain("position-yellow");
    }

    [Fact]
    public void PlayerAvatar_PositionCentralDefender_HasBlueColorClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, Position.CentralDefender));

        // Assert
        cut.Find(".avatar-circle").ClassList.Should().Contain("position-blue");
    }

    [Fact]
    public void PlayerAvatar_PositionWingBack_HasBlueColorClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, Position.WingBack));

        // Assert
        cut.Find(".avatar-circle").ClassList.Should().Contain("position-blue");
    }

    [Fact]
    public void PlayerAvatar_PositionInnerMidfielder_HasGreenColorClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, Position.InnerMidfielder));

        // Assert
        cut.Find(".avatar-circle").ClassList.Should().Contain("position-green");
    }

    [Fact]
    public void PlayerAvatar_PositionWinger_HasGreenColorClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, Position.Winger));

        // Assert
        cut.Find(".avatar-circle").ClassList.Should().Contain("position-green");
    }

    [Fact]
    public void PlayerAvatar_PositionForward_HasRedColorClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, Position.Forward));

        // Assert
        cut.Find(".avatar-circle").ClassList.Should().Contain("position-red");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Position badge content
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayerAvatar_PositionKeeper_BadgeShowsGK()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, Position.Keeper));

        // Assert
        cut.Find(".position-badge").TextContent.Should().Be("GK");
    }

    [Fact]
    public void PlayerAvatar_PositionCentralDefender_BadgeShowsCD()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, Position.CentralDefender));

        // Assert
        cut.Find(".position-badge").TextContent.Should().Be("CD");
    }

    [Fact]
    public void PlayerAvatar_PositionWingBack_BadgeShowsWB()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, Position.WingBack));

        // Assert
        cut.Find(".position-badge").TextContent.Should().Be("WB");
    }

    [Fact]
    public void PlayerAvatar_PositionInnerMidfielder_BadgeShowsIM()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, Position.InnerMidfielder));

        // Assert
        cut.Find(".position-badge").TextContent.Should().Be("IM");
    }

    [Fact]
    public void PlayerAvatar_PositionWinger_BadgeShowsWI()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, Position.Winger));

        // Assert
        cut.Find(".position-badge").TextContent.Should().Be("WI");
    }

    [Fact]
    public void PlayerAvatar_PositionForward_BadgeShowsFW()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, Position.Forward));

        // Assert
        cut.Find(".position-badge").TextContent.Should().Be("FW");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Selection state
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayerAvatar_IsSelectedTrue_HasSelectedCssClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.IsSelected, true));

        // Assert
        cut.Find(".player-avatar").ClassList.Should().Contain("selected");
    }

    [Fact]
    public void PlayerAvatar_IsSelectedFalse_DoesNotHaveSelectedCssClass()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.IsSelected, false));

        // Assert
        cut.Find(".player-avatar").ClassList.Should().NotContain("selected");
    }

    [Fact]
    public void PlayerAvatar_DefaultSelection_IsNotSelected()
    {
        // Arrange & Act - no IsSelected provided, should default to false
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition));

        // Assert
        cut.Find(".player-avatar").ClassList.Should().NotContain("selected");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Click handler
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayerAvatar_WhenClicked_InvokesOnClickCallback()
    {
        // Arrange
        var clicked = false;

        // Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.OnClick, EventCallback.Factory.Create(this, () => clicked = true)));
        cut.Find(".player-avatar").Click();

        // Assert
        clicked.Should().BeTrue();
    }

    [Fact]
    public void PlayerAvatar_WhenClickedMultipleTimes_InvokesCallbackEachTime()
    {
        // Arrange
        var clickCount = 0;

        // Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.OnClick, EventCallback.Factory.Create(this, () => clickCount++)));
        var element = cut.Find(".player-avatar");
        element.Click();
        element.Click();
        element.Click();

        // Assert
        clickCount.Should().Be(3);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Edge cases
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayerAvatar_EmptyName_ShowsEmptyInitials()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, "")
            .Add(p => p.Position, DefaultPosition));

        // Assert - Should handle empty name gracefully with empty initials
        var content = cut.Find(".avatar-content");
        content.Should().NotBeNull();
        content.TextContent.Should().BeEmpty();
    }

    [Fact]
    public void PlayerAvatar_JerseyNumberZero_ShowsZero()
    {
        // Arrange & Act - Jersey number 0 is technically valid (some leagues allow it)
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.JerseyNumber, 0));

        // Assert
        cut.Find(".avatar-content").TextContent.Should().Be("0");
    }

    [Fact]
    public void PlayerAvatar_WithNullJerseyNumber_ShowsInitialsNotNull()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, "Max Schmidt")
            .Add(p => p.Position, DefaultPosition)
            .Add(p => p.JerseyNumber, (int?)null));

        // Assert - Should NOT show "null" as text, should show initials
        cut.Find(".avatar-content").TextContent.Should().NotBe("null");
        cut.Find(".avatar-content").TextContent.Should().Be("MS");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // All positions have correct CSS/badge combinations (Theory tests)
    // ─────────────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(Position.Keeper, "position-yellow", "GK")]
    [InlineData(Position.CentralDefender, "position-blue", "CD")]
    [InlineData(Position.WingBack, "position-blue", "WB")]
    [InlineData(Position.InnerMidfielder, "position-green", "IM")]
    [InlineData(Position.Winger, "position-green", "WI")]
    [InlineData(Position.Forward, "position-red", "FW")]
    public void PlayerAvatar_Position_HasCorrectColorAndBadge(Position position, string expectedColorClass, string expectedBadge)
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, position));

        // Assert
        cut.Find(".avatar-circle").ClassList.Should().Contain(expectedColorClass);
        cut.Find(".position-badge").TextContent.Should().Be(expectedBadge);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Structure validation - element presence and ordering
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayerAvatar_Layout_AvatarCircleInsideContainer()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition));

        // Assert - avatar-circle should be a descendant of player-avatar
        var container = cut.Find(".player-avatar");
        container.QuerySelector(".avatar-circle").Should().NotBeNull();
    }

    [Fact]
    public void PlayerAvatar_Layout_AvatarContentInsideCircle()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition));

        // Assert - avatar-content should be inside avatar-circle
        var circle = cut.Find(".avatar-circle");
        circle.QuerySelector(".avatar-content").Should().NotBeNull();
    }

    [Fact]
    public void PlayerAvatar_Layout_PositionBadgeInsideContainer()
    {
        // Arrange & Act
        var cut = Render<PlayerAvatar>(parameters => parameters
            .Add(p => p.PlayerId, DefaultPlayerId)
            .Add(p => p.Name, DefaultPlayerName)
            .Add(p => p.Position, DefaultPosition));

        // Assert - position-badge should be a descendant of player-avatar
        var container = cut.Find(".player-avatar");
        container.QuerySelector(".position-badge").Should().NotBeNull();
    }
}
