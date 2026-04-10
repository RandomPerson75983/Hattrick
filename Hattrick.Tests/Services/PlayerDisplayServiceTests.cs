using Hattrick.Core.Models;
using Hattrick.Core.Services;

namespace Hattrick.Tests.Services;

/// <summary>
/// Unit tests for IPlayerDisplayService.
///
/// PlayerDisplayService provides pure formatting and display helpers used by the Players page.
/// All methods are stateless — no dependencies needed, service is instantiated directly.
/// </summary>
public class PlayerDisplayServiceTests
{
    private readonly IPlayerDisplayService _sut = new PlayerDisplayService();

    // ─────────────────────────────────────────────────────────────
    // FormatAge
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void FormatAge_AgeWithDays_ReturnsCorrectString()
    {
        var result = _sut.FormatAge(23, 72);

        result.Should().Be("23 years and 72 days");
    }

    [Fact]
    public void FormatAge_MinimumAge_ReturnsCorrectString()
    {
        // Youngest possible Hattrick player: 17 years, 0 days
        var result = _sut.FormatAge(17, 0);

        result.Should().Be("17 years and 0 days");
    }

    [Fact]
    public void FormatAge_MaxAgeDays_ReturnsCorrectString()
    {
        // Max day within a season year is 111 (0-based, 112-day seasons)
        var result = _sut.FormatAge(23, 111);

        result.Should().Be("23 years and 111 days");
    }

    [Fact]
    public void FormatAge_OlderPlayer_ReturnsCorrectString()
    {
        var result = _sut.FormatAge(35, 45);

        result.Should().Be("35 years and 45 days");
    }

    [Fact]
    public void FormatAge_OneDayIn_ReturnsCorrectString()
    {
        var result = _sut.FormatAge(20, 1);

        result.Should().Be("20 years and 1 days");
    }

    // ─────────────────────────────────────────────────────────────
    // GetSkillFloor
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void GetSkillFloor_Zero_ReturnsZero()
    {
        var result = _sut.GetSkillFloor(0.0);

        result.Should().Be(0);
    }

    [Fact]
    public void GetSkillFloor_SkillWithFraction_ReturnsIntegerPart()
    {
        // 7.73 → 7
        var result = _sut.GetSkillFloor(7.73);

        result.Should().Be(7);
    }

    [Fact]
    public void GetSkillFloor_ExactInteger_ReturnsThatInteger()
    {
        var result = _sut.GetSkillFloor(8.0);

        result.Should().Be(8);
    }

    [Fact]
    public void GetSkillFloor_NearlyNextLevel_ReturnsCurrentLevel()
    {
        // 6.99 is still floor 6, not 7
        var result = _sut.GetSkillFloor(6.99);

        result.Should().Be(6);
    }

    [Fact]
    public void GetSkillFloor_LowSkill_ReturnsCorrectFloor()
    {
        var result = _sut.GetSkillFloor(4.99);

        result.Should().Be(4);
    }

    [Fact]
    public void GetSkillFloor_MaxSkillRange_ReturnsCorrectFloor()
    {
        var result = _sut.GetSkillFloor(20.0);

        result.Should().Be(20);
    }

    // ─────────────────────────────────────────────────────────────
    // GetSkillBarPercent
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void GetSkillBarPercent_Zero_ReturnsZero()
    {
        // 0.0 → fractional part = 0 → 0%
        var result = _sut.GetSkillBarPercent(0.0);

        result.Should().Be(0);
    }

    [Fact]
    public void GetSkillBarPercent_SkillWithFraction_ReturnsPercentage()
    {
        // 7.73 → fractional part = 0.73 → 73%
        var result = _sut.GetSkillBarPercent(7.73);

        result.Should().Be(73);
    }

    [Fact]
    public void GetSkillBarPercent_ExactInteger_ReturnsZero()
    {
        // 8.0 → fractional part = 0 → 0%
        var result = _sut.GetSkillBarPercent(8.0);

        result.Should().Be(0);
    }

