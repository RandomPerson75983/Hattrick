using FluentAssertions;
using Hattrick.Core.Models;
using Xunit;

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
    public void Formation_Has442Member()
    {
        // Arrange & Act
        var has442 = typeof(Formation).GetEnumNames().Contains("Formation442");

        // Assert
        has442.Should().BeTrue("Formation enum must have Formation442 member");
    }

    [Fact]
    public void Formation_Has352Member()
    {
        // Arrange & Act
        var has352 = typeof(Formation).GetEnumNames().Contains("Formation352");

        // Assert
        has352.Should().BeTrue("Formation enum must have Formation352 member");
    }

    [Fact]
    public void Formation_Has433Member()
    {
        // Arrange & Act
        var has433 = typeof(Formation).GetEnumNames().Contains("Formation433");

        // Assert
        has433.Should().BeTrue("Formation enum must have Formation433 member");
    }

    [Fact]
    public void Formation_Has343Member()
    {
        // Arrange & Act
        var has343 = typeof(Formation).GetEnumNames().Contains("Formation343");

        // Assert
        has343.Should().BeTrue("Formation enum must have Formation343 member");
    }

    [Fact]
    public void Formation_Has541Member()
    {
        // Arrange & Act
        var has541 = typeof(Formation).GetEnumNames().Contains("Formation541");

        // Assert
        has541.Should().BeTrue("Formation enum must have Formation541 member");
    }

    [Fact]
    public void Formation_Has451Member()
    {
        // Arrange & Act
        var has451 = typeof(Formation).GetEnumNames().Contains("Formation451");

        // Assert
        has451.Should().BeTrue("Formation enum must have Formation451 member");
    }

    [Fact]
    public void Formation_Has532Member()
    {
        // Arrange & Act
        var has532 = typeof(Formation).GetEnumNames().Contains("Formation532");

        // Assert
        has532.Should().BeTrue("Formation enum must have Formation532 member");
    }

    [Fact]
    public void Formation_Has523Member()
    {
        // Arrange & Act
        var has523 = typeof(Formation).GetEnumNames().Contains("Formation523");

        // Assert
        has523.Should().BeTrue("Formation enum must have Formation523 member");
    }

    [Fact]
    public void Formation_Has550Member()
    {
        // Arrange & Act
        var has550 = typeof(Formation).GetEnumNames().Contains("Formation550");

        // Assert
        has550.Should().BeTrue("Formation enum must have Formation550 member");
    }

    [Fact]
    public void Formation_Has253Member()
    {
        // Arrange & Act
        var has253 = typeof(Formation).GetEnumNames().Contains("Formation253");

        // Assert
        has253.Should().BeTrue("Formation enum must have Formation253 member");
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

    [Fact]
    public void Formation_CanBeConvertedToString()
    {
        // Arrange
        var formation = Formation.Formation442;

        // Act
        var stringValue = formation.ToString();

        // Assert
        stringValue.Should().Be("Formation442");
    }

    [Fact]
    public void Formation_CanBeCastFromInt()
    {
        // Arrange
        const int formationValue = 0;

        // Act
        var formation = (Formation)formationValue;

        // Assert
        formation.Should().Be(Formation.Formation442);
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
    public void Tactic_HasNormalMember()
    {
        // Arrange & Act
        var hasNormal = typeof(Tactic).GetEnumNames().Contains("Normal");

        // Assert
        hasNormal.Should().BeTrue("Tactic enum must have Normal member");
    }

    [Fact]
    public void Tactic_HasPressingMember()
    {
        // Arrange & Act
        var hasPressing = typeof(Tactic).GetEnumNames().Contains("Pressing");

        // Assert
        hasPressing.Should().BeTrue("Tactic enum must have Pressing member");
    }

    [Fact]
    public void Tactic_HasCounterAttackMember()
    {
        // Arrange & Act
        var hasCounterAttack = typeof(Tactic).GetEnumNames().Contains("CounterAttack");

        // Assert
        hasCounterAttack.Should().BeTrue("Tactic enum must have CounterAttack member");
    }

    [Fact]
    public void Tactic_HasAttackInMiddleMember()
    {
        // Arrange & Act
        var hasAttackInMiddle = typeof(Tactic).GetEnumNames().Contains("AttackInMiddle");

        // Assert
        hasAttackInMiddle.Should().BeTrue("Tactic enum must have AttackInMiddle member");
    }

    [Fact]
    public void Tactic_HasAttackOnWingsMember()
    {
        // Arrange & Act
        var hasAttackOnWings = typeof(Tactic).GetEnumNames().Contains("AttackOnWings");

        // Assert
        hasAttackOnWings.Should().BeTrue("Tactic enum must have AttackOnWings member");
    }

    [Fact]
    public void Tactic_HasPlayCreativelyMember()
    {
        // Arrange & Act
        var hasPlayCreatively = typeof(Tactic).GetEnumNames().Contains("PlayCreatively");

        // Assert
        hasPlayCreatively.Should().BeTrue("Tactic enum must have PlayCreatively member");
    }

    [Fact]
    public void Tactic_HasLongShotsMember()
    {
        // Arrange & Act
        var hasLongShots = typeof(Tactic).GetEnumNames().Contains("LongShots");

        // Assert
        hasLongShots.Should().BeTrue("Tactic enum must have LongShots member");
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

    [Fact]
    public void Tactic_CanBeConvertedToString()
    {
        // Arrange
        var tactic = Tactic.Normal;

        // Act
        var stringValue = tactic.ToString();

        // Assert
        stringValue.Should().Be("Normal");
    }

    [Fact]
    public void Tactic_CanBeCastFromInt()
    {
        // Arrange
        const int tacticValue = 0;

        // Act
        var tactic = (Tactic)tacticValue;

        // Assert
        tactic.Should().Be(Tactic.Normal);
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
    public void TeamAttitude_HasPlayItCoolMember()
    {
        // Arrange & Act
        var hasPlayItCool = typeof(TeamAttitude).GetEnumNames().Contains("PlayItCool");

        // Assert
        hasPlayItCool.Should().BeTrue("TeamAttitude enum must have PlayItCool member");
    }

    [Fact]
    public void TeamAttitude_HasNormalMember()
    {
        // Arrange & Act
        var hasNormal = typeof(TeamAttitude).GetEnumNames().Contains("Normal");

        // Assert
        hasNormal.Should().BeTrue("TeamAttitude enum must have Normal member");
    }

    [Fact]
    public void TeamAttitude_HasMatchOfTheSeasonMember()
    {
        // Arrange & Act
        var hasMatchOfTheSeason = typeof(TeamAttitude).GetEnumNames().Contains("MatchOfTheSeason");

        // Assert
        hasMatchOfTheSeason.Should().BeTrue("TeamAttitude enum must have MatchOfTheSeason member");
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

    [Fact]
    public void TeamAttitude_CanBeConvertedToString()
    {
        // Arrange
        var attitude = TeamAttitude.Normal;

        // Act
        var stringValue = attitude.ToString();

        // Assert
        stringValue.Should().Be("Normal");
    }

    [Fact]
    public void TeamAttitude_CanBeCastFromInt()
    {
        // Arrange
        const int attitudeValue = 1;

        // Act
        var attitude = (TeamAttitude)attitudeValue;

        // Assert
        attitude.Should().Be(TeamAttitude.Normal);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void AllFormationMembers_CanBeIterated()
    {
        // Arrange & Act
        var formations = Enum.GetValues(typeof(Formation)).Cast<Formation>().ToList();

        // Assert
        formations.Should().HaveCount(10);
        formations.Should().Contain(Formation.Formation442);
        formations.Should().Contain(Formation.Formation352);
        formations.Should().Contain(Formation.Formation433);
        formations.Should().Contain(Formation.Formation343);
        formations.Should().Contain(Formation.Formation541);
        formations.Should().Contain(Formation.Formation451);
        formations.Should().Contain(Formation.Formation532);
        formations.Should().Contain(Formation.Formation523);
        formations.Should().Contain(Formation.Formation550);
        formations.Should().Contain(Formation.Formation253);
    }

    [Fact]
    public void AllTacticMembers_CanBeIterated()
    {
        // Arrange & Act
        var tactics = Enum.GetValues(typeof(Tactic)).Cast<Tactic>().ToList();

        // Assert
        tactics.Should().HaveCount(7);
        tactics.Should().Contain(Tactic.Normal);
        tactics.Should().Contain(Tactic.Pressing);
        tactics.Should().Contain(Tactic.CounterAttack);
        tactics.Should().Contain(Tactic.AttackInMiddle);
        tactics.Should().Contain(Tactic.AttackOnWings);
        tactics.Should().Contain(Tactic.PlayCreatively);
        tactics.Should().Contain(Tactic.LongShots);
    }

    [Fact]
    public void AllTeamAttitudeMembers_CanBeIterated()
    {
        // Arrange & Act
        var attitudes = Enum.GetValues(typeof(TeamAttitude)).Cast<TeamAttitude>().ToList();

        // Assert
        attitudes.Should().HaveCount(3);
        attitudes.Should().Contain(TeamAttitude.PlayItCool);
        attitudes.Should().Contain(TeamAttitude.Normal);
        attitudes.Should().Contain(TeamAttitude.MatchOfTheSeason);
    }

    [Fact]
    public void Formation_And_Tactic_AndTeamAttitude_CanCoexist()
    {
        // Arrange
        var formation = Formation.Formation442;
        var tactic = Tactic.Normal;
        var attitude = TeamAttitude.Normal;

        // Act & Assert
        formation.Should().Be(Formation.Formation442);
        tactic.Should().Be(Tactic.Normal);
        attitude.Should().Be(TeamAttitude.Normal);
    }

    [Fact]
    public void FormationValues_AreUniqueAndSequential()
    {
        // Arrange & Act
        var values = Enum.GetValues(typeof(Formation)).Cast<int>().ToList();

        // Assert
        values.Should().BeInAscendingOrder();
        values.Should().ContainSingle(x => x == 0);
        values.Should().ContainSingle(x => x == 9);
        values.Distinct().Count().Should().Be(10, "All formation values should be unique");
    }

    [Fact]
    public void TacticValues_AreUniqueAndSequential()
    {
        // Arrange & Act
        var values = Enum.GetValues(typeof(Tactic)).Cast<int>().ToList();

        // Assert
        values.Should().BeInAscendingOrder();
        values.Should().ContainSingle(x => x == 0);
        values.Should().ContainSingle(x => x == 6);
        values.Distinct().Count().Should().Be(7, "All tactic values should be unique");
    }

    [Fact]
    public void TeamAttitudeValues_AreUniqueAndSequential()
    {
        // Arrange & Act
        var values = Enum.GetValues(typeof(TeamAttitude)).Cast<int>().ToList();

        // Assert
        values.Should().BeInAscendingOrder();
        values.Should().ContainSingle(x => x == 0);
        values.Should().ContainSingle(x => x == 2);
        values.Distinct().Count().Should().Be(3, "All team attitude values should be unique");
    }

    #endregion
}
