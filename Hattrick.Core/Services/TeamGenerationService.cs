using Hattrick.Core.Models;

namespace Hattrick.Core.Services;

/// <summary>
/// Service for generating complete teams with populated player squads.
/// Uses IPlayerGenerationService to generate individual players.
/// </summary>
public class TeamGenerationService : ITeamGenerationService
{
    private readonly IPlayerGenerationService _playerGenerationService;

    /// <summary>
    /// Initializes a new instance of <see cref="TeamGenerationService"/>.
    /// </summary>
    /// <param name="playerGenerationService">Service for generating individual players.</param>
    /// <exception cref="ArgumentNullException">Thrown when playerGenerationService is null.</exception>
    public TeamGenerationService(IPlayerGenerationService playerGenerationService)
    {
        ArgumentNullException.ThrowIfNull(playerGenerationService);
        _playerGenerationService = playerGenerationService;
    }

    // Position distribution constants
    private const int KeeperCount = 3;
    private const int CentralDefenderCount = 4;
    private const int WingBackCount = 2;
    private const int InnerMidfielderCount = 4;
    private const int WingerCount = 4;
    private const int ForwardCount = 8;

    // Team default constants
    private const decimal DefaultBudget = 10_000_000m;
    private const double DefaultTeamSpirit = 5.0;
    private const double DefaultConfidence = 5.0;
    private const int DefaultCoachLevel = 5;
    private const int DefaultFans = 1000;
    private const int DefaultFanClubSize = 100;

    /// <inheritdoc />
    public (Team Team, IReadOnlyList<Player> Players) GenerateTeam(string teamName, bool isHumanControlled)
    {
        ArgumentNullException.ThrowIfNull(teamName);

        var team = new Team
        {
            Name = teamName,
            IsHumanControlled = isHumanControlled,
            Budget = DefaultBudget,
            TeamSpirit = DefaultTeamSpirit,
            Confidence = DefaultConfidence,
            CoachLevel = DefaultCoachLevel,
            AssistantCoachLevel = 0,
            DoctorLevel = 0,
            SpokespersonLevel = 0,
            FinancialDirectorLevel = 0,
            Fans = DefaultFans,
            FanClubSize = DefaultFanClubSize
        };

        var players = GeneratePlayers(team);

        return (team, players);
    }

    private List<Player> GeneratePlayers(Team team)
    {
        var players = new List<Player>();
        players.AddRange(GeneratePlayersForPosition(team, Position.Keeper, KeeperCount));
        players.AddRange(GeneratePlayersForPosition(team, Position.CentralDefender, CentralDefenderCount));
        players.AddRange(GeneratePlayersForPosition(team, Position.WingBack, WingBackCount));
        players.AddRange(GeneratePlayersForPosition(team, Position.InnerMidfielder, InnerMidfielderCount));
        players.AddRange(GeneratePlayersForPosition(team, Position.Winger, WingerCount));
        players.AddRange(GeneratePlayersForPosition(team, Position.Forward, ForwardCount));
        return players;
    }

    private List<Player> GeneratePlayersForPosition(Team team, Position position, int count)
    {
        var players = new List<Player>();
        for (var i = 0; i < count; i++)
        {
            var player = _playerGenerationService.GeneratePlayer(position);
            player.TeamId = team.Id;
            players.Add(player);
        }
        return players;
    }
}
