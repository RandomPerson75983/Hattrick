using Hattrick.Core.Models;

namespace Hattrick.Tests.Models;

/// <summary>
/// Tests for SkillType and SkillLevel enums and their display names mapping.
/// </summary>
public class SkillEnumTests
{
    #region SkillType Enum Tests

    [Fact]
    public void SkillType_HasExactlyEightMembers()
    {
        var members = Enum.GetValues(typeof(SkillType)).Cast<SkillType>().ToList();
        members.Should().HaveCount(8);
    }

    [Fact]
    public void SkillType_MembersHaveCorrectNames()
    {
        var memberNames = Enum.GetNames(typeof(SkillType));
        memberNames.Should().HaveCount(8).And.ContainInOrder(new[]
        {
            "Keeper", "Defending", "Playmaking", "Winger",
            "Passing", "Scoring", "SetPieces", "Stamina"
        });
    }

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
        ((int)member).Should().Be(expectedValue,
            $"SkillType.{member} must have explicit value {expectedValue} for serialization stability");
    }

    [Fact]
    public void SkillType_PassingComesBeforeScoring()
    {
        ((int)SkillType.Passing).Should().BeLessThan((int)SkillType.Scoring,
            "Passing must come before Scoring per game reference ordering");
    }

    [Fact]
    public void SkillType_ValuesAreContiguousFromZero()
    {
        var intValues = Enum.GetValues(typeof(SkillType))
            .Cast<int>()
            .OrderBy(v => v)
            .ToList();

        intValues.Should().BeEquivalentTo(
            Enumerable.Range(0, 8),
            "SkillType values must be contiguous integers starting from 0");
    }

    #endregion

    #region SkillLevel Enum Tests

    [Fact]
    public void SkillLevel_Has21Members()
    {
        var values = Enum.GetValues(typeof(SkillLevel)).Cast<SkillLevel>().ToList();
        values.Should().HaveCount(21);
    }

    [Fact]
    public void SkillLevel_MembersHaveCorrectNames()
    {
        var memberNames = Enum.GetNames(typeof(SkillLevel));
        memberNames.Should().HaveCount(21).And.ContainInOrder(new[]
        {
            "NonExistent", "Disastrous", "Wretched", "Poor", "Weak",
            "Inadequate", "Passable", "Solid", "Excellent", "Formidable",
            "Outstanding", "Brilliant", "Magnificent", "WorldClass",
            "Supernatural", "Titanic", "ExtraTerrestrial", "Mythical",
            "Magical", "Utopian", "Divine"
        });
    }

    [Fact]
    public void SkillLevel_ValuesAreSequentialFrom0To20()
    {
        var values = Enum.GetValues(typeof(SkillLevel)).Cast<int>().OrderBy(v => v).ToList();
        values.Should().BeEquivalentTo(Enumerable.Range(0, 21));
    }

    [Fact]
    public void SkillLevel_DefaultIsNonExistent()
    {
        var defaultLevel = default(SkillLevel);
        defaultLevel.Should().Be(SkillLevel.NonExistent);
        Enum.IsDefined(typeof(SkillLevel), defaultLevel).Should().BeTrue();
    }

    #endregion

    #region Display Names Mapping Tests

    [Fact]
    public void GetDisplayName_AllLevels_MatchOfficialDenominations()
    {
        var expected = new Dictionary<SkillLevel, string>
        {
            { SkillLevel.NonExistent, "non-existent" },
            { SkillLevel.Disastrous, "disastrous" },
            { SkillLevel.Wretched, "wretched" },
            { SkillLevel.Poor, "poor" },
            { SkillLevel.Weak, "weak" },
            { SkillLevel.Inadequate, "inadequate" },
            { SkillLevel.Passable, "passable" },
            { SkillLevel.Solid, "solid" },
            { SkillLevel.Excellent, "excellent" },
            { SkillLevel.Formidable, "formidable" },
            { SkillLevel.Outstanding, "outstanding" },
            { SkillLevel.Brilliant, "brilliant" },
            { SkillLevel.Magnificent, "magnificent" },
            { SkillLevel.WorldClass, "world class" },
            { SkillLevel.Supernatural, "supernatural" },
            { SkillLevel.Titanic, "titanic" },
            { SkillLevel.ExtraTerrestrial, "extra-terrestrial" },
            { SkillLevel.Mythical, "mythical" },
            { SkillLevel.Magical, "magical" },
            { SkillLevel.Utopian, "utopian" },
            { SkillLevel.Divine, "divine" }
        };

        foreach (var (level, expectedName) in expected)
        {
            SkillLevelDisplayNames.GetDisplayName(level).Should().Be(expectedName,
                $"SkillLevel.{level} should map to '{expectedName}'");
        }
    }

    [Fact]
    public void GetDisplayName_UndefinedValue_ThrowsArgumentOutOfRangeException()
    {
        var act = () => SkillLevelDisplayNames.GetDisplayName((SkillLevel)99);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetDisplayName_NegativeValue_ThrowsArgumentOutOfRangeException()
    {
        var act = () => SkillLevelDisplayNames.GetDisplayName((SkillLevel)(-1));
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetDisplayName_ValueJustAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var act = () => SkillLevelDisplayNames.GetDisplayName((SkillLevel)21);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void AllSkillTypesAndAllSkillLevels_CanBeCreated()
    {
        var skillTypes = Enum.GetValues(typeof(SkillType)).Cast<SkillType>().ToList();
        var skillLevels = Enum.GetValues(typeof(SkillLevel)).Cast<SkillLevel>().ToList();

        skillTypes.Should().HaveCount(8);
        skillLevels.Should().HaveCount(21);

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
