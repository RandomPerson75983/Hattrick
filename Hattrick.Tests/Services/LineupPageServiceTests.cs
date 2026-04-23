using Hattrick.Core.Models;
using Hattrick.Core.Repositories;
using Hattrick.Core.Services;
using NSubstitute;

namespace Hattrick.Tests.Services;

/// <summary>
/// Unit tests for ILineupPageService.
///
/// LineupPageService mediates between Lineup.razor and the repositories/services,
/// following the architecture rule: "Components call services. Services call repositories."
///
/// Dependencies:
///   - ILineupService: for validating and suggesting lineups
///   - IPlayerRepository: for getting players by team ID
///   - ITeamRepository: for verifying team existence
///   - IGameStateService: for getting HumanPlayerTeamId
///
/// Key behaviors:
///   - GetAvailablePlayers excludes injured (InjuryWeeks > 0) and suspended players
///   - Suspended = RedCard OR YellowCards >= 3
///   - SuggestLineup delegates to ILineupService.SuggestLineup with available players
/// </summary>
public class LineupPageServiceTests
{
    private readonly ILineupService _lineupService;
    private readonly IPlayerRepository _playerRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IGameStateService _gameStateService;

    // Test fixture constants
    private const int TestPlayerAge = 25;
    private const int TestPlayerForm = 5;
    private const int TestPlayerStamina = 5;
    private const int YellowCardSuspensionThreshold = 3;

    public LineupPageServiceTests()
    {
        _lineupService = Substitute.For<ILineupService>();
        _playerRepository = Substitute.For<IPlayerRepository>();
        _teamRepository = Substitute.For<ITeamRepository>();
        _gameStateService = Substitute.For<IGameStateService>();
    }

    private ILineupPageService CreateSut() =>
        new LineupPageService(_lineupService, _playerRepository, _teamRepository, _gameStateService);

    // -------------------------------------------------------------------------
    // Helper: Create a minimal player for testing
    // -------------------------------------------------------------------------

    private static Player CreatePlayer(
        Guid teamId,
        string name = "Test Player",
        int injuryWeeks = 0,
        bool redCard = false,
        int yellowCards = 0)
    {
        return new Player
        {
            Id = Guid.NewGuid(),
            TeamId = teamId,
            Name = name,
            Age = TestPlayerAge,
            Form = TestPlayerForm,
            Stamina = TestPlayerStamina,
            InjuryWeeks = injuryWeeks,
            RedCard = redCard,
            YellowCards = yellowCards,
        };
    }

