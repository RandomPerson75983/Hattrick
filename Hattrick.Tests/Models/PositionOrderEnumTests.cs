using Hattrick.Core.Models;

namespace Hattrick.Tests.Models;

/// <summary>
/// Tests for Position and IndividualOrder enums (Phase 1 Sprint 1, Quartet 2).
/// Validates enum member existence, correct names, and default values.
/// </summary>
public class PositionOrderEnumTests
{
    #region Position Enum Tests

    [Fact]
    public void Position_HasExpectedMemberCount()
    {
        // Arrange & Act
        var members = ((Position[])typeof(Position).GetEnumValues()).Length;

        // Assert
        members.Should().Be(6, "Position enum should have exactly 6 members");
    }

    [Fact]
    public void Position_AllMembersAreExpected()
    {
        // Arrange & Act
        var actualNames = typeof(Position).GetEnumNames();
        var expectedNames = new[] { "Keeper", "CentralDefender", "WingBack", "InnerMidfielder", "Winger", "Forward" };

        // Assert - verify each expected name exists
        foreach (var expectedName in expectedNames)
        {
            actualNames.Should().Contain(expectedName, $"Position enum must contain {expectedName}");
        }

        // Also verify no unexpected members exist
        actualNames.Should().HaveCount(expectedNames.Length);
        actualNames.Should().BeEquivalentTo(expectedNames);
    }

    [Fact]
    public void Position_MembersHaveDefaultValues()
    {
        // Arrange & Act
        var keeper = (int)Position.Keeper;
        var centralDefender = (int)Position.CentralDefender;
        var wingBack = (int)Position.WingBack;
        var innerMidfielder = (int)Position.InnerMidfielder;
        var winger = (int)Position.Winger;
        var forward = (int)Position.Forward;

        // Assert - verify sequential default values (0-5)
        keeper.Should().Be(0);
        centralDefender.Should().Be(1);
        wingBack.Should().Be(2);
        innerMidfielder.Should().Be(3);
        winger.Should().Be(4);
        forward.Should().Be(5);
    }

    #endregion

    #region IndividualOrder Enum Tests

    [Fact]
    public void IndividualOrder_HasExpectedMemberCount()
    {
        // Arrange & Act
        var members = ((IndividualOrder[])typeof(IndividualOrder).GetEnumValues()).Length;

        // Assert
        members.Should().Be(5, "IndividualOrder enum should have exactly 5 members");
    }

    [Fact]
    public void IndividualOrder_AllMembersAreExpected()
    {
        // Arrange & Act
        var actualNames = typeof(IndividualOrder).GetEnumNames();
        var expectedNames = new[] { "Normal", "Offensive", "Defensive", "TowardsMiddle", "TowardsWing" };

        // Assert - verify each expected name exists
        foreach (var expectedName in expectedNames)
        {
            actualNames.Should().Contain(expectedName, $"IndividualOrder enum must contain {expectedName}");
        }

        // Also verify no unexpected members exist
        actualNames.Should().HaveCount(expectedNames.Length);
        actualNames.Should().BeEquivalentTo(expectedNames);
    }

    [Fact]
    public void IndividualOrder_MembersHaveDefaultValues()
    {
        // Arrange & Act
        var normal = (int)IndividualOrder.Normal;
        var offensive = (int)IndividualOrder.Offensive;
        var defensive = (int)IndividualOrder.Defensive;
        var towardsMiddle = (int)IndividualOrder.TowardsMiddle;
        var towardsWing = (int)IndividualOrder.TowardsWing;

        // Assert - verify sequential default values (0-4)
        normal.Should().Be(0);
        offensive.Should().Be(1);
        defensive.Should().Be(2);
        towardsMiddle.Should().Be(3);
        towardsWing.Should().Be(4);
    }

    #endregion
}
