using Hattrick.Core.Models;
using Hattrick.Core.Repositories;
using Hattrick.Core.Services;
using NSubstitute;

namespace Hattrick.Tests.Services;

/// <summary>
/// Unit tests for IPlayersPageService.
///
/// PlayersPageService mediates between Players.razor and the repositories/services,
/// following the architecture rule: "Components call services. Services call repositories."
///
/// Dependencies:
///   - IPlayerRepository: for getting players by team ID
///   - IPlayerStatsService: for calculating totals and averages
///   - IGameStateService: for getting HumanPlayerTeamId
///
/// Pattern: Service retrieves HumanPlayerTeamId from IGameStateService,
/// fetches players from IPlayerRepository, then delegates aggregate
/// calculations to IPlayerStatsService.
/// </summary>
public class PlayersPageServiceTests
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerStatsService _playerStatsService;
    private readonly IGameStateService _gameStateService;

    public PlayersPageServiceTests()
    {
        _playerRepository = Substitute.For<IPlayerRepository>();
        _playerStatsService = Substitute.For<IPlayerStatsService>();
        _gameStateService = Substitute.For<IGameStateService>();
    }

    private IPlayersPageService CreateSut() =>
        new PlayersPageService(_playerRepository, _playerStatsService, _gameStateService);

    // -------------------------------------------------------------------------
    // Helper: Create a minimal player for testing
    // -------------------------------------------------------------------------

    private static Player CreatePlayer(Guid teamId, string name = "Test Player")
    {
        return new Player
        {
            Id = Guid.NewGuid(),
            TeamId = teamId,
            Name = name,
            Age = 25,
            Form = 5,
            Stamina = 5,
            Experience = 5,
            TSI = 4500,
            Wage = 10000m,
        };
    }

    // =========================================================================
    // Constructor null checks
    // =========================================================================

    [Fact]
    public void Constructor_WithNullPlayerRepository_ThrowsArgumentNullException()
    {
        var act = () => new PlayersPageService(null!, _playerStatsService, _gameStateService);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("playerRepository");
    }

    [Fact]
    public void Constructor_WithNullPlayerStatsService_ThrowsArgumentNullException()
    {
        var act = () => new PlayersPageService(_playerRepository, null!, _gameStateService);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("playerStatsService");
    }

    [Fact]
    public void Constructor_WithNullGameStateService_ThrowsArgumentNullException()
    {
        var act = () => new PlayersPageService(_playerRepository, _playerStatsService, null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("gameStateService");
    }

    // =========================================================================
    // GetPlayers
    // =========================================================================

    [Fact]
    public void GetPlayers_WithValidHumanPlayerTeamId_ReturnsPlayersFromRepository()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var expectedPlayers = new List<Player>
        {
            CreatePlayer(teamId, "Player One"),
            CreatePlayer(teamId, "Player Two"),
            CreatePlayer(teamId, "Player Three"),
        }.AsReadOnly();

        _gameStateService.HumanPlayerTeamId.Returns(teamId);
        _playerRepository.GetByTeamId(teamId).Returns(expectedPlayers);

        var sut = CreateSut();

        // Act
        var result = sut.GetPlayers();

        // Assert
        result.Should().BeEquivalentTo(expectedPlayers);
        _playerRepository.Received(1).GetByTeamId(teamId);
    }

    [Fact]
    public void GetPlayers_WithNullHumanPlayerTeamId_ReturnsEmptyList()
    {
        // Arrange
        _gameStateService.HumanPlayerTeamId.Returns((Guid?)null);

        var sut = CreateSut();

        // Act
        var result = sut.GetPlayers();

        // Assert
        result.Should().BeEmpty();
        _playerRepository.DidNotReceive().GetByTeamId(Arg.Any<Guid>());
    }

    [Fact]
    public void GetPlayers_WithValidTeamId_QueriesCorrectTeamId()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();
        var teamPlayers = new List<Player> { CreatePlayer(teamId) }.AsReadOnly();

        _gameStateService.HumanPlayerTeamId.Returns(teamId);
        _playerRepository.GetByTeamId(teamId).Returns(teamPlayers);
        _playerRepository.GetByTeamId(otherTeamId).Returns(Array.Empty<Player>());

        var sut = CreateSut();

        // Act
        var result = sut.GetPlayers();

        // Assert
        result.Should().HaveCount(1);
        _playerRepository.Received(1).GetByTeamId(teamId);
        _playerRepository.DidNotReceive().GetByTeamId(otherTeamId);
    }

    [Fact]
    public void GetPlayers_WhenRepositoryReturnsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        _gameStateService.HumanPlayerTeamId.Returns(teamId);
        _playerRepository.GetByTeamId(teamId).Returns(Array.Empty<Player>());

        var sut = CreateSut();

        // Act
        var result = sut.GetPlayers();

        // Assert
        result.Should().BeEmpty();
    }

    // =========================================================================
    // GetTeamTotals
    // =========================================================================

    [Fact]
    public void GetTeamTotals_WithValidTeam_CallsPlayerStatsServiceWithCorrectPlayers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var players = new List<Player>
        {
            CreatePlayer(teamId, "Player A"),
            CreatePlayer(teamId, "Player B"),
        }.AsReadOnly();

        var expectedTotals = new TeamTotals
        {
            TotalTSI = 9000,
            TotalWage = 20000m,
            TotalEstimatedValue = 225000m,
            NationalityCount = 2,
        };

        _gameStateService.HumanPlayerTeamId.Returns(teamId);
        _playerRepository.GetByTeamId(teamId).Returns(players);
        _playerStatsService.GetTeamTotals(Arg.Is<IReadOnlyList<Player>>(p => p.Count == 2))
            .Returns(expectedTotals);

        var sut = CreateSut();

        // Act
        var result = sut.GetTeamTotals();

        // Assert
        result.Should().Be(expectedTotals);
        _playerStatsService.Received(1).GetTeamTotals(Arg.Is<IReadOnlyList<Player>>(p => p.Count == 2));
    }

    [Fact]
    public void GetTeamTotals_WithNullHumanPlayerTeamId_ReturnsEmptyTotals()
    {
        // Arrange
        _gameStateService.HumanPlayerTeamId.Returns((Guid?)null);

        var sut = CreateSut();

        // Act
        var result = sut.GetTeamTotals();

        // Assert - all fields should be zero (empty totals)
        result.TotalTSI.Should().Be(0);
        result.TotalWage.Should().Be(0m);
        result.TotalEstimatedValue.Should().Be(0m);
        result.NationalityCount.Should().Be(0);
        result.InjuredCount.Should().Be(0);
        result.RedCardCount.Should().Be(0);
        result.YellowCardCount.Should().Be(0);
        _playerStatsService.DidNotReceive().GetTeamTotals(Arg.Any<IReadOnlyList<Player>>());
    }

    [Fact]
    public void GetTeamTotals_WithValidTeam_PassesSamePlayersFromRepository()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var player1 = CreatePlayer(teamId, "Specific Player 1");
        var player2 = CreatePlayer(teamId, "Specific Player 2");
        var players = new List<Player> { player1, player2 }.AsReadOnly();

        _gameStateService.HumanPlayerTeamId.Returns(teamId);
        _playerRepository.GetByTeamId(teamId).Returns(players);
        _playerStatsService.GetTeamTotals(Arg.Any<IReadOnlyList<Player>>())
            .Returns(new TeamTotals());

        var sut = CreateSut();

        // Act
        sut.GetTeamTotals();

        // Assert - verify exact players were passed
        _playerStatsService.Received(1).GetTeamTotals(Arg.Is<IReadOnlyList<Player>>(
            p => p.Contains(player1) && p.Contains(player2)));
    }

    // =========================================================================
    // GetTeamAverages
    // =========================================================================

    [Fact]
    public void GetTeamAverages_WithValidTeam_CallsPlayerStatsServiceWithCorrectPlayers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var players = new List<Player>
        {
            CreatePlayer(teamId, "Player X"),
            CreatePlayer(teamId, "Player Y"),
            CreatePlayer(teamId, "Player Z"),
        }.AsReadOnly();

        var expectedAverages = new TeamAverages
        {
            AvgTSI = 4500.0,
            AvgWage = 10000.0,
            AvgEstimatedValue = 112500m,
            AvgAge = 25.0,
            AvgForm = 5.0,
            AvgStamina = 5.0,
            AvgExperience = 5.0,
        };

        _gameStateService.HumanPlayerTeamId.Returns(teamId);
        _playerRepository.GetByTeamId(teamId).Returns(players);
        _playerStatsService.GetTeamAverages(Arg.Is<IReadOnlyList<Player>>(p => p.Count == 3))
            .Returns(expectedAverages);

        var sut = CreateSut();

        // Act
        var result = sut.GetTeamAverages();

        // Assert
        result.Should().Be(expectedAverages);
        _playerStatsService.Received(1).GetTeamAverages(Arg.Is<IReadOnlyList<Player>>(p => p.Count == 3));
    }

    [Fact]
    public void GetTeamAverages_WithNullHumanPlayerTeamId_ReturnsEmptyAverages()
    {
        // Arrange
        _gameStateService.HumanPlayerTeamId.Returns((Guid?)null);

        var sut = CreateSut();

        // Act
        var result = sut.GetTeamAverages();

        // Assert - all fields should be zero (empty averages)
        result.AvgTSI.Should().Be(0.0);
        result.AvgWage.Should().Be(0.0);
        result.AvgEstimatedValue.Should().Be(0m);
        result.AvgAge.Should().Be(0.0);
        result.AvgForm.Should().Be(0.0);
        result.AvgStamina.Should().Be(0.0);
        result.AvgExperience.Should().Be(0.0);
        _playerStatsService.DidNotReceive().GetTeamAverages(Arg.Any<IReadOnlyList<Player>>());
    }

    [Fact]
    public void GetTeamAverages_WithValidTeam_PassesSamePlayersFromRepository()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var player1 = CreatePlayer(teamId, "Unique Player Alpha");
        var player2 = CreatePlayer(teamId, "Unique Player Beta");
        var players = new List<Player> { player1, player2 }.AsReadOnly();

        _gameStateService.HumanPlayerTeamId.Returns(teamId);
        _playerRepository.GetByTeamId(teamId).Returns(players);
        _playerStatsService.GetTeamAverages(Arg.Any<IReadOnlyList<Player>>())
            .Returns(new TeamAverages());

        var sut = CreateSut();

        // Act
        sut.GetTeamAverages();

        // Assert - verify exact players were passed
        _playerStatsService.Received(1).GetTeamAverages(Arg.Is<IReadOnlyList<Player>>(
            p => p.Contains(player1) && p.Contains(player2)));
    }

    // =========================================================================
    // Integration behavior - consistent player list across calls
    // =========================================================================

    [Fact]
    public void AllMethods_UseConsistentHumanPlayerTeamId()
    {
        // Arrange - ensure all methods query the same team
        var teamId = Guid.NewGuid();
        var players = new List<Player> { CreatePlayer(teamId) }.AsReadOnly();

        _gameStateService.HumanPlayerTeamId.Returns(teamId);
        _playerRepository.GetByTeamId(teamId).Returns(players);
        _playerStatsService.GetTeamTotals(Arg.Any<IReadOnlyList<Player>>())
            .Returns(new TeamTotals());
        _playerStatsService.GetTeamAverages(Arg.Any<IReadOnlyList<Player>>())
            .Returns(new TeamAverages());

        var sut = CreateSut();

        // Act
        sut.GetPlayers();
        sut.GetTeamTotals();
        sut.GetTeamAverages();

        // Assert - all calls should use the same team ID
        _playerRepository.Received(3).GetByTeamId(teamId);
    }
}
