namespace Hattrick.Core.Models;

/// <summary>
/// Represents a football player in the game.
/// Mutable model class since player state changes during gameplay
/// (training, aging, injuries, cards, form, etc.).
/// </summary>
public class Player
{
    /// <summary>
    /// Unique identifier for the player. Auto-generated on construction.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The team this player belongs to. Empty Guid if unassigned.
    /// </summary>
    public Guid TeamId { get; set; }

    /// <summary>
    /// Player's full name (first + last).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Player's age in years. Minimum is 17 in Hattrick.
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Days within the current age year (0-111, Hattrick uses 112-day seasons).
    /// </summary>
    public int AgeDays { get; set; }

    /// <summary>
    /// Player skills with sub-level precision (e.g., 7.73 means level 7, 73% toward level 8).
    /// Keys are SkillType enum values, values are doubles for sub-level tracking.
    /// </summary>
    public Dictionary<SkillType, double> Skills { get; set; } = new Dictionary<SkillType, double>();

    /// <summary>
    /// Player specialty (e.g., Technical, Quick, Head). None if no specialty.
    /// </summary>
    public Specialty Specialty { get; set; }

    /// <summary>
    /// Current form level (1-8).
    /// </summary>
    public int Form { get; set; } = 1;

    /// <summary>
    /// Current stamina level (1-9).
    /// </summary>
    public int Stamina { get; set; } = 1;

    /// <summary>
    /// Experience level (1-20).
    /// </summary>
    public int Experience { get; set; } = 1;

    /// <summary>
    /// Loyalty to current club (0.0-1.0).
    /// </summary>
    public double Loyalty { get; set; }

    /// <summary>
    /// Whether this player was raised at the current club (mother club bonus).
    /// </summary>
    public bool IsMotherClub { get; set; }

    /// <summary>
    /// Total Skill Index - an aggregate measure of player quality.
    /// </summary>
    public int TSI { get; set; }

    /// <summary>
    /// Weeks of injury remaining. 0 means the player is healthy.
    /// </summary>
    public int InjuryWeeks { get; set; }

    /// <summary>
    /// Whether the player currently has a red card suspension.
    /// </summary>
    public bool RedCard { get; set; }

    /// <summary>
    /// Number of accumulated yellow cards (0-3). At 3, player is suspended next match.
    /// Note: 2 yellows in a single match = red card (different rule).
    /// </summary>
    public int YellowCards { get; set; }

    /// <summary>
    /// Player's jersey/shirt number.
    /// </summary>
    public int JerseyNumber { get; set; }

    /// <summary>
    /// Weekly wage in currency units. Decimal for precise financial tracking.
    /// </summary>
    public decimal Wage { get; set; }

    /// <summary>
    /// Player personality type affecting team dynamics.
    /// </summary>
    public PlayerPersonality Personality { get; set; }

    /// <summary>
    /// Leadership ability (1-8). Affects team spirit when player is captain.
    /// </summary>
    public int Leadership { get; set; } = 1;

    /// <summary>
    /// Hattrick Manager Score - an estimated total skill value.
    /// </summary>
    public int HTMS { get; set; }

    /// <summary>
    /// Player's potential rating.
    /// </summary>
    public int Potential { get; set; }

    /// <summary>
    /// The position where this player performs best.
    /// </summary>
    public Position BestPosition { get; set; }

    /// <summary>
    /// ID of the player's native country.
    /// </summary>
    public int NativeCountryId { get; set; }
}
