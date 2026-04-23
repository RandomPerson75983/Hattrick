using Hattrick.Core.Models;
using Hattrick.Core.Repositories;

namespace Hattrick.Core.Services;

/// <summary>
/// Implementation of ILineupPageService.
/// Mediates between Lineup.razor and repositories/services.
/// </summary>
public sealed class LineupPageService : ILineupPageService
{
    private const int YellowCardSuspensionThreshold = 3;

    private readonly ILineupService _lineupService;
    private readonly IPlayerRepository _playerRepository;
    private readonly Lock _lock = new();
    private readonly Dictionary<Guid, TeamLineup> _lineups = new();

    public LineupPageService(
        ILineupService lineupService,
        IPlayerRepository playerRepository)
    {
        _lineupService = lineupService ?? throw new ArgumentNullException(nameof(lineupService));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

    /// <inheritdoc />
    public TeamLineup GetLineupForTeam(Guid teamId)
    {
        if (teamId == Guid.Empty)
            throw new ArgumentException("Team ID cannot be empty.", nameof(teamId));

        lock (_lock)
        {
            if (_lineups.TryGetValue(teamId, out var lineup))
            {
                return lineup;
            }

            // Create and return a new empty lineup for this team
            var newLineup = new TeamLineup { TeamId = teamId };
            _lineups[teamId] = newLineup;
            return newLineup;
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Player> GetAvailablePlayers(Guid teamId)
    {
        if (teamId == Guid.Empty)
            throw new ArgumentException("Team ID cannot be empty.", nameof(teamId));

        var players = _playerRepository.GetByTeamId(teamId);

        return players
            .Where(p => p.InjuryWeeks == 0)
            .Where(p => !p.RedCard)
            .Where(p => p.YellowCards < YellowCardSuspensionThreshold)
            .ToList()
            .AsReadOnly();
    }

    /// <inheritdoc />
    public void SaveLineup(TeamLineup lineup)
    {
        ArgumentNullException.ThrowIfNull(lineup);

        lock (_lock)
        {
            _lineups[lineup.TeamId] = lineup;
        }
    }

    /// <inheritdoc />
    public TeamLineup SuggestLineup(Guid teamId)
    {
        if (teamId == Guid.Empty)
            throw new ArgumentException("Team ID cannot be empty.", nameof(teamId));

        var availablePlayers = GetAvailablePlayers(teamId);
        return _lineupService.SuggestLineup(teamId, availablePlayers);
    }

    /// <inheritdoc />
    public IReadOnlyList<MatchLineupSlot> GetStarters(Guid teamId)
    {
        if (teamId == Guid.Empty)
            throw new ArgumentException("Team ID cannot be empty.", nameof(teamId));

        var lineup = GetLineupForTeam(teamId);
        return lineup.Slots.Where(s => s.IsStarter).ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public IReadOnlyList<MatchLineupSlot> GetBenchPlayers(Guid teamId)
    {
        if (teamId == Guid.Empty)
            throw new ArgumentException("Team ID cannot be empty.", nameof(teamId));

        var lineup = GetLineupForTeam(teamId);
        return lineup.Slots.Where(s => !s.IsStarter).ToList().AsReadOnly();
    }
}
