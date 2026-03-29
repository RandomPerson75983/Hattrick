using Hattrick.Core.Models;

namespace Hattrick.Tests.Models;

/// <summary>
/// Tests for Specialty, PlayerPersonality, and CoachType enums.
/// Verifies enum member existence, correct values, and member counts.
/// </summary>
public class SpecialtyPersonalityEnumTests
{
    #region Specialty Enum Tests

    [Fact]
    public void Specialty_HasExactlyEightMembers()
    {
        // Act
        var members = Enum.GetValues(typeof(Specialty)).Cast<Specialty>().ToList();

        // Assert
        members.Should().HaveCount(8);
    }

    [Fact]
    public void Specialty_MembersHaveCorrectNames()
    {
        // Act
        var memberNames = Enum.GetNames(typeof(Specialty));

        // Assert
        memberNames.Should().HaveCount(8).And.BeEquivalentTo(new[]
        {
            "None", "Technical", "Quick", "Head",
            "Powerful", "Unpredictable", "Resilient", "Support"
        });
    }

    [Fact]
    public void Specialty_MembersHaveDefaultValues()
    {
        // Assert - verify sequential default values (0-7)
        ((int)Specialty.None).Should().Be(0);
        ((int)Specialty.Technical).Should().Be(1);
        ((int)Specialty.Quick).Should().Be(2);
        ((int)Specialty.Head).Should().Be(3);
        ((int)Specialty.Powerful).Should().Be(4);
        ((int)Specialty.Unpredictable).Should().Be(5);
        ((int)Specialty.Resilient).Should().Be(6);
        ((int)Specialty.Support).Should().Be(7);
    }

    #endregion

    #region PlayerPersonality Enum Tests

    [Fact]
    public void PlayerPersonality_HasExactlySixMembers()
    {
        // Act
        var members = Enum.GetValues(typeof(PlayerPersonality)).Cast<PlayerPersonality>().ToList();

        // Assert
        members.Should().HaveCount(6);
    }

    [Fact]
    public void PlayerPersonality_MembersHaveCorrectNames()
    {
        // Act
        var memberNames = Enum.GetNames(typeof(PlayerPersonality));

        // Assert
        memberNames.Should().HaveCount(6).And.BeEquivalentTo(new[]
        {
            "Nice", "Nasty", "Leader", "Loner", "Temperamental", "Calm"
        });
    }

    [Fact]
    public void PlayerPersonality_MembersHaveDefaultValues()
    {
        // Assert - verify sequential default values (0-5)
        ((int)PlayerPersonality.Nice).Should().Be(0);
        ((int)PlayerPersonality.Nasty).Should().Be(1);
        ((int)PlayerPersonality.Leader).Should().Be(2);
        ((int)PlayerPersonality.Loner).Should().Be(3);
        ((int)PlayerPersonality.Temperamental).Should().Be(4);
        ((int)PlayerPersonality.Calm).Should().Be(5);
    }

    #endregion

    #region CoachType Enum Tests

    [Fact]
    public void CoachType_HasExactlyThreeMembers()
    {
        // Act
        var members = Enum.GetValues(typeof(CoachType)).Cast<CoachType>().ToList();

        // Assert
        members.Should().HaveCount(3);
    }

    [Fact]
    public void CoachType_MembersHaveCorrectNames()
    {
        // Act
        var memberNames = Enum.GetNames(typeof(CoachType));

        // Assert
        memberNames.Should().HaveCount(3).And.BeEquivalentTo(new[]
        {
            "Offensive", "Defensive", "Balanced"
        });
    }

    [Fact]
    public void CoachType_MembersHaveDefaultValues()
    {
        // Assert - verify sequential default values (0-2)
        ((int)CoachType.Offensive).Should().Be(0);
        ((int)CoachType.Defensive).Should().Be(1);
        ((int)CoachType.Balanced).Should().Be(2);
    }

    #endregion
}
