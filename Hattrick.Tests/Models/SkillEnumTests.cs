using Hattrick.Core.Models;

namespace Hattrick.Tests.Models;

/// <summary>
/// Tests for SkillType and SkillLevel enums and their display names mapping.
/// Verifies enum member existence, correct values, explicit integer assignments,
/// correct ordering per game reference, and static display name lookups.
/// </summary>
public class SkillEnumTests
{
    #region SkillType Enum Tests

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
        // Act - GetNames returns names in declaration order
        var memberNames = Enum.GetNames(typeof(SkillType));

        // Assert - Passing must come before Scoring per game reference ordering
        memberNames.Should().HaveCount(8).And.ContainInOrder(new[]
        {
            "Keeper", "Defending", "Playmaking", "Winger",
            "Passing", "Scoring", "SetPieces", "Stamina"
        });
    }

    #region SkillType Explicit Integer Value Tests

    /// <summary>
    /// SkillType members must have explicit integer values to prevent
    /// silent breakage if members are inserted or reordered.
    /// Values match game reference ordering: Passing (4) before Scoring (5).
    /// </summary>
    [Theory]
    [InlineData(SkillType.Keeper, 0)]
    [InlineData(SkillType.Defending, 1)]
    [InlineData(SkillType.Playmaking, 2)]
    [InlineData(SkillType.Winger, 3)]
    [InlineData(SkillType.Passing, 4)]
    [InlineData(SkillType.Scoring, 5)]
    [InlineData(SkillType.SetPieces, 6)]
    [InlineData(SkillType.Stamina, 7)]
    public void SkillType_MemberHasExplicitIntegerValue(SkillType member, int expectedValue)
    {
        // Assert - Each member must map to the correct integer for serialization stability
        ((int)member).Should().Be(expectedValue,
            $"SkillType.{member} must have explicit value {expectedValue} for serialization stability");
    }

    [Fact]
    public void SkillType_PassingComesBeforeScoring()
    {
        // Assert - Game reference ordering: Passing (4) before Scoring (5)
        ((int)SkillType.Passing).Should().BeLessThan((int)SkillType.Scoring,
            "Passing must come before Scoring per game reference ordering");
    }

    [Fact]
    public void SkillType_ValuesAreContiguousFromZero()
    {
        // Act
        var intValues = Enum.GetValues(typeof(SkillType))
            .Cast<int>()
            .OrderBy(v => v)
            .ToList();

        // Assert - Values should be 0, 1, 2, 3, 4, 5, 6, 7
        intValues.Should().BeEquivalentTo(
            Enumerable.Range(0, 8),
            "SkillType values must be contiguous integers starting from 0");
    }

    #endregion

    #endregion

    #region SkillLevel Enum Tests

    [Fact]
    public void SkillLevel_HasNoneSentinelAtZero()
    {
        // Assert - None=0 sentinel must exist so default(SkillLevel) is valid
        var none = (SkillLevel)0;
        none.Should().Be(SkillLevel.None,
            "SkillLevel must have a None=0 member to handle default/uninitialized values");
    }

    [Fact]
    public void SkillLevel_DefaultIsNone()
    {
        // Arrange - default(enum) in C# is always 0
        var defaultLevel = default(SkillLevel);

        // Assert - default(SkillLevel) must be a defined member, not an undefined int 0
        defaultLevel.Should().Be(SkillLevel.None,
            "default(SkillLevel) must equal SkillLevel.None so uninitialized fields are valid");
        Enum.IsDefined(typeof(SkillLevel), defaultLevel).Should().BeTrue(
            "default(SkillLevel) must be a defined enum member");
    }

    [Fact]
    public void SkillLevel_NoneHasIntegerValueZero()
    {
        // Assert
        ((int)SkillLevel.None).Should().Be(0,
            "SkillLevel.None must have integer value 0");
    }

    [Fact]
    public void SkillLevel_HasAllLevelsFrom0To20()
    {
        // Act - Now includes None=0 plus Level1-Level20
        var values = Enum.GetValues(typeof(SkillLevel));
        var intValues = values.Cast<int>().ToList();

        // Assert - 21 members: None(0) + Level1(1) through Level20(20)
        intValues.Should().HaveCount(21);
        intValues.Should().BeInAscendingOrder();
        intValues.Min().Should().Be(0, "minimum should be None=0");
        intValues.Max().Should().Be(20);
    }

    [Fact]
    public void SkillLevel_AllLevelsHaveSequentialValues()
    {
        // Act - Get all values sorted
        var values = Enum.GetValues(typeof(SkillLevel)).Cast<SkillLevel>().ToList();

        // Assert - values should be 0, 1, 2, ..., 20 (None + Level1-Level20)
        for (int i = 0; i < values.Count; i++)
        {
            ((int)values[i]).Should().Be(i,
                $"SkillLevel member at index {i} should have integer value {i}");
        }
    }

    #endregion

    #region Display Names Mapping Tests

    [Fact]
    public void GetDisplayName_None_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var none = SkillLevel.None;

        // Act
        var act = () => SkillLevelDisplayNames.GetDisplayName(none);

        // Assert - None is not a playable skill level, so GetDisplayName should reject it
        act.Should().Throw<ArgumentOutOfRangeException>(
            "SkillLevel.None is not a valid playable level and should not have a display name");
    }

    [Fact]
    public void GetDisplayName_Progression_IsMonotonic()
    {
        // Arrange - Official Hattrick skill level denominations
        var expectedProgression = new[]
        {
            "non-existent",      // 1
            "disastrous",        // 2
            "wretched",          // 3
            "poor",              // 4
            "weak",              // 5
            "inadequate",        // 6
            "passable",          // 7
            "solid",             // 8
            "excellent",         // 9
            "formidable",        // 10
            "outstanding",       // 11
            "brilliant",         // 12
            "magnificent",       // 13
            "world class",       // 14
            "supernatural",      // 15
            "titanic",           // 16
            "extra-terrestrial", // 17
            "mythical",          // 18
            "magical",           // 19
            "utopian (divine)"   // 20
        };

        // Act & Assert
        for (int level = (int)SkillLevel.Level1; level <= (int)SkillLevel.Level20; level++)
        {
            var skillLevel = (SkillLevel)level;
            var displayName = SkillLevelDisplayNames.GetDisplayName(skillLevel);
            displayName.Should().Be(expectedProgression[level - 1],
                $"Level {level} should have display name '{expectedProgression[level - 1]}'");
        }
    }

    [Fact]
    public void GetDisplayName_UndefinedHighValue_ThrowsArgumentOutOfRangeException()
    {
        // Arrange - Cast an arbitrary high integer to SkillLevel that is not a defined member
        var undefined = (SkillLevel)99;

        // Act
        var act = () => SkillLevelDisplayNames.GetDisplayName(undefined);

        // Assert - Undefined values must throw ArgumentOutOfRangeException, not KeyNotFoundException
        act.Should().Throw<ArgumentOutOfRangeException>(
            "undefined SkillLevel values must be rejected with ArgumentOutOfRangeException");
    }

    [Fact]
    public void GetDisplayName_NegativeValue_ThrowsArgumentOutOfRangeException()
    {
        // Arrange - Cast a negative integer to SkillLevel
        var negative = (SkillLevel)(-1);

        // Act
        var act = () => SkillLevelDisplayNames.GetDisplayName(negative);

        // Assert - Negative values must throw ArgumentOutOfRangeException, not KeyNotFoundException
        act.Should().Throw<ArgumentOutOfRangeException>(
            "negative SkillLevel values must be rejected with ArgumentOutOfRangeException");
    }

    [Fact]
    public void GetDisplayName_ValueJustAboveMax_ThrowsArgumentOutOfRangeException()
    {
        // Arrange - Cast 21 to SkillLevel (one above the maximum defined Level20=20)
        var justAboveMax = (SkillLevel)21;

        // Act
        var act = () => SkillLevelDisplayNames.GetDisplayName(justAboveMax);

        // Assert - Off-by-one above max must throw ArgumentOutOfRangeException, not KeyNotFoundException
        act.Should().Throw<ArgumentOutOfRangeException>(
            "SkillLevel value just above the maximum (21) must be rejected with ArgumentOutOfRangeException");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void AllSkillTypesAndAllSkillLevels_CanBeCreated()
    {
        // Arrange
        var skillTypes = Enum.GetValues(typeof(SkillType)).Cast<SkillType>().ToList();
        var skillLevels = Enum.GetValues(typeof(SkillLevel)).Cast<SkillLevel>().ToList();

        // Act & Assert - 8 skill types, 21 skill levels (None + Level1-Level20)
        skillTypes.Should().HaveCount(8);
        skillLevels.Should().HaveCount(21);

        // Verify all combinations of type + playable level are valid
        foreach (var type in skillTypes)
        {
            foreach (var level in skillLevels.Where(l => l != SkillLevel.None))
            {
                var displayName = SkillLevelDisplayNames.GetDisplayName(level);
                displayName.Should().NotBeNullOrEmpty();
            }
        }
    }

    #endregion
}
