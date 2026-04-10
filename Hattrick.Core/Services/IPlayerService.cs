using Hattrick.Core.Models;

namespace Hattrick.Core.Services;

public interface IPlayerService
{
    IReadOnlyList<Player> GetAllPlayers();
}
