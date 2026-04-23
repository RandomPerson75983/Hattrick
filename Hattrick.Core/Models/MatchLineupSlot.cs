namespace Hattrick.Core.Models;

/// <summary>
/// Represents a single player's position assignment in a match lineup.
/// Phase 2 Sprint 2, Quartet 1 (MatchLineupSlot Model).
///
/// Immutable record capturing which player, what position, any individual order,
/// and whether they are a starter or substitute. Validation logic belongs in
/// LineupService, not in this model.
/// </summary>
/// <param name="playerId">The unique identifier of the player assigned to this slot.</param>
/// <param name="position">The position on the pitch for this slot.</param>
/// <param name="individualOrder">The individual tactical order for this player (defaults to Normal).</param>
/// <param name="isStarter">True if the player starts the match, false if substitute.</param>
public record MatchLineupSlot(
    Guid playerId,
    Position position,
    IndividualOrder individualOrder = IndividualOrder.Normal,
    bool isStarter = false)
{
    /// <summary>
    /// The unique identifier of the player assigned to this slot.
    /// </summary>
    public Guid PlayerId { get; init; } = playerId;

    /// <summary>
    /// The position on the pitch for this slot.
    /// </summary>
    public Position Position { get; init; } = position;

    /// <summary>
    /// The individual tactical order for this player.
    /// </summary>
    public IndividualOrder IndividualOrder { get; init; } = individualOrder;

    /// <summary>
    /// True if the player starts the match, false if substitute.
    /// </summary>
    public bool IsStarter { get; init; } = isStarter;
}
