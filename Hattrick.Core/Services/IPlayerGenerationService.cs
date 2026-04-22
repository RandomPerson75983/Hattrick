using Hattrick.Core.Models;

namespace Hattrick.Core.Services;

/// <summary>
/// Service for generating players with randomized but position-appropriate attributes.
/// </summary>
public interface IPlayerGenerationService
{
    /// <summary>
    /// Generates a new player with randomized attributes appropriate for the specified position.
    /// </summary>
    /// <param name="position">The position the player will play.</param>
    /// <returns>A newly generated player with position-appropriate skills.</returns>
    Player GeneratePlayer(Position position);
}
