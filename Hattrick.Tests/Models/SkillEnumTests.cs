using Hattrick.Core.Models;

namespace Hattrick.Tests.Models;

/// <summary>
/// Tests for SkillType and SkillLevel enums and their display names mapping.
/// Verifies enum member existence, correct values, and static display name lookups.
/// </summary>
public class SkillEnumTests
{
    #region SkillType Enum Tests

    [Fact]
    public void SkillType_HasKeeperMember()
    {
        // Assert
        ((SkillType)Enum.Parse(typeof(SkillType), "Keeper")).Should().Be(SkillType.Keeper);
    }

    [Fact]
    public void SkillType_HasDefendingMember()
    {
        // Assert
        ((SkillType)Enum.Parse(typeof(SkillType), "Defending")).Should().Be(SkillType.Defending);
    }

    [Fact]
    public void SkillType_HasPlaymakingMember()
    {
        // Assert
        ((SkillType)Enum.Parse(typeof(SkillType), "Playmaking")).Should().Be(SkillType.Playmaking);
    }

    [Fact]
    public void SkillType_HasWingerMember()
    {
        // Assert
        ((SkillType)Enum.Parse(typeof(SkillType), "Winger")).Should().Be(SkillType.Winger);
    }

    [Fact]
    public void SkillType_HasScoringMember()
    {
        // Assert
        ((SkillType)Enum.Parse(typeof(SkillType), "Scoring")).Should().Be(SkillType.Scoring);
    }

    [Fact]
    public void SkillType_HasPassingMember()
    {
        // Assert
        ((SkillType)Enum.Parse(typeof(SkillType), "Passing")).Should().Be(SkillType.Passing);
    }

    [Fact]
    public void SkillType_HasSetPiecesMember()
    {
        // Assert
        ((SkillType)Enum.Parse(typeof(SkillType), "SetPieces")).Should().Be(SkillType.SetPieces);
    }

    [Fact]
    public void SkillType_HasStaminaMember()
    {
        // Assert
        ((SkillType)Enum.Parse(typeof(SkillType), "Stamina")).Should().Be(SkillType.Stamina);
    }

    [Fact]
    public void SkillType_HasExactlyEightMembers()
    {
        // Act
        var members = Enum.GetValues(typeof(SkillType)).Cast<SkillType>().ToList();

        // Assert
        members.Should().HaveCount(8);
    }

    [Fact]
    public void SkillType_MembersHaveCorrectNames()
    {
        // Act
        var memberNames = Enum.GetNames(typeof(SkillType));

        // Assert
        memberNames.Should().Contain(new[]
        {
            "Keeper", "Defending", "Playmaking", "Winger",
            "Scoring", "Passing", "SetPieces", "Stamina"
        });
    }

    #endregion

    #region SkillLevel Enum Tests

    [Fact]
    public void SkillLevel_HasAllLevelsFrom1To20()
    {
        // Act
        var values = Enum.GetValues(typeof(SkillLevel));
        var intValues = values.Cast<int>().ToList();

        // Assert
        intValues.Should().HaveCount(20);
        intValues.Should().BeInAscendingOrder();
        intValues.Min().Should().Be(1);
        intValues.Max().Should().Be(20);
    }

    [Fact]
    public void SkillLevel_Level1Exists()
    {
        // Act
        var level = (SkillLevel)1;

        // Assert
        level.Should().Be(SkillLevel.Level1);
    }

    [Fact]
    public void SkillLevel_Level10Exists()
    {
        // Act
        var level = (SkillLevel)10;

        // Assert
        level.Should().Be(SkillLevel.Level10);
    }

    [Fact]
    public void SkillLevel_Level20Exists()
    {
        // Act
        var level = (SkillLevel)20;

        // Assert
        level.Should().Be(SkillLevel.Level20);
    }

    [Fact]
    public void SkillLevel_AllLevelsHaveSequentialValues()
    {
        // Act
        var values = Enum.GetValues(typeof(SkillLevel)).Cast<SkillLevel>().ToList();

        // Assert
        for (int i = 0; i < values.Count; i++)
        {
            ((int)values[i]).Should().Be(i + 1);
        }
    }

    #endregion

    #region Display Names Mapping Tests

    [Fact]
    public void GetDisplayName_Level1_ReturnsNonExistent()
    {
        // Act
        var displayName = SkillLevelDisplayNames.GetDisplayName(SkillLevel.Level1);

        // Assert
        displayName.Should().Be("non-existent");
    }

    [Fact]
    public void GetDisplayName_Level2_ReturnsVeryPoor()
    {
        // Act
        var displayName = SkillLevelDisplayNames.GetDisplayName(SkillLevel.Level2);

        // Assert
        displayName.Should().Be("very poor");
    }

