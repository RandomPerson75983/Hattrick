using Hattrick.Core.Models;

namespace Hattrick.Core.Repositories;

/// <summary>
/// Thread-safe, in-memory repository for Player entities.
/// Uses a Dictionary keyed by Player.Id for O(1) lookups.
/// </summary>
public class PlayerRepository : IPlayerRepository
{
    private readonly Lock _lock = new();
    private readonly Dictionary<Guid, Player> _players = new();

    /// <inheritdoc />
    public IReadOnlyList<Player> GetByTeamId(Guid teamId)
    {
        lock (_lock)
        {
            return _players.Values
                .Where(p => p.TeamId == teamId)
                .ToList()
                .AsReadOnly();
        }
    }

    /// <inheritdoc />
    public Player? GetById(Guid playerId)
    {
        lock (_lock)
        {
            return _players.GetValueOrDefault(playerId);
        }
    }

    /// <inheritdoc />
    public void Add(Player player)
    {
        lock (_lock)
        {
            if (_players.ContainsKey(player.Id))
            {
                throw new ArgumentException(
                    $"A player with ID '{player.Id}' already exists in the repository.");
            }

            _players[player.Id] = player;
        }
    }

    /// <inheritdoc />
    public void Update(Player player)
    {
        lock (_lock)
        {
            if (!_players.ContainsKey(player.Id))
            {
                throw new KeyNotFoundException(
                    $"No player with ID '{player.Id}' exists in the repository.");
            }

            _players[player.Id] = player;
        }
    }

    /// <inheritdoc />
    public void Remove(Guid playerId)
    {
        lock (_lock)
        {
            _players.Remove(playerId);
        }
    }
}