    [Fact]
    public void GetSkillBarPercent_HalfwayToNextLevel_ReturnsFiftyPercent()
    {
        // 5.50 → fractional part = 0.50 → 50%
        var result = _sut.GetSkillBarPercent(5.50);

        result.Should().Be(50);
    }

    [Fact]
    public void GetSkillBarPercent_NearlyAtNextLevel_ReturnsHighPercent()
    {
        // 6.99 → fractional part = 0.99 → 99%
        var result = _sut.GetSkillBarPercent(6.99);

        result.Should().Be(99);
    }

    [Fact]
    public void GetSkillBarPercent_OnePercentIn_ReturnsOne()
    {
        // 3.014 → fractional part = 0.014 → 0.014 * 100 = 1.4 → truncates to 1
        // (3.01 cannot be used: IEEE 754 represents it slightly below 3.01,
        // so (3.01 % 1) * 100 ≈ 0.9999..., which truncates to 0, not 1.)
        var result = _sut.GetSkillBarPercent(3.014);

        result.Should().Be(1);
    }

    [Fact]
    public void GetSkillBarPercent_ExactlyFive_ReturnsZero()
    {
        // 5.0 → fractional part = 0 → 0%
        _sut.GetSkillBarPercent(5.0).Should().Be(0);
    }

    [Fact]
    public void GetSkillBarPercent_SkillNearMidpoint_TruncatesNotRounds()
    {
        // Contract says "Truncates; does not round."
        // skill = 7.995 → fractional part = 0.995 → 0.995 * 100 = 99.5
        // Truncation: (int)99.5 = 99
        // Math.Round(99.5) = 100 (rounds up, or banker's rounding may also give 100)
        // Expected: 99 (truncation), NOT 100 (rounding)
        _sut.GetSkillBarPercent(7.995).Should().Be(99);
    }

    [Fact]
    public void GetSkillBarPercent_WithNegativeSkill_ReturnsZero()
    {
        // Negative skill is invalid; CSS width must never be negative.
        // Current impl: (int)Math.Round((-1.3 % 1) * 100) → (int)Math.Round(-0.3 * 100) → -30
        // Expected: 0 (clamped to valid range)
        _sut.GetSkillBarPercent(-1.3).Should().Be(0);
    }

    // ─────────────────────────────────────────────────────────────
    // GetSkillColorClass
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void GetSkillColorClass_ZeroSkill_ReturnsPoor()
    {
        // Floor 0 → "skill-poor"
        var result = _sut.GetSkillColorClass(0.0);

        result.Should().Be("skill-poor");
    }

    [Fact]
    public void GetSkillColorClass_FloorFour_ReturnsPoor()
    {
        // Floor 4 (range 0-4) → "skill-poor"
        var result = _sut.GetSkillColorClass(4.99);

        result.Should().Be("skill-poor");
    }

    [Fact]
    public void GetSkillColorClass_FloorFive_ReturnsInadequate()
    {
        // Floor 5 (mid-bucket) → "skill-inadequate"
        var result = _sut.GetSkillColorClass(5.5);

        result.Should().Be("skill-inadequate");
    }

    [Fact]
    public void GetSkillColorClass_FloorFiveWithFraction_ReturnsInadequate()
    {
        // Floor 5 still at 5.99 → "skill-inadequate"
        var result = _sut.GetSkillColorClass(5.99);

        result.Should().Be("skill-inadequate");
    }

    [Fact]
    public void GetSkillColorClass_FloorSix_ReturnsPassable()
    {
        // Floor 6 (mid-bucket) → "skill-passable"
        var result = _sut.GetSkillColorClass(6.5);

        result.Should().Be("skill-passable");
    }

    [Fact]
    public void GetSkillColorClass_FloorSixWithFraction_ReturnsPassable()
    {
        // 6.99 floor = 6 → "skill-passable"
        var result = _sut.GetSkillColorClass(6.99);

        result.Should().Be("skill-passable");
    }

    [Fact]
    public void GetSkillColorClass_FloorSeven_ReturnsGood()
    {
        // Floor 7 (mid-bucket) → "skill-good"
        var result = _sut.GetSkillColorClass(7.5);

        result.Should().Be("skill-good");
    }

