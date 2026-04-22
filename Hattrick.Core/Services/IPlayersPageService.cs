using Hattrick.Core.Models;

namespace Hattrick.Core.Services;

/// <summary>
/// Service mediating between Players.razor component and repositories/services.
/// Follows the architecture rule: "Components call services. Services call repositories."
/// </summary>
public interface IPlayersPageService
{
    /// <summary>
    /// Returns the list of players for the human player's team.
    /// Returns an empty list if no team is loaded (HumanPlayerTeamId is null).
    /// </summary>
    IReadOnlyList<Player> GetPlayers();

    /// <summary>
    /// Returns aggregate totals for the human player's team.
    /// Returns empty TeamTotals if no team is loaded.
    /// </summary>
    TeamTotals GetTeamTotals();

    /// <summary>
    /// Returns arithmetic mean averages for the human player's team.
    /// Returns empty TeamAverages if no team is loaded.
    /// </summary>
    TeamAverages GetTeamAverages();
}
