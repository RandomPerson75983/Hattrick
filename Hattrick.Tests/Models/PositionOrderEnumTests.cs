using FluentAssertions;
using Hattrick.Core.Models;
using Xunit;

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
    public void Position_HasKeeperMember()
    {
        // Arrange & Act
        var hasKeeper = typeof(Position).GetEnumNames().Contains("Keeper");

        // Assert
        hasKeeper.Should().BeTrue("Position enum must have Keeper member");
    }

    [Fact]
    public void Position_HasCentralDefenderMember()
    {
        // Arrange & Act
        var hasCentralDefender = typeof(Position).GetEnumNames().Contains("CentralDefender");

        // Assert
        hasCentralDefender.Should().BeTrue("Position enum must have CentralDefender member");
    }

    [Fact]
    public void Position_HasWingBackMember()
    {
        // Arrange & Act
        var hasWingBack = typeof(Position).GetEnumNames().Contains("WingBack");

        // Assert
        hasWingBack.Should().BeTrue("Position enum must have WingBack member");
    }

    [Fact]
    public void Position_HasInnerMidfielderMember()
    {
        // Arrange & Act
        var hasInnerMidfielder = typeof(Position).GetEnumNames().Contains("InnerMidfielder");

        // Assert
        hasInnerMidfielder.Should().BeTrue("Position enum must have InnerMidfielder member");
    }

    [Fact]
    public void Position_HasWingerMember()
    {
        // Arrange & Act
        var hasWinger = typeof(Position).GetEnumNames().Contains("Winger");

        // Assert
        hasWinger.Should().BeTrue("Position enum must have Winger member");
    }

    [Fact]
    public void Position_HasForwardMember()
    {
        // Arrange & Act
        var hasForward = typeof(Position).GetEnumNames().Contains("Forward");

        // Assert
        hasForward.Should().BeTrue("Position enum must have Forward member");
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
    public void IndividualOrder_HasNormalMember()
    {
        // Arrange & Act
        var hasNormal = typeof(IndividualOrder).GetEnumNames().Contains("Normal");

        // Assert
        hasNormal.Should().BeTrue("IndividualOrder enum must have Normal member");
    }

    [Fact]
    public void IndividualOrder_HasOffensiveMember()
    {
        // Arrange & Act
        var hasOffensive = typeof(IndividualOrder).GetEnumNames().Contains("Offensive");

        // Assert
        hasOffensive.Should().BeTrue("IndividualOrder enum must have Offensive member");
    }

    [Fact]
    public void IndividualOrder_HasDefensiveMember()
    {
        // Arrange & Act
        var hasDefensive = typeof(IndividualOrder).GetEnumNames().Contains("Defensive");

        // Assert
        hasDefensive.Should().BeTrue("IndividualOrder enum must have Defensive member");
    }

    [Fact]
    public void IndividualOrder_HasTowardsMiddleMember()
    {
        // Arrange & Act
        var hasTowardsMiddle = typeof(IndividualOrder).GetEnumNames().Contains("TowardsMiddle");

        // Assert
        hasTowardsMiddle.Should().BeTrue("IndividualOrder enum must have TowardsMiddle member");
    }

    [Fact]
    public void IndividualOrder_HasTowardsWingMember()
    {
        // Arrange & Act
        var hasTowardsWing = typeof(IndividualOrder).GetEnumNames().Contains("TowardsWing");

        // Assert
        hasTowardsWing.Should().BeTrue("IndividualOrder enum must have TowardsWing member");
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

    #region Integration Tests

    [Fact]
    public void Position_CanBeConvertedToString()
    {
        // Arrange
        var position = Position.Keeper;

        // Act
        var stringValue = position.ToString();

        // Assert
        stringValue.Should().Be("Keeper");
    }

    [Fact]
    public void Position_CanBeCastFromInt()
    {
        // Arrange
        const int positionValue = 0;

        // Act
        var position = (Position)positionValue;

        // Assert
        position.Should().Be(Position.Keeper);
    }

    [Fact]
    public void IndividualOrder_CanBeConvertedToString()
    {
        // Arrange
        var order = IndividualOrder.Normal;

        // Act
        var stringValue = order.ToString();

        // Assert
        stringValue.Should().Be("Normal");
    }

    [Fact]
    public void IndividualOrder_CanBeCastFromInt()
    {
        // Arrange
        const int orderValue = 0;

        // Act
        var order = (IndividualOrder)orderValue;

        // Assert
        order.Should().Be(IndividualOrder.Normal);
    }

    #endregion
}