    private static Team CreateTeam(Guid? id = null, string name = "Test Team")
    {
        return new Team
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
        };
    }

    // =========================================================================
    // Constructor null checks
    // =========================================================================

    [Fact]
    public void Constructor_WithNullLineupService_ThrowsArgumentNullException()
    {
        var act = () => new LineupPageService(null!, _playerRepository, _teamRepository, _gameStateService);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("lineupService");
    }

    [Fact]
    public void Constructor_WithNullPlayerRepository_ThrowsArgumentNullException()
    {
        var act = () => new LineupPageService(_lineupService, null!, _teamRepository, _gameStateService);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("playerRepository");
    }

    [Fact]
    public void Constructor_WithNullTeamRepository_ThrowsArgumentNullException()
    {
        var act = () => new LineupPageService(_lineupService, _playerRepository, null!, _gameStateService);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("teamRepository");
    }

    [Fact]
    public void Constructor_WithNullGameStateService_ThrowsArgumentNullException()
    {
        var act = () => new LineupPageService(_lineupService, _playerRepository, _teamRepository, null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("gameStateService");
    }

    // =========================================================================
    // GetLineupForTeam
    // =========================================================================

    [Fact]
    public void GetLineupForTeam_WithValidTeamId_ReturnsLineupWithCorrectTeamId()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = CreateTeam(teamId);
        _teamRepository.GetById(teamId).Returns(team);

        var sut = CreateSut();

        // Act
        var result = sut.GetLineupForTeam(teamId);

        // Assert
        result.Should().NotBeNull();
        result.TeamId.Should().Be(teamId);
    }

    [Fact]
    public void GetLineupForTeam_WithNonExistentTeamId_ReturnsEmptyLineupWithTeamId()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        _teamRepository.GetById(teamId).Returns((Team?)null);

        var sut = CreateSut();

        // Act
        var result = sut.GetLineupForTeam(teamId);

        // Assert
        result.Should().NotBeNull();
        result.TeamId.Should().Be(teamId);
        result.Slots.Should().BeEmpty();
    }

    [Fact]
    public void GetLineupForTeam_CalledTwiceForSameTeam_ReturnsConsistentLineup()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = CreateTeam(teamId);
        _teamRepository.GetById(teamId).Returns(team);

        var sut = CreateSut();

        // Act
        var result1 = sut.GetLineupForTeam(teamId);
        var result2 = sut.GetLineupForTeam(teamId);

        // Assert - both calls should return equivalent lineups
        result1.TeamId.Should().Be(result2.TeamId);
    }

    // =========================================================================
    // GetAvailablePlayers - Basic functionality
    // =========================================================================

    [Fact]
    public void GetAvailablePlayers_WithHealthyPlayers_ReturnsAllPlayers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var players = new List<Player>
        {
            CreatePlayer(teamId, "Player One"),
            CreatePlayer(teamId, "Player Two"),
            CreatePlayer(teamId, "Player Three"),
        }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Name == "Player One");
        result.Should().Contain(p => p.Name == "Player Two");
        result.Should().Contain(p => p.Name == "Player Three");
    }

    [Fact]
    public void GetAvailablePlayers_WithEmptySquad_ReturnsEmptyList()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        _playerRepository.GetByTeamId(teamId).Returns(Array.Empty<Player>());

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().BeEmpty();
    }

    // =========================================================================
    // GetAvailablePlayers - Injury filtering
    // =========================================================================

    [Fact]
    public void GetAvailablePlayers_ExcludesInjuredPlayers_InjuryWeeksGreaterThanZero()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var healthyPlayer = CreatePlayer(teamId, "Healthy Player", injuryWeeks: 0);
        var injuredPlayer = CreatePlayer(teamId, "Injured Player", injuryWeeks: 1);
        var players = new List<Player> { healthyPlayer, injuredPlayer }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(p => p.Name == "Healthy Player");
        result.Should().NotContain(p => p.Name == "Injured Player");
    }

    [Fact]
    public void GetAvailablePlayers_ExcludesLongTermInjuredPlayers()
    {
        // Arrange - test with various injury lengths
        var teamId = Guid.NewGuid();
        var healthyPlayer = CreatePlayer(teamId, "Healthy", injuryWeeks: 0);
        var shortInjury = CreatePlayer(teamId, "Short Injury", injuryWeeks: 1);
        var mediumInjury = CreatePlayer(teamId, "Medium Injury", injuryWeeks: 4);
        var longInjury = CreatePlayer(teamId, "Long Injury", injuryWeeks: 12);
        var players = new List<Player> { healthyPlayer, shortInjury, mediumInjury, longInjury }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().HaveCount(1);
        result.Single().Name.Should().Be("Healthy");
    }

    [Fact]
    public void GetAvailablePlayers_InjuryWeeksZero_PlayerIsAvailable()
    {
        // Arrange - boundary test: InjuryWeeks == 0 means healthy
        var teamId = Guid.NewGuid();
        var player = CreatePlayer(teamId, "Just Recovered", injuryWeeks: 0);
        var players = new List<Player> { player }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().HaveCount(1);
        result.Single().Name.Should().Be("Just Recovered");
    }

    // =========================================================================
    // GetAvailablePlayers - Red card suspension filtering
    // =========================================================================

    [Fact]
    public void GetAvailablePlayers_ExcludesRedCardPlayers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var cleanPlayer = CreatePlayer(teamId, "Clean Player", redCard: false);
        var redCardPlayer = CreatePlayer(teamId, "Red Card Player", redCard: true);
        var players = new List<Player> { cleanPlayer, redCardPlayer }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(p => p.Name == "Clean Player");
        result.Should().NotContain(p => p.Name == "Red Card Player");
    }

    [Fact]
    public void GetAvailablePlayers_RedCardFalse_PlayerIsAvailable()
    {
        // Arrange - boundary: RedCard == false means available
        var teamId = Guid.NewGuid();
        var player = CreatePlayer(teamId, "No Red Card", redCard: false);
        var players = new List<Player> { player }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().HaveCount(1);
    }

    // =========================================================================
    // GetAvailablePlayers - Yellow card suspension filtering
    // =========================================================================

    [Fact]
    public void GetAvailablePlayers_ExcludesPlayersWithThreeYellowCards()
    {
        // Arrange - 3 yellows = suspended
        var teamId = Guid.NewGuid();
        var noYellows = CreatePlayer(teamId, "No Yellows", yellowCards: 0);
        var threeYellows = CreatePlayer(teamId, "Three Yellows", yellowCards: 3);
        var players = new List<Player> { noYellows, threeYellows }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().HaveCount(1);
        result.Single().Name.Should().Be("No Yellows");
    }

    [Fact]
    public void GetAvailablePlayers_TwoYellowCards_PlayerIsAvailable()
    {
        // Arrange - boundary: 2 yellows is NOT suspended (only 3+ is)
        var teamId = Guid.NewGuid();
        var player = CreatePlayer(teamId, "Two Yellows", yellowCards: 2);
        var players = new List<Player> { player }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().HaveCount(1);
        result.Single().YellowCards.Should().Be(2);
    }

    [Fact]
    public void GetAvailablePlayers_MoreThanThreeYellowCards_PlayerIsSuspended()
    {
        // Arrange - edge case: more than 3 yellows (shouldn't happen normally but test it)
        var teamId = Guid.NewGuid();
        var player = CreatePlayer(teamId, "Four Yellows", yellowCards: 4);
        var players = new List<Player> { player }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0, true)]  // 0 yellows = available
    [InlineData(1, true)]  // 1 yellow = available
    [InlineData(2, true)]  // 2 yellows = available
    [InlineData(3, false)] // 3 yellows = suspended
    [InlineData(4, false)] // 4 yellows = suspended
    public void GetAvailablePlayers_YellowCardBoundaries_CorrectlyFilters(int yellowCards, bool shouldBeAvailable)
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var player = CreatePlayer(teamId, $"Player with {yellowCards} yellows", yellowCards: yellowCards);
        var players = new List<Player> { player }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        if (shouldBeAvailable)
        {
            result.Should().HaveCount(1);
        }
        else
        {
            result.Should().BeEmpty();
        }
    }

    // =========================================================================
    // GetAvailablePlayers - Combined exclusion scenarios
    // =========================================================================

    [Fact]
    public void GetAvailablePlayers_WithMultipleExclusionReasons_ExcludesAll()
    {
        // Arrange - one player with each exclusion reason
        var teamId = Guid.NewGuid();
        var healthy = CreatePlayer(teamId, "Healthy");
        var injured = CreatePlayer(teamId, "Injured", injuryWeeks: 2);
        var redCarded = CreatePlayer(teamId, "Red Card", redCard: true);
        var yellowSuspended = CreatePlayer(teamId, "Yellow Suspended", yellowCards: 3);
        var players = new List<Player> { healthy, injured, redCarded, yellowSuspended }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().HaveCount(1);
        result.Single().Name.Should().Be("Healthy");
    }

    [Fact]
    public void GetAvailablePlayers_PlayerWithMultipleIssues_ExcludedOnce()
    {
        // Arrange - player with both injury AND red card
        var teamId = Guid.NewGuid();
        var doublyUnavailable = CreatePlayer(teamId, "Multiple Issues", injuryWeeks: 3, redCard: true);
        var healthy = CreatePlayer(teamId, "Healthy");
        var players = new List<Player> { doublyUnavailable, healthy }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().HaveCount(1);
        result.Single().Name.Should().Be("Healthy");
    }

    [Fact]
    public void GetAvailablePlayers_AllPlayersUnavailable_ReturnsEmptyList()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var injured1 = CreatePlayer(teamId, "Injured1", injuryWeeks: 1);
        var injured2 = CreatePlayer(teamId, "Injured2", injuryWeeks: 5);
        var redCarded = CreatePlayer(teamId, "RedCarded", redCard: true);
        var players = new List<Player> { injured1, injured2, redCarded }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().BeEmpty();
    }

    // =========================================================================
    // SaveLineup
    // =========================================================================

    [Fact]
    public void SaveLineup_WithValidLineup_DoesNotThrow()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var lineup = new TeamLineup { TeamId = teamId };

        var sut = CreateSut();

        // Act
        var act = () => sut.SaveLineup(lineup);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void SaveLineup_WithNullLineup_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var act = () => sut.SaveLineup(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("lineup");
    }

    [Fact]
    public void SaveLineup_ThenGetLineupForTeam_ReturnsUpdatedLineup()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = CreateTeam(teamId);
        _teamRepository.GetById(teamId).Returns(team);

        var lineup = new TeamLineup
        {
            TeamId = teamId,
            Formation = Formation.Formation433,
            Tactic = Tactic.CounterAttack,
        };

        var sut = CreateSut();

        // Act
        sut.SaveLineup(lineup);
        var result = sut.GetLineupForTeam(teamId);

        // Assert
        result.Formation.Should().Be(Formation.Formation433);
        result.Tactic.Should().Be(Tactic.CounterAttack);
    }

    [Fact]
    public void SaveLineup_ForDifferentTeams_KeepsLineupsSeparate()
    {
        // Arrange
        var teamId1 = Guid.NewGuid();
        var teamId2 = Guid.NewGuid();
        var team1 = CreateTeam(teamId1);
        var team2 = CreateTeam(teamId2);
        _teamRepository.GetById(teamId1).Returns(team1);
        _teamRepository.GetById(teamId2).Returns(team2);

        var lineup1 = new TeamLineup { TeamId = teamId1, Formation = Formation.Formation442 };
        var lineup2 = new TeamLineup { TeamId = teamId2, Formation = Formation.Formation352 };

        var sut = CreateSut();

        // Act
        sut.SaveLineup(lineup1);
        sut.SaveLineup(lineup2);
        var result1 = sut.GetLineupForTeam(teamId1);
        var result2 = sut.GetLineupForTeam(teamId2);

        // Assert
        result1.Formation.Should().Be(Formation.Formation442);
        result2.Formation.Should().Be(Formation.Formation352);
    }

    // =========================================================================
    // SuggestLineup - Delegation to ILineupService
    // =========================================================================

    [Fact]
    public void SuggestLineup_DelegatesToLineupService()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var availablePlayers = new List<Player>
        {
            CreatePlayer(teamId, "Player1"),
            CreatePlayer(teamId, "Player2"),
        }.AsReadOnly();

        var expectedLineup = new TeamLineup { TeamId = teamId, Formation = Formation.Formation442 };

        _playerRepository.GetByTeamId(teamId).Returns(availablePlayers);
        _lineupService.SuggestLineup(teamId, Arg.Any<IReadOnlyList<Player>>()).Returns(expectedLineup);

        var sut = CreateSut();

        // Act
        var result = sut.SuggestLineup(teamId);

        // Assert
        result.Should().Be(expectedLineup);
        _lineupService.Received(1).SuggestLineup(teamId, Arg.Any<IReadOnlyList<Player>>());
    }

    [Fact]
    public void SuggestLineup_PassesOnlyAvailablePlayersToLineupService()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var healthy = CreatePlayer(teamId, "Healthy");
        var injured = CreatePlayer(teamId, "Injured", injuryWeeks: 2);
        var allPlayers = new List<Player> { healthy, injured }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(allPlayers);
        _lineupService.SuggestLineup(teamId, Arg.Any<IReadOnlyList<Player>>())
            .Returns(new TeamLineup { TeamId = teamId });

        var sut = CreateSut();

        // Act
        sut.SuggestLineup(teamId);

        // Assert - verify only healthy player was passed
        _lineupService.Received(1).SuggestLineup(
            teamId,
            Arg.Is<IReadOnlyList<Player>>(players =>
                players.Count == 1 && players.Single().Name == "Healthy"));
    }

    [Fact]
    public void SuggestLineup_ExcludesAllSuspendedFromLineupServiceCall()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var healthy = CreatePlayer(teamId, "Healthy");
        var redCard = CreatePlayer(teamId, "RedCard", redCard: true);
        var yellowSuspended = CreatePlayer(teamId, "YellowSuspended", yellowCards: 3);
        var allPlayers = new List<Player> { healthy, redCard, yellowSuspended }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(allPlayers);
        _lineupService.SuggestLineup(teamId, Arg.Any<IReadOnlyList<Player>>())
            .Returns(new TeamLineup { TeamId = teamId });

        var sut = CreateSut();

        // Act
        sut.SuggestLineup(teamId);

        // Assert
        _lineupService.Received(1).SuggestLineup(
            teamId,
            Arg.Is<IReadOnlyList<Player>>(players =>
                players.Count == 1 &&
                !players.Any(p => p.RedCard) &&
                !players.Any(p => p.YellowCards >= YellowCardSuspensionThreshold)));
    }

    [Fact]
    public void SuggestLineup_WhenLineupServiceThrows_PropagatesException()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var players = new List<Player>
        {
            CreatePlayer(teamId, "Player1"),
        }.AsReadOnly();

        _playerRepository.GetByTeamId(teamId).Returns(players);
        _lineupService.SuggestLineup(teamId, Arg.Any<IReadOnlyList<Player>>())
            .Returns(_ => throw new InvalidOperationException("Not enough players"));

        var sut = CreateSut();

        // Act
        var act = () => sut.SuggestLineup(teamId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Not enough players");
    }

    // =========================================================================
    // Edge cases and integration behavior
    // =========================================================================

    [Fact]
    public void GetAvailablePlayers_QueriesCorrectTeamId()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();

        _playerRepository.GetByTeamId(teamId).Returns(new List<Player> { CreatePlayer(teamId) }.AsReadOnly());
        _playerRepository.GetByTeamId(otherTeamId).Returns(Array.Empty<Player>());

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().HaveCount(1);
        _playerRepository.Received(1).GetByTeamId(teamId);
        _playerRepository.DidNotReceive().GetByTeamId(otherTeamId);
    }

    [Fact]
    public void GetAvailablePlayers_ReturnsIReadOnlyList()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        _playerRepository.GetByTeamId(teamId).Returns(new List<Player> { CreatePlayer(teamId) }.AsReadOnly());

        var sut = CreateSut();

        // Act
        var result = sut.GetAvailablePlayers(teamId);

        // Assert
        result.Should().BeAssignableTo<IReadOnlyList<Player>>();
    }
}
