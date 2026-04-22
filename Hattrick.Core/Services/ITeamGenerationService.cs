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
    /// <returns>A newly generated team with a complete squad of 25 players.</returns>
    Team GenerateTeam(string teamName, bool isHumanControlled);
}
