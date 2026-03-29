using Hattrick.Core.Models;

namespace Hattrick.Tests.Models;

/// <summary>
/// Tests for Formation, Tactic, and TeamAttitude enums (Phase 1 Sprint 1, Quartet 3).
/// Validates enum member existence, correct names, and proper values.
/// </summary>
public class TacticsFormationEnumTests
{
    #region Formation Enum Tests

    [Fact]
    public void Formation_HasExpectedMemberCount()
    {
        // Arrange & Act
        var members = ((Formation[])typeof(Formation).GetEnumValues()).Length;

        // Assert
        members.Should().Be(10, "Formation enum should have exactly 10 members");
    }

    [Fact]
    public void Formation_AllMembersAreExpected()
    {
        // Arrange & Act
        var actualNames = typeof(Formation).GetEnumNames();
        var expectedNames = new[]
        {
            "Formation442", "Formation352", "Formation433", "Formation343", "Formation541",
            "Formation451", "Formation532", "Formation523", "Formation550", "Formation253"
        };

        // Assert - verify each expected name exists
        foreach (var expectedName in expectedNames)
        {
            actualNames.Should().Contain(expectedName, $"Formation enum must contain {expectedName}");
        }

        // Also verify no unexpected members exist
        actualNames.Should().HaveCount(expectedNames.Length);
        actualNames.Should().BeEquivalentTo(expectedNames);
    }

    [Fact]
    public void Formation_MembersHaveDefaultValues()
    {
        // Arrange & Act
        var formation442 = (int)Formation.Formation442;
        var formation352 = (int)Formation.Formation352;
        var formation433 = (int)Formation.Formation433;
        var formation343 = (int)Formation.Formation343;
        var formation541 = (int)Formation.Formation541;
        var formation451 = (int)Formation.Formation451;
        var formation532 = (int)Formation.Formation532;
        var formation523 = (int)Formation.Formation523;
        var formation550 = (int)Formation.Formation550;
        var formation253 = (int)Formation.Formation253;

        // Assert - verify sequential default values (0-9)
        formation442.Should().Be(0);
        formation352.Should().Be(1);
        formation433.Should().Be(2);
        formation343.Should().Be(3);
        formation541.Should().Be(4);
        formation451.Should().Be(5);
        formation532.Should().Be(6);
        formation523.Should().Be(7);
        formation550.Should().Be(8);
        formation253.Should().Be(9);
    }

    #endregion

    #region Tactic Enum Tests

    [Fact]
    public void Tactic_HasExpectedMemberCount()
    {
        // Arrange & Act
        var members = ((Tactic[])typeof(Tactic).GetEnumValues()).Length;

        // Assert
        members.Should().Be(7, "Tactic enum should have exactly 7 members");
    }

    [Fact]
    public void Tactic_AllMembersAreExpected()
    {
        // Arrange & Act
        var actualNames = typeof(Tactic).GetEnumNames();
        var expectedNames = new[]
        {
            "Normal", "Pressing", "CounterAttack", "AttackInMiddle",
            "AttackOnWings", "PlayCreatively", "LongShots"
        };

        // Assert - verify each expected name exists
        foreach (var expectedName in expectedNames)
        {
            actualNames.Should().Contain(expectedName, $"Tactic enum must contain {expectedName}");
        }

        // Also verify no unexpected members exist
        actualNames.Should().HaveCount(expectedNames.Length);
        actualNames.Should().BeEquivalentTo(expectedNames);
    }

    [Fact]
    public void Tactic_MembersHaveDefaultValues()
    {
        // Arrange & Act
        var normal = (int)Tactic.Normal;
        var pressing = (int)Tactic.Pressing;
        var counterAttack = (int)Tactic.CounterAttack;
        var attackInMiddle = (int)Tactic.AttackInMiddle;
        var attackOnWings = (int)Tactic.AttackOnWings;
        var playCreatively = (int)Tactic.PlayCreatively;
        var longShots = (int)Tactic.LongShots;

        // Assert - verify sequential default values (0-6)
        normal.Should().Be(0);
        pressing.Should().Be(1);
        counterAttack.Should().Be(2);
        attackInMiddle.Should().Be(3);
        attackOnWings.Should().Be(4);
        playCreatively.Should().Be(5);
        longShots.Should().Be(6);
    }

    #endregion

    #region TeamAttitude Enum Tests

    [Fact]
    public void TeamAttitude_HasExpectedMemberCount()
    {
        // Arrange & Act
        var members = ((TeamAttitude[])typeof(TeamAttitude).GetEnumValues()).Length;

        // Assert
        members.Should().Be(3, "TeamAttitude enum should have exactly 3 members");
    }

    [Fact]
    public void TeamAttitude_AllMembersAreExpected()
    {
        // Arrange & Act
        var actualNames = typeof(TeamAttitude).GetEnumNames();
        var expectedNames = new[] { "PlayItCool", "Normal", "MatchOfTheSeason" };

        // Assert - verify each expected name exists
        foreach (var expectedName in expectedNames)
        {
            actualNames.Should().Contain(expectedName, $"TeamAttitude enum must contain {expectedName}");
        }

        // Also verify no unexpected members exist
        actualNames.Should().HaveCount(expectedNames.Length);
        actualNames.Should().BeEquivalentTo(expectedNames);
    }

    [Fact]
    public void TeamAttitude_MembersHaveDefaultValues()
    {
        // Arrange & Act
        var playItCool = (int)TeamAttitude.PlayItCool;
        var normal = (int)TeamAttitude.Normal;
        var matchOfTheSeason = (int)TeamAttitude.MatchOfTheSeason;

        // Assert - verify sequential default values (0-2)
        playItCool.Should().Be(0);
        normal.Should().Be(1);
        matchOfTheSeason.Should().Be(2);
    }

    #endregion
}