    [Fact]
    public void GetDisplayName_Level10_ReturnsMediocre()
    {
        // Act
        var displayName = SkillLevelDisplayNames.GetDisplayName(SkillLevel.Level10);

        // Assert
        displayName.Should().Be("mediocre");
    }

    [Fact]
    public void GetDisplayName_Level20_ReturnsUtopian()
    {
        // Act
        var displayName = SkillLevelDisplayNames.GetDisplayName(SkillLevel.Level20);

        // Assert
        displayName.Should().Be("utopian");
    }

    [Fact]
    public void GetDisplayName_AllLevels_ReturnNonEmptyString()
    {
        // Act & Assert
        for (int level = 1; level <= 20; level++)
        {
            var skillLevel = (SkillLevel)level;
            var displayName = SkillLevelDisplayNames.GetDisplayName(skillLevel);
            displayName.Should().NotBeNullOrEmpty($"Level {level} should have a display name");
        }
    }

    [Fact]
    public void GetDisplayName_AllLevels_ReturnUniqueNames()
    {
        // Act
        var displayNames = new List<string>();
        for (int level = 1; level <= 20; level++)
        {
            var skillLevel = (SkillLevel)level;
            displayNames.Add(SkillLevelDisplayNames.GetDisplayName(skillLevel));
        }

        // Assert
        var uniqueCount = displayNames.Distinct().Count();
        uniqueCount.Should().Be(20, "All 20 levels should have unique display names");
    }

    [Fact]
    public void GetDisplayName_Progression_IsMonotonic()
    {
        // Arrange
        var expectedProgression = new[]
        {
            "non-existent",      // 1
            "very poor",         // 2
            "poor",              // 3
            "below average",     // 4
            "average",           // 5
            "above average",     // 6
            "good",              // 7
            "very good",         // 8
            "excellent",         // 9
            "mediocre",          // 10 (pivot - middle of scale)
            "decent",            // 11
            "competent",         // 12
            "proficient",        // 13
            "accomplished",      // 14
            "formidable",        // 15
            "exceptional",       // 16
            "outstanding",       // 17
            "remarkable",        // 18
            "extraordinary",     // 19
            "utopian"            // 20
        };

        // Act & Assert
        for (int level = 1; level <= 20; level++)
        {
            var skillLevel = (SkillLevel)level;
            var displayName = SkillLevelDisplayNames.GetDisplayName(skillLevel);
            displayName.Should().Be(expectedProgression[level - 1],
                $"Level {level} should have display name '{expectedProgression[level - 1]}'");
        }
    }

    [Fact]
    public void GetDisplayName_LowerLevels_SuggestWeakness()
    {
        // Act
        var level1 = SkillLevelDisplayNames.GetDisplayName(SkillLevel.Level1);
        var level3 = SkillLevelDisplayNames.GetDisplayName(SkillLevel.Level3);

        // Assert
        level1.ToLower().Should().Contain("non");
        level3.ToLower().Should().NotContain("excellent");
    }

    [Fact]
    public void GetDisplayName_UpperLevels_SuggestExcellence()
    {
        // Act
        var level18 = SkillLevelDisplayNames.GetDisplayName(SkillLevel.Level18);
        var level20 = SkillLevelDisplayNames.GetDisplayName(SkillLevel.Level20);

        // Assert
        level18.ToLower().Should().NotContain("poor");
        level20.Should().Be("utopian");
    }

    [Fact]
    public void GetDisplayName_MidpointLevel_ReturnsMediacore()
    {
        // Arrange - Level 10 is the midpoint
        var skillLevel = SkillLevel.Level10;

        // Act
        var displayName = SkillLevelDisplayNames.GetDisplayName(skillLevel);

        // Assert
        displayName.Should().Be("mediocre");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void SkillTypeAndLevel_CanBeUsedTogether()
    {
        // Arrange
        var skillType = SkillType.Keeper;
        var skillLevel = SkillLevel.Level15;

        // Act
        var displayName = SkillLevelDisplayNames.GetDisplayName(skillLevel);

        // Assert
        skillType.Should().Be(SkillType.Keeper);
        skillLevel.Should().Be(SkillLevel.Level15);
        displayName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void AllSkillTypesAndAllSkillLevels_CanBeCreated()
    {
        // Arrange
        var skillTypes = Enum.GetValues(typeof(SkillType)).Cast<SkillType>().ToList();
        var skillLevels = Enum.GetValues(typeof(SkillLevel)).Cast<SkillLevel>().ToList();

        // Act & Assert
        skillTypes.Should().HaveCount(8);
        skillLevels.Should().HaveCount(20);

        // Verify all combinations are valid
        foreach (var type in skillTypes)
        {
            foreach (var level in skillLevels)
            {
                var displayName = SkillLevelDisplayNames.GetDisplayName(level);
                displayName.Should().NotBeNullOrEmpty();
            }
        }
    }

    #endregion
}
