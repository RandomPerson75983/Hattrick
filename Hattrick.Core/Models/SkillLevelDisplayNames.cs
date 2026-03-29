using System.Collections.Frozen;

namespace Hattrick.Core.Models;

/// <summary>
/// Static class providing display name mappings for skill levels.
/// Maps each SkillLevel (0-20) to its official Hattrick denomination string.
/// </summary>
public static class SkillLevelDisplayNames
{
    private static readonly FrozenDictionary<SkillLevel, string> DisplayNames = new Dictionary<SkillLevel, string>
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
    }.ToFrozenDictionary();

    /// <summary>
    /// Gets the display name for a given skill level.
    /// </summary>
    /// <param name="level">The skill level (0-20).</param>
    /// <returns>The display name string for the level.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when level is not a defined SkillLevel.</exception>
    public static string GetDisplayName(SkillLevel level)
    {
        if (!DisplayNames.TryGetValue(level, out var displayName))
        {
            throw new ArgumentOutOfRangeException(nameof(level), level,
                $"SkillLevel value '{level}' is not a defined skill level.");
        }

        return displayName;
    }
}
