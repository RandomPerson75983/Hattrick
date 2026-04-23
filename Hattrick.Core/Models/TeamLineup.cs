namespace Hattrick.Core.Models;

/// <summary>
/// Represents a team's lineup configuration for a match.
/// Mutable class since lineup can be edited before match submission.
/// Phase 2 Sprint 2, Quartet 2 (TeamLineup Model).
///
/// This model holds formation, tactics, attitude, player slots, and special role assignments.
/// Validation logic belongs in LineupService, not in this model.
/// </summary>
public class TeamLineup
{
    /// <summary>
    /// Number of starting players required for a match.
    /// </summary>
    public const int StarterCount = 11;

    /// <summary>
    /// Maximum number of substitutes allowed on the bench.
    /// </summary>
    public const int MaxSubstituteCount = 3;

    /// <summary>
    /// Maximum total slots (starters + substitutes).
    /// </summary>
    public const int MaxTotalSlots = 14;

    /// <summary>
    /// The team this lineup belongs to. Empty Guid if unassigned.
    /// </summary>
    public Guid TeamId { get; set; }

    /// <summary>
    /// The formation setup for the match.
    /// Defaults to 4-4-2 (the zero-value enum member).
    /// </summary>
    public Formation Formation { get; set; }

    /// <summary>
    /// The tactical approach for the match.
    /// Defaults to Normal (the zero-value enum member).
    /// </summary>
    public Tactic Tactic { get; set; }

    /// <summary>
    /// The team's attitude/confidence level for the match.
    /// Defaults to PlayItCool (the zero-value enum member).
    /// </summary>
    public TeamAttitude Attitude { get; set; }

    /// <summary>
    /// The player slots in this lineup (starters and substitutes).
    /// Initialized to empty list.
    /// </summary>
    public List<MatchLineupSlot> Slots { get; set; } = new();

    /// <summary>
    /// The player designated for set pieces (free kicks, corners).
    /// Null if no set pieces taker assigned.
    /// </summary>
    public Guid? SetPiecesTakerId { get; set; }

    /// <summary>
    /// The player designated as team captain.
    /// Null if no captain assigned.
    /// </summary>
    public Guid? CaptainId { get; set; }
}
