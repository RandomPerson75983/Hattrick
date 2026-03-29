namespace Hattrick.Core.Models;

/// <summary>
/// Static class providing display name mappings for skill levels.
/// Maps each SkillLevel (1-20) to a descriptive string progression.
/// </summary>
public static class SkillLevelDisplayNames
{
    private static readonly Dictionary<SkillLevel, string> DisplayNames = new()
    {
        { SkillLevel.Level1, "non-existent" },
        { SkillLevel.Level2, "very poor" },
        { SkillLevel.Level3, "poor" },
        { SkillLevel.Level4, "below average" },
        { SkillLevel.Level5, "average" },
        { SkillLevel.Level6, "above average" },
        { SkillLevel.Level7, "good" },
        { SkillLevel.Level8, "very good" },
        { SkillLevel.Level9, "excellent" },
        { SkillLevel.Level10, "mediocre" },
        { SkillLevel.Level11, "decent" },
        { SkillLevel.Level12, "competent" },
        { SkillLevel.Level13, "proficient" },
        { SkillLevel.Level14, "accomplished" },
        { SkillLevel.Level15, "formidable" },
        { SkillLevel.Level16, "exceptional" },
        { SkillLevel.Level17, "outstanding" },
        { SkillLevel.Level18, "remarkable" },
        { SkillLevel.Level19, "extraordinary" },
        { SkillLevel.Level20, "utopian" }
    };

    /// <summary>
    /// Gets the display name for a given skill level.
    /// </summary>
    /// <param name="level">The skill level.</param>
    /// <returns>The display name string for the level.</returns>
    public static string GetDisplayName(SkillLevel level)
    {
        return DisplayNames[level];
    }
}
