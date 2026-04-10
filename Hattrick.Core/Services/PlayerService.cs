using Hattrick.Core.Models;
using Hattrick.Core.Repositories;

namespace Hattrick.Core.Services;

public sealed class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _repository;

    public PlayerService(IPlayerRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    // TODO: Replace Guid.Empty with IGameStateService.HumanPlayerTeamId when game-loading is wired up
    public IReadOnlyList<Player> GetAllPlayers() => _repository.GetByTeamId(Guid.Empty);
}
