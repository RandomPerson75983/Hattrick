using Hattrick.Core.Models;

namespace Hattrick.Core.Services;

/// <summary>
/// Computes team-level aggregate statistics from a list of players.
/// All methods are pure functions with no side effects.
///
/// Formula reference:
///   EstimatedValue per player = TSI * 25
/// </summary>
public interface IPlayerStatsService
{
    /// <summary>
    /// Returns cumulative totals (TSI, wage, estimated value, card counts, etc.)
    /// for the given player list. Safe to call with an empty list.
    /// </summary>
    TeamTotals GetTeamTotals(IReadOnlyList<Player> players);

    /// <summary>
    /// Returns arithmetic-mean averages for the given player list.
    /// All fields return 0.0 when the list is empty (no divide-by-zero).
    /// </summary>
    TeamAverages GetTeamAverages(IReadOnlyList<Player> players);
}
