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
    public Team GenerateTeam(string teamName, bool isHumanControlled)
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

        GeneratePlayers(team);

        return team;
    }

    private void GeneratePlayers(Team team)
    {
        GeneratePlayersForPosition(team, Position.Keeper, KeeperCount);
        GeneratePlayersForPosition(team, Position.CentralDefender, CentralDefenderCount);
        GeneratePlayersForPosition(team, Position.WingBack, WingBackCount);
        GeneratePlayersForPosition(team, Position.InnerMidfielder, InnerMidfielderCount);
        GeneratePlayersForPosition(team, Position.Winger, WingerCount);
        GeneratePlayersForPosition(team, Position.Forward, ForwardCount);
    }

    private void GeneratePlayersForPosition(Team team, Position position, int count)
    {
        for (var i = 0; i < count; i++)
        {
            var player = _playerGenerationService.GeneratePlayer(position);
            player.TeamId = team.Id;
        }
    }
}
