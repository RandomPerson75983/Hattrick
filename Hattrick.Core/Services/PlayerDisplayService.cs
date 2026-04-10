using Hattrick.Core.Models;

namespace Hattrick.Core.Services;

/// <summary>
/// Pure formatting helpers for displaying player data in the UI.
/// No state, no dependencies — safe to register as singleton.
/// </summary>
public sealed class PlayerDisplayService : IPlayerDisplayService
{
    private static readonly IReadOnlyList<SkillType> _displayedSkillTypes = new[]
    {
        SkillType.Keeper, SkillType.Defending, SkillType.Playmaking,
        SkillType.Winger, SkillType.Scoring, SkillType.Passing, SkillType.SetPieces
    }.AsReadOnly();

    private const int SkillInadequateFloor = 5;
    private const int SkillPassableFloor = 6;
    private const int SkillGoodCeiling = 8;
    private const int SkillExcellentCeiling = 10;
    private const int SkillBarPercentMultiplier = 100;

    /// <inheritdoc/>
    public string FormatAge(int age, int ageDays) =>
        $"{age} years and {ageDays} days";

    /// <inheritdoc/>
    public int GetSkillFloor(double skill) => (int)Math.Floor(skill);

    /// <inheritdoc/>
    public int GetSkillBarPercent(double skill) =>
        Math.Clamp((int)((skill % 1) * SkillBarPercentMultiplier), 0, SkillBarPercentMultiplier);

    /// <inheritdoc/>
    public string GetSkillColorClass(double skill)
    {
        var floor = GetSkillFloor(skill);

        return floor switch
        {
            < SkillInadequateFloor => "skill-poor",
            SkillInadequateFloor => "skill-inadequate",
            SkillPassableFloor => "skill-passable",
            <= SkillGoodCeiling => "skill-good",
            <= SkillExcellentCeiling => "skill-excellent",
            _ => "skill-outstanding"
        };
    }

    /// <inheritdoc/>
    public string GetPositionDisplay(Position position) => position switch
    {
        Position.Keeper => "Keeper",
        Position.CentralDefender => "Central Defender",
        Position.WingBack => "Wing Back",
        Position.InnerMidfielder => "Inner Midfielder",
        Position.Winger => "Winger",
        Position.Forward => "Forward",
        _ => position.ToString()
    };

    /// <inheritdoc/>
    public IReadOnlyList<SkillType> GetDisplayedSkillTypes() => _displayedSkillTypes;

    /// <inheritdoc/>
    public string GetSpecialtyDisplay(Specialty specialty) => specialty switch
    {
        Specialty.None => string.Empty,
        Specialty.Technical => "Technical",
        Specialty.Quick => "Quick",
        Specialty.Head => "Head",
        Specialty.Powerful => "Powerful",
        Specialty.Unpredictable => "Unpredictable",
        Specialty.Resilient => "Resilient",
        Specialty.Support => "Support",
        _ => specialty.ToString()
    };
}
