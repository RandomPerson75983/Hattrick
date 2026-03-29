namespace Hattrick.Core.Models;

/// <summary>
/// Player position on the pitch in Hattrick formations.
/// Phase 1 Sprint 1, Quartet 2 (Position & Order Enums).
///
/// Valid positions for standard Hattrick formations:
/// - Keeper: Goalkeeper
/// - CentralDefender: Central defense line
/// - WingBack: Wide defense (can attack)
/// - InnerMidfielder: Central midfield
/// - Winger: Wide midfield (can defend)
/// - Forward: Striker/attacker
/// </summary>
public enum Position
{
    Keeper = 0,
    CentralDefender = 1,
    WingBack = 2,
    InnerMidfielder = 3,
    Winger = 4,
    Forward = 5
}
