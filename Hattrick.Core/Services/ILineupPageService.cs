using Hattrick.Core.Models;

namespace Hattrick.Core.Services;

/// <summary>
/// Service mediating between Lineup.razor component and repositories/services.
/// Follows the architecture rule: "Components call services. Services call repositories."
/// </summary>
public interface ILineupPageService
{
    /// <summary>
    /// Gets the lineup for a team. Returns an empty lineup if none has been saved.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <returns>The team's lineup.</returns>
    TeamLineup GetLineupForTeam(Guid teamId);

    /// <summary>
    /// Gets all available players for lineup selection.
    /// Excludes injured (InjuryWeeks > 0), red-carded, and yellow-suspended (YellowCards >= 3) players.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <returns>List of available players.</returns>
    IReadOnlyList<Player> GetAvailablePlayers(Guid teamId);

    /// <summary>
    /// Saves a lineup configuration for a team.
    /// </summary>
    /// <param name="lineup">The lineup to save.</param>
    void SaveLineup(TeamLineup lineup);

    /// <summary>
    /// Suggests an optimal lineup for a team using available players.
    /// Delegates to ILineupService.SuggestLineup with filtered players.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <returns>A suggested TeamLineup.</returns>
    TeamLineup SuggestLineup(Guid teamId);
}
