using Hattrick.Core.Repositories;

namespace Hattrick.Core.Services;

/// <summary>
/// Service for seeding development data on app startup.
/// </summary>
public class DevSeedService : IDevSeedService
{
    private readonly ITeamGenerationService _teamGenerationService;
    private readonly IPlayerRepository _playerRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IGameStateService _gameStateService;

    public DevSeedService(
        ITeamGenerationService teamGenerationService,
        IPlayerRepository playerRepository,
        ITeamRepository teamRepository,
        IGameStateService gameStateService)
    {
        ArgumentNullException.ThrowIfNull(teamGenerationService);
        ArgumentNullException.ThrowIfNull(playerRepository);
        ArgumentNullException.ThrowIfNull(teamRepository);
        ArgumentNullException.ThrowIfNull(gameStateService);

        _teamGenerationService = teamGenerationService;
        _playerRepository = playerRepository;
        _teamRepository = teamRepository;
        _gameStateService = gameStateService;
    }

    private const string TeamName = "FC Development";

    /// <inheritdoc />
    public Task SeedAsync()
    {
        // Idempotency check: if already seeded, return early
        if (_gameStateService.HumanPlayerTeamId.HasValue)
        {
            return Task.CompletedTask;
        }

        // Generate the team and players
        var (team, players) = _teamGenerationService.GenerateTeam(TeamName, isHumanControlled: true);

        // Add each player to the repository
        foreach (var player in players)
        {
            _playerRepository.Add(player);
        }

        // Add team to repository BEFORE setting HumanPlayerTeamId
        _teamRepository.Add(team);

        // Set the human player team ID
        _gameStateService.HumanPlayerTeamId = team.Id;

        return Task.CompletedTask;
    }
}
