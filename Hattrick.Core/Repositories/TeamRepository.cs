using Hattrick.Core.Models;

namespace Hattrick.Core.Repositories;

/// <summary>
/// Thread-safe, in-memory repository for Team entities.
/// Uses a Dictionary keyed by Team.Id for O(1) lookups.
/// Teams are permanent in Phase 1 — no Remove method.
/// </summary>
public class TeamRepository : ITeamRepository
{
    private readonly Lock _lock = new();
    private readonly Dictionary<Guid, Team> _teams = new();

    /// <inheritdoc />
    public Team? GetById(Guid teamId)
    {
        lock (_lock)
        {
            return _teams.GetValueOrDefault(teamId);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Team> GetAll()
    {
        lock (_lock)
        {
            return _teams.Values
                .ToList()
                .AsReadOnly();
        }
    }

    /// <inheritdoc />
    public void Add(Team team)
    {
        ArgumentNullException.ThrowIfNull(team);

        lock (_lock)
        {
            if (!_teams.TryAdd(team.Id, team))
            {
                throw new ArgumentException(
                    $"A team with ID '{team.Id}' already exists in the repository.");
            }
        }
    }

    /// <inheritdoc />
    public void Update(Team team)
    {
        ArgumentNullException.ThrowIfNull(team);

        lock (_lock)
        {
            if (!_teams.ContainsKey(team.Id))
            {
                throw new KeyNotFoundException(
                    $"No team with ID '{team.Id}' exists in the repository.");
            }

            _teams[team.Id] = team;
        }
    }
}
