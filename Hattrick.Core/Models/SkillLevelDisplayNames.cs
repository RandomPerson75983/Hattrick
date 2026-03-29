using System.Collections.Frozen;

namespace Hattrick.Core.Models;

/// <summary>
/// Static class providing display name mappings for skill levels.
/// Maps each SkillLevel (1-20) to a descriptive string progression.
/// </summary>
public static class SkillLevelDisplayNames
{
    private static readonly FrozenDictionary<SkillLevel, string> DisplayNames = new Dictionary<SkillLevel, string>
    {
        { SkillLevel.Level1, "non-existent" },
        { SkillLevel.Level2, "disastrous" },
        { SkillLevel.Level3, "wretched" },
        { SkillLevel.Level4, "poor" },
        { SkillLevel.Level5, "weak" },
        { SkillLevel.Level6, "inadequate" },
        { SkillLevel.Level7, "passable" },
        { SkillLevel.Level8, "solid" },
        { SkillLevel.Level9, "excellent" },
        { SkillLevel.Level10, "formidable" },
        { SkillLevel.Level11, "outstanding" },
        { SkillLevel.Level12, "brilliant" },
        { SkillLevel.Level13, "magnificent" },
        { SkillLevel.Level14, "world class" },
        { SkillLevel.Level15, "supernatural" },
        { SkillLevel.Level16, "titanic" },
        { SkillLevel.Level17, "extra-terrestrial" },
        { SkillLevel.Level18, "mythical" },
        { SkillLevel.Level19, "magical" },
        { SkillLevel.Level20, "utopian (divine)" }
    }.ToFrozenDictionary();

    /// <summary>
    /// Gets the display name for a given skill level.
    /// </summary>
    /// <param name="level">The skill level (must be Level1-Level20, not None).</param>
    /// <returns>The display name string for the level.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when level is None or undefined.</exception>
    public static string GetDisplayName(SkillLevel level)
    {
        if (!DisplayNames.TryGetValue(level, out var displayName))
        {
            throw new ArgumentOutOfRangeException(nameof(level), level,
                $"SkillLevel value '{level}' is not a valid playable level and does not have a display name.");
        }

        return displayName;
    }
}
