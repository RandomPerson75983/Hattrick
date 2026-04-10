using Hattrick.Core.Models;

namespace Hattrick.Core.Services;

/// <summary>
/// Provides formatting and display helpers for player data.
/// All methods are pure functions with no side effects.
/// </summary>
public interface IPlayerDisplayService
{
    /// <summary>
    /// Returns the player's age formatted as "{age} years and {ageDays} days".
    /// No pluralization — always "days" regardless of the value.
    /// </summary>
    string FormatAge(int age, int ageDays);

    /// <summary>
    /// Returns the integer floor of a skill value, e.g. 7.73 → 7.
    /// </summary>
    int GetSkillFloor(double skill);

    /// <summary>
    /// Returns the fractional part of a skill value as a 0-100 integer percentage,
    /// e.g. 7.73 → 73. Truncates and clamps to [0, 100]; does not round.
    /// </summary>
    int GetSkillBarPercent(double skill);

    /// <summary>
    /// Returns the CSS class name for a skill bar based on the skill's floor value.
    /// Ranges: 0-4 → "skill-poor", 5 → "skill-inadequate", 6 → "skill-passable",
    /// 7-8 → "skill-good", 9-10 → "skill-excellent", 11+ → "skill-outstanding".
    /// </summary>
    string GetSkillColorClass(double skill);

    /// <summary>
    /// Returns the human-readable display name for a <see cref="Position"/>.
    /// </summary>
    string GetPositionDisplay(Position position);

    /// <summary>
    /// Returns the human-readable display name for a <see cref="Specialty"/>.
    /// Returns an empty string for <see cref="Specialty.None"/>.
    /// </summary>
    string GetSpecialtyDisplay(Specialty specialty);

    /// <summary>Returns the ordered list of skill types shown in the player table (excludes Stamina).</summary>
    IReadOnlyList<SkillType> GetDisplayedSkillTypes();
}
