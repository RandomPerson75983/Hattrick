using Hattrick.Core.Models;

namespace Hattrick.Core.Repositories;

/// <summary>
/// Repository interface for managing Player entities in memory.
/// </summary>
public interface IPlayerRepository
{
    /// <summary>
    /// Returns all players belonging to the specified team.
    /// Returns a snapshot (new list), not a live reference.
    /// </summary>
    IReadOnlyList<Player> GetByTeamId(Guid teamId);

    /// <summary>
    /// Returns the player with the specified ID, or null if not found.
    /// </summary>
    Player? GetById(Guid playerId);

    /// <summary>
    /// Adds a player to the repository.
    /// Throws <see cref="ArgumentException"/> if a player with the same ID already exists.
    /// </summary>
    void Add(Player player);

    /// <summary>
    /// Replaces the stored player with the given player (matched by ID).
    /// Throws <see cref="KeyNotFoundException"/> if the player does not exist.
    /// </summary>
    void Update(Player player);

    /// <summary>
    /// Removes the player with the specified ID. No-op if the player does not exist.
    /// </summary>
    void Remove(Guid playerId);
}