    [Fact]
    public void GetSkillColorClass_FloorEight_ReturnsGood()
    {
        // Floor 8 (range 7-8) → "skill-good"
        var result = _sut.GetSkillColorClass(8.99);

        result.Should().Be("skill-good");
    }

    [Fact]
    public void GetSkillColorClass_FloorNine_ReturnsExcellent()
    {
        // Floor 9 (mid-bucket) → "skill-excellent"
        var result = _sut.GetSkillColorClass(9.5);

        result.Should().Be("skill-excellent");
    }

    [Fact]
    public void GetSkillColorClass_FloorTen_ReturnsExcellent()
    {
        // Floor 10 (range 9-10) → "skill-excellent"
        var result = _sut.GetSkillColorClass(10.99);

        result.Should().Be("skill-excellent");
    }

    [Fact]
    public void GetSkillColorClass_FloorEleven_ReturnsOutstanding()
    {
        // Floor 12 (mid-bucket above 11) → "skill-outstanding"
        var result = _sut.GetSkillColorClass(12.0);

        result.Should().Be("skill-outstanding");
    }

    [Fact]
    public void GetSkillColorClass_VeryHighSkill_ReturnsOutstanding()
    {
        // Very high skill (e.g. 20) → "skill-outstanding"
        var result = _sut.GetSkillColorClass(20.0);

        result.Should().Be("skill-outstanding");
    }

    // Boundary: exactly at the transition from "skill-poor" (4) to "skill-inadequate" (5)
    [Fact]
    public void GetSkillColorClass_ExactlyAtFive_ReturnsInadequate()
    {
        var result = _sut.GetSkillColorClass(5.0);

        result.Should().Be("skill-inadequate");
    }

    // Boundary: exactly at the transition from "skill-inadequate" (5) to "skill-passable" (6)
    [Fact]
    public void GetSkillColorClass_ExactlyAtSix_ReturnsPassable()
    {
        var result = _sut.GetSkillColorClass(6.0);

        result.Should().Be("skill-passable");
    }

    // Boundary: exactly at the transition from "skill-passable" (6) to "skill-good" (7)
    [Fact]
    public void GetSkillColorClass_ExactlyAtSeven_ReturnsGood()
    {
        var result = _sut.GetSkillColorClass(7.0);

        result.Should().Be("skill-good");
    }

    // Boundary: exactly at the skill=8.0 point (floor=8, SkillGoodCeiling=8 → still "skill-good")
    [Fact]
    public void GetSkillColorClass_ExactlyAtEight_ReturnsGood()
    {
        var result = _sut.GetSkillColorClass(8.0);

        result.Should().Be("skill-good");
    }

    // Boundary: exactly at the transition from "skill-good" (8) to "skill-excellent" (9)
    [Fact]
    public void GetSkillColorClass_ExactlyAtNine_ReturnsExcellent()
    {
        var result = _sut.GetSkillColorClass(9.0);

        result.Should().Be("skill-excellent");
    }

    // Boundary: exactly at the transition from "skill-excellent" (10) to "skill-outstanding" (11)
    [Fact]
    public void GetSkillColorClass_ExactlyAtEleven_ReturnsOutstanding()
    {
        var result = _sut.GetSkillColorClass(11.0);

        result.Should().Be("skill-outstanding");
    }

    // ─────────────────────────────────────────────────────────────
    // GetPositionDisplay
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void GetPositionDisplay_Keeper_ReturnsKeeper()
    {
        var result = _sut.GetPositionDisplay(Position.Keeper);

        result.Should().Be("Keeper");
    }

    [Fact]
    public void GetPositionDisplay_CentralDefender_ReturnsCentralDefender()
    {
        var result = _sut.GetPositionDisplay(Position.CentralDefender);

        result.Should().Be("Central Defender");
    }

    [Fact]
    public void GetPositionDisplay_WingBack_ReturnsWingBack()
    {
        var result = _sut.GetPositionDisplay(Position.WingBack);

        result.Should().Be("Wing Back");
    }

