using Hattrick.Core.Models;

namespace Hattrick.Core.Repositories;

/// <summary>
/// Repository interface for managing Team entities in memory.
/// </summary>
public interface ITeamRepository
{
    /// <summary>
    /// Returns the team with the specified ID, or null if not found.
    /// </summary>
    Team? GetById(Guid teamId);

    /// <summary>
    /// Returns all teams in the repository.
    /// Returns a snapshot (new list), not a live reference.
    /// </summary>
    IReadOnlyList<Team> GetAll();

    /// <summary>
    /// Adds a team to the repository.
    /// Throws <see cref="ArgumentNullException"/> if team is null.
    /// Throws <see cref="ArgumentException"/> if a team with the same ID already exists.
    /// </summary>
    void Add(Team team);

    /// <summary>
    /// Replaces the stored team with the given team (matched by ID).
    /// Throws <see cref="ArgumentNullException"/> if team is null.
    /// Throws <see cref="KeyNotFoundException"/> if the team does not exist.
    /// </summary>
    void Update(Team team);
}
