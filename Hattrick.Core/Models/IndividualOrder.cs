namespace Hattrick.Core.Models;

/// <summary>
/// Individual tactical orders for a player within the team's formation.
/// Phase 1 Sprint 1, Quartet 2 (Position & Order Enums).
///
/// Controls how aggressively/passively a player executes their role:
/// - Normal: Standard play for position
/// - Offensive: Play more attack-minded (push forward, take risks)
/// - Defensive: Play more cautiously (stay back, reduce risk)
/// - TowardsMiddle: Shift positioning toward the center
/// - TowardsWing: Shift positioning toward the touchline
/// </summary>
public enum IndividualOrder
{
    Normal = 0,
    Offensive = 1,
    Defensive = 2,
    TowardsMiddle = 3,
    TowardsWing = 4
}
