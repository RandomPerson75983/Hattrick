using Hattrick.Core.Models;

namespace Hattrick.Core.Services;

/// <summary>
/// Service for generating complete teams with populated player squads.
/// </summary>
public interface ITeamGenerationService
{
    /// <summary>
    /// Generates a new team with a full squad of players.
    /// </summary>
    /// <param name="teamName">The name for the new team.</param>
    /// <param name="isHumanControlled">Whether the team is controlled by a human player.</param>
    /// <returns>A tuple containing the newly generated team and its 25 players.</returns>
    (Team Team, IReadOnlyList<Player> Players) GenerateTeam(string teamName, bool isHumanControlled);
}