    [Fact]
    public void GetPositionDisplay_InnerMidfielder_ReturnsInnerMidfielder()
    {
        var result = _sut.GetPositionDisplay(Position.InnerMidfielder);

        result.Should().Be("Inner Midfielder");
    }

    [Fact]
    public void GetPositionDisplay_Winger_ReturnsWinger()
    {
        var result = _sut.GetPositionDisplay(Position.Winger);

        result.Should().Be("Winger");
    }

    [Fact]
    public void GetPositionDisplay_Forward_ReturnsForward()
    {
        var result = _sut.GetPositionDisplay(Position.Forward);

        result.Should().Be("Forward");
    }

    // ─────────────────────────────────────────────────────────────
    // GetSpecialtyDisplay
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void GetSpecialtyDisplay_None_ReturnsEmptyString()
    {
        var result = _sut.GetSpecialtyDisplay(Specialty.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetSpecialtyDisplay_Technical_ReturnsTechnical()
    {
        var result = _sut.GetSpecialtyDisplay(Specialty.Technical);

        result.Should().Be("Technical");
    }

    [Fact]
    public void GetSpecialtyDisplay_Quick_ReturnsQuick()
    {
        var result = _sut.GetSpecialtyDisplay(Specialty.Quick);

        result.Should().Be("Quick");
    }

    [Fact]
    public void GetSpecialtyDisplay_Head_ReturnsHead()
    {
        var result = _sut.GetSpecialtyDisplay(Specialty.Head);

        result.Should().Be("Head");
    }

    [Fact]
    public void GetSpecialtyDisplay_Powerful_ReturnsPowerful()
    {
        var result = _sut.GetSpecialtyDisplay(Specialty.Powerful);

        result.Should().Be("Powerful");
    }

    [Fact]
    public void GetSpecialtyDisplay_Unpredictable_ReturnsUnpredictable()
    {
        var result = _sut.GetSpecialtyDisplay(Specialty.Unpredictable);

        result.Should().Be("Unpredictable");
    }

    [Fact]
    public void GetSpecialtyDisplay_Resilient_ReturnsResilient()
    {
        var result = _sut.GetSpecialtyDisplay(Specialty.Resilient);

        result.Should().Be("Resilient");
    }

    [Fact]
    public void GetSpecialtyDisplay_Support_ReturnsSupport()
    {
        var result = _sut.GetSpecialtyDisplay(Specialty.Support);

        result.Should().Be("Support");
    }

    // ─────────────────────────────────────────────────────────────
    // GetDisplayedSkillTypes
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void GetDisplayedSkillTypes_WhenCalled_ReturnsSevenSkills()
    {
        // Domain rule: exactly 7 field-player skills are displayed in the roster table
        // (Keeper, Defending, Playmaking, Winger, Scoring, Passing, SetPieces).
        var result = _sut.GetDisplayedSkillTypes();

        result.Count.Should().Be(7);
    }

    [Fact]
    public void GetDisplayedSkillTypes_WhenCalled_ContainsAllExpectedSkillTypes()
    {
        // All seven displayed skill columns must be present.
        var result = _sut.GetDisplayedSkillTypes();

        result.Should().Contain(SkillType.Keeper);
        result.Should().Contain(SkillType.Defending);
        result.Should().Contain(SkillType.Playmaking);
        result.Should().Contain(SkillType.Winger);
        result.Should().Contain(SkillType.Scoring);
        result.Should().Contain(SkillType.Passing);
        result.Should().Contain(SkillType.SetPieces);
    }

    [Fact]
    public void GetDisplayedSkillTypes_WhenCalled_DoesNotContainStamina()
    {
        // Stamina is shown in the sidebar averages, NOT as a skill column.
        var result = _sut.GetDisplayedSkillTypes();

        result.Should().NotContain(SkillType.Stamina);
    }

    [Fact]
    public void GetDisplayedSkillTypes_WhenCalled_ReturnsIReadOnlyList()
    {
        // Callers may only read the list; mutation is not permitted.
        _sut.GetDisplayedSkillTypes().Should().BeAssignableTo<IReadOnlyList<SkillType>>();
    }
}
