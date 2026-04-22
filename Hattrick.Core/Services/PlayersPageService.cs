using Hattrick.Core.Models;
using Hattrick.Core.Repositories;

namespace Hattrick.Core.Services;

/// <summary>
/// Implementation of IPlayersPageService.
/// Mediates between Players.razor and repositories/services.
/// </summary>
public sealed class PlayersPageService : IPlayersPageService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerStatsService _playerStatsService;
    private readonly IGameStateService _gameStateService;

    public PlayersPageService(
        IPlayerRepository playerRepository,
        IPlayerStatsService playerStatsService,
        IGameStateService gameStateService)
    {
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _playerStatsService = playerStatsService ?? throw new ArgumentNullException(nameof(playerStatsService));
        _gameStateService = gameStateService ?? throw new ArgumentNullException(nameof(gameStateService));
    }

    /// <inheritdoc />
    public IReadOnlyList<Player> GetPlayers()
    {
        var teamId = _gameStateService.HumanPlayerTeamId;
        if (!teamId.HasValue)
        {
            return Array.Empty<Player>();
        }

        return _playerRepository.GetByTeamId(teamId.Value);
    }

    /// <inheritdoc />
    public TeamTotals GetTeamTotals()
    {
        var players = GetPlayers();
        if (players.Count == 0)
        {
            return new TeamTotals();
        }

        return _playerStatsService.GetTeamTotals(players);
    }

    /// <inheritdoc />
    public TeamAverages GetTeamAverages()
    {
        var players = GetPlayers();
        if (players.Count == 0)
        {
            return new TeamAverages();
        }

        return _playerStatsService.GetTeamAverages(players);
    }
}
