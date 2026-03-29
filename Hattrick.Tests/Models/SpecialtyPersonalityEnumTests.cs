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
    public void Specialty_HasNoneMember()
    {
        // Assert
        ((Specialty)Enum.Parse(typeof(Specialty), "None")).Should().Be(Specialty.None);
    }

    [Fact]
    public void Specialty_HasTechnicalMember()
    {
        // Assert
        ((Specialty)Enum.Parse(typeof(Specialty), "Technical")).Should().Be(Specialty.Technical);
    }

    [Fact]
    public void Specialty_HasQuickMember()
    {
        // Assert
        ((Specialty)Enum.Parse(typeof(Specialty), "Quick")).Should().Be(Specialty.Quick);
    }

    [Fact]
    public void Specialty_HasHeadMember()
    {
        // Assert
        ((Specialty)Enum.Parse(typeof(Specialty), "Head")).Should().Be(Specialty.Head);
    }

    [Fact]
    public void Specialty_HasPowerfulMember()
    {
        // Assert
        ((Specialty)Enum.Parse(typeof(Specialty), "Powerful")).Should().Be(Specialty.Powerful);
    }

    [Fact]
    public void Specialty_HasUnpredictableMember()
    {
        // Assert
        ((Specialty)Enum.Parse(typeof(Specialty), "Unpredictable")).Should().Be(Specialty.Unpredictable);
    }

    [Fact]
    public void Specialty_HasResilientMember()
    {
        // Assert
        ((Specialty)Enum.Parse(typeof(Specialty), "Resilient")).Should().Be(Specialty.Resilient);
    }

    [Fact]
    public void Specialty_HasSupportMember()
    {
        // Assert
        ((Specialty)Enum.Parse(typeof(Specialty), "Support")).Should().Be(Specialty.Support);
    }

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
    public void Specialty_AllMembers_CanBeEnumerated()
    {
        // Act
        var specialties = Enum.GetValues(typeof(Specialty)).Cast<Specialty>().ToList();
        var names = new List<string>();

        // Assert
        foreach (var specialty in specialties)
        {
            names.Add(specialty.ToString());
        }

        names.Should().HaveCount(8);
        names.Should().Contain("None");
        names.Should().Contain("Technical");
        names.Should().Contain("Quick");
        names.Should().Contain("Head");
        names.Should().Contain("Powerful");
        names.Should().Contain("Unpredictable");
        names.Should().Contain("Resilient");
        names.Should().Contain("Support");
    }

    #endregion

    #region PlayerPersonality Enum Tests

    [Fact]
    public void PlayerPersonality_HasNiceMember()
    {
        // Assert
        ((PlayerPersonality)Enum.Parse(typeof(PlayerPersonality), "Nice")).Should().Be(PlayerPersonality.Nice);
    }

    [Fact]
    public void PlayerPersonality_HasNastyMember()
    {
        // Assert
        ((PlayerPersonality)Enum.Parse(typeof(PlayerPersonality), "Nasty")).Should().Be(PlayerPersonality.Nasty);
    }

    [Fact]
    public void PlayerPersonality_HasLeaderMember()
    {
        // Assert
        ((PlayerPersonality)Enum.Parse(typeof(PlayerPersonality), "Leader")).Should().Be(PlayerPersonality.Leader);
    }

    [Fact]
    public void PlayerPersonality_HasLonerMember()
    {
        // Assert
        ((PlayerPersonality)Enum.Parse(typeof(PlayerPersonality), "Loner")).Should().Be(PlayerPersonality.Loner);
    }

    [Fact]
    public void PlayerPersonality_HasTemperamentalMember()
    {
        // Assert
        ((PlayerPersonality)Enum.Parse(typeof(PlayerPersonality), "Temperamental")).Should().Be(PlayerPersonality.Temperamental);
    }

    [Fact]
    public void PlayerPersonality_HasCalmMember()
    {
        // Assert
        ((PlayerPersonality)Enum.Parse(typeof(PlayerPersonality), "Calm")).Should().Be(PlayerPersonality.Calm);
    }

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
    public void PlayerPersonality_AllMembers_CanBeEnumerated()
    {
        // Act
        var personalities = Enum.GetValues(typeof(PlayerPersonality)).Cast<PlayerPersonality>().ToList();
        var names = new List<string>();

        // Assert
        foreach (var personality in personalities)
        {
            names.Add(personality.ToString());
        }

        names.Should().HaveCount(6);
        names.Should().Contain("Nice");
        names.Should().Contain("Nasty");
        names.Should().Contain("Leader");
        names.Should().Contain("Loner");
        names.Should().Contain("Temperamental");
        names.Should().Contain("Calm");
    }

    #endregion

    #region CoachType Enum Tests

    [Fact]
    public void CoachType_HasOffensiveMember()
    {
        // Assert
        ((CoachType)Enum.Parse(typeof(CoachType), "Offensive")).Should().Be(CoachType.Offensive);
    }

    [Fact]
    public void CoachType_HasDefensiveMember()
    {
        // Assert
        ((CoachType)Enum.Parse(typeof(CoachType), "Defensive")).Should().Be(CoachType.Defensive);
    }

    [Fact]
    public void CoachType_HasBalancedMember()
    {
        // Assert
        ((CoachType)Enum.Parse(typeof(CoachType), "Balanced")).Should().Be(CoachType.Balanced);
    }

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
    public void CoachType_AllMembers_CanBeEnumerated()
    {
        // Act
        var coachTypes = Enum.GetValues(typeof(CoachType)).Cast<CoachType>().ToList();
        var names = new List<string>();

        // Assert
        foreach (var coachType in coachTypes)
        {
            names.Add(coachType.ToString());
        }

        names.Should().HaveCount(3);
        names.Should().Contain("Offensive");
        names.Should().Contain("Defensive");
        names.Should().Contain("Balanced");
    }

    #endregion

    #region Numeric Value Validation Tests

    [Fact]
    public void Specialty_NoneHasValue0()
    {
        // Assert
        ((int)Specialty.None).Should().Be(0);
    }

    [Fact]
    public void Specialty_TechnicalHasValue1()
    {
        // Assert
        ((int)Specialty.Technical).Should().Be(1);
    }

    [Fact]
    public void Specialty_QuickHasValue2()
    {
        // Assert
        ((int)Specialty.Quick).Should().Be(2);
    }

    [Fact]
    public void Specialty_HeadHasValue3()
    {
        // Assert
        ((int)Specialty.Head).Should().Be(3);
    }

    [Fact]
    public void Specialty_PowerfulHasValue4()
    {
        // Assert
        ((int)Specialty.Powerful).Should().Be(4);
    }

    [Fact]
    public void Specialty_UnpredictableHasValue5()
    {
        // Assert
        ((int)Specialty.Unpredictable).Should().Be(5);
    }

    [Fact]
    public void Specialty_ResilientHasValue6()
    {
        // Assert
        ((int)Specialty.Resilient).Should().Be(6);
    }

    [Fact]
    public void Specialty_SupportHasValue7()
    {
        // Assert
        ((int)Specialty.Support).Should().Be(7);
    }

    [Fact]
    public void PlayerPersonality_NiceHasValue0()
    {
        // Assert
        ((int)PlayerPersonality.Nice).Should().Be(0);
    }

    [Fact]
    public void PlayerPersonality_NastyHasValue1()
    {
        // Assert
        ((int)PlayerPersonality.Nasty).Should().Be(1);
    }

    [Fact]
    public void PlayerPersonality_LeaderHasValue2()
    {
        // Assert
        ((int)PlayerPersonality.Leader).Should().Be(2);
    }

    [Fact]
    public void PlayerPersonality_LonerHasValue3()
    {
        // Assert
        ((int)PlayerPersonality.Loner).Should().Be(3);
    }

    [Fact]
    public void PlayerPersonality_TemperamentalHasValue4()
    {
        // Assert
        ((int)PlayerPersonality.Temperamental).Should().Be(4);
    }

    [Fact]
    public void PlayerPersonality_CalmHasValue5()
    {
        // Assert
        ((int)PlayerPersonality.Calm).Should().Be(5);
    }

    [Fact]
    public void CoachType_OffensiveHasValue0()
    {
        // Assert
        ((int)CoachType.Offensive).Should().Be(0);
    }

    [Fact]
    public void CoachType_DefensiveHasValue1()
    {
        // Assert
        ((int)CoachType.Defensive).Should().Be(1);
    }

    [Fact]
    public void CoachType_BalancedHasValue2()
    {
        // Assert
        ((int)CoachType.Balanced).Should().Be(2);
    }

    #endregion

    #region Invalid Parsing Error Tests

    [Fact]
    public void Specialty_ParsingInvalidMember_Throws()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Enum.Parse(typeof(Specialty), "InvalidMember"));
    }

    [Fact]
    public void PlayerPersonality_ParsingInvalidMember_Throws()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Enum.Parse(typeof(PlayerPersonality), "InvalidMember"));
    }

    [Fact]
    public void CoachType_ParsingInvalidMember_Throws()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Enum.Parse(typeof(CoachType), "InvalidMember"));
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Specialty_And_PlayerPersonality_CanBeUsedTogether()
    {
        // Arrange
        var specialty = Specialty.Technical;
        var personality = PlayerPersonality.Leader;

        // Act
        var specString = specialty.ToString();
        var persString = personality.ToString();

        // Assert
        specialty.Should().Be(Specialty.Technical);
        personality.Should().Be(PlayerPersonality.Leader);
        specString.Should().Be("Technical");
        persString.Should().Be("Leader");
    }

    [Fact]
    public void All_Enums_Can_Be_Converted_To_String_And_Back()
    {
        // Arrange
        var specialties = Enum.GetValues(typeof(Specialty)).Cast<Specialty>().ToList();
        var personalities = Enum.GetValues(typeof(PlayerPersonality)).Cast<PlayerPersonality>().ToList();
        var coachTypes = Enum.GetValues(typeof(CoachType)).Cast<CoachType>().ToList();

        // Act & Assert
        foreach (var specialty in specialties)
        {
            var str = specialty.ToString();
            var parsed = (Specialty)Enum.Parse(typeof(Specialty), str);
            parsed.Should().Be(specialty);
        }

        foreach (var personality in personalities)
        {
            var str = personality.ToString();
            var parsed = (PlayerPersonality)Enum.Parse(typeof(PlayerPersonality), str);
            parsed.Should().Be(personality);
        }

        foreach (var coachType in coachTypes)
        {
            var str = coachType.ToString();
            var parsed = (CoachType)Enum.Parse(typeof(CoachType), str);
            parsed.Should().Be(coachType);
        }
    }

    [Fact]
    public void Specialty_None_IsFirstMember()
    {
        // Act
        var firstMember = (Specialty)0;

        // Assert
        firstMember.Should().Be(Specialty.None);
    }

    [Fact]
    public void PlayerPersonality_AllMembersAreDistinct()
    {
        // Act
        var personalities = Enum.GetValues(typeof(PlayerPersonality)).Cast<PlayerPersonality>().ToList();
        var personalitySet = new HashSet<PlayerPersonality>(personalities);

        // Assert
        personalitySet.Should().HaveCount(6);
    }

    [Fact]
    public void CoachType_AllMembersAreDistinct()
    {
        // Act
        var coachTypes = Enum.GetValues(typeof(CoachType)).Cast<CoachType>().ToList();
        var coachTypeSet = new HashSet<CoachType>(coachTypes);

        // Assert
        coachTypeSet.Should().HaveCount(3);
    }

    [Fact]
    public void Specialty_AllMembersAreDistinct()
    {
        // Act
        var specialties = Enum.GetValues(typeof(Specialty)).Cast<Specialty>().ToList();
        var specialtySet = new HashSet<Specialty>(specialties);

        // Assert
        specialtySet.Should().HaveCount(8);
    }

    #endregion
}
