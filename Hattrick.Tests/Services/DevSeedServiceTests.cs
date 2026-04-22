using FluentAssertions;
using Hattrick.Core.Models;
using Hattrick.Core.Repositories;
using Hattrick.Core.Services;
using NSubstitute;

namespace Hattrick.Tests.Services;

/// <summary>
/// Unit tests for IDevSeedService + DevSeedService.
///
/// DevSeedService seeds development data on app startup.
/// SeedAsync() creates one human-controlled team with 25 players.
///
/// Requirements:
/// - Uses ITeamGenerationService to generate the team
/// - Populates IPlayerRepository with all players
/// - Populates ITeamRepository with the team
/// - Sets IGameStateService.HumanPlayerTeamId to the team's Id
/// - Must be idempotent (if HumanPlayerTeamId is already set, don't re-seed)
/// - Team is marked as human-controlled (isHumanControlled = true)
/// </summary>
public class DevSeedServiceTests
{
    private readonly ITeamGenerationService _teamGenerationServiceMock;
    private readonly IPlayerRepository _playerRepositoryMock;
    private readonly ITeamRepository _teamRepositoryMock;
    private readonly IGameStateService _gameStateServiceMock;
    private readonly IDevSeedService _sut;

    // Constants for expected values
    private const string ExpectedTeamName = "FC Development";
    private const int ExpectedPlayerCount = 25;

    public DevSeedServiceTests()
    {
        _teamGenerationServiceMock = Substitute.For<ITeamGenerationService>();
        _playerRepositoryMock = Substitute.For<IPlayerRepository>();
        _teamRepositoryMock = Substitute.For<ITeamRepository>();
        _gameStateServiceMock = Substitute.For<IGameStateService>();

        SetupDefaultMocks();

        _sut = new DevSeedService(
            _teamGenerationServiceMock,
            _playerRepositoryMock,
            _teamRepositoryMock,
            _gameStateServiceMock);
    }

    // -------------------------------------------------------------------------
    // Constructor / DI Tests - Null Checks
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_WithNullTeamGenerationService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new DevSeedService(
            null!,
            _playerRepositoryMock,
            _teamRepositoryMock,
            _gameStateServiceMock));
        ex.ParamName.Should().Be("teamGenerationService");
    }

    [Fact]
    public void Constructor_WithNullPlayerRepository_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new DevSeedService(
            _teamGenerationServiceMock,
            null!,
            _teamRepositoryMock,
            _gameStateServiceMock));
        ex.ParamName.Should().Be("playerRepository");
    }

    [Fact]
    public void Constructor_WithNullTeamRepository_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new DevSeedService(
            _teamGenerationServiceMock,
            _playerRepositoryMock,
            null!,
            _gameStateServiceMock));
        ex.ParamName.Should().Be("teamRepository");
    }

    [Fact]
    public void Constructor_WithNullGameStateService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new DevSeedService(
            _teamGenerationServiceMock,
            _playerRepositoryMock,
            _teamRepositoryMock,
            null!));
        ex.ParamName.Should().Be("gameStateService");
    }

    [Fact]
    public void Constructor_WithAllValidDependencies_DoesNotThrow()
    {
        // Arrange & Act
        var service = new DevSeedService(
            _teamGenerationServiceMock,
            _playerRepositoryMock,
            _teamRepositoryMock,
            _gameStateServiceMock);

        // Assert
        service.Should().NotBeNull();
    }

    // -------------------------------------------------------------------------
    // SeedAsync - Team Generation
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_WhenCalled_GeneratesTeamWithCorrectName()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);

        // Act
        await _sut.SeedAsync();

        // Assert
        _teamGenerationServiceMock.Received(1).GenerateTeam(ExpectedTeamName, Arg.Any<bool>());
    }

    [Fact]
    public async Task SeedAsync_WhenCalled_GeneratesTeamWithIsHumanControlledTrue()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);

        // Act
        await _sut.SeedAsync();

        // Assert
        _teamGenerationServiceMock.Received(1).GenerateTeam(Arg.Any<string>(), isHumanControlled: true);
    }

    [Fact]
    public async Task SeedAsync_WhenCalled_GeneratesTeamWithCorrectNameAndIsHumanControlled()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);

        // Act
        await _sut.SeedAsync();

        // Assert: Verify exact parameters
        _teamGenerationServiceMock.Received(1).GenerateTeam(ExpectedTeamName, isHumanControlled: true);
    }

    // -------------------------------------------------------------------------
    // SeedAsync - Player Repository Population
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_WhenCalled_AddsAllPlayersToPlayerRepository()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        // Act
        await _sut.SeedAsync();

        // Assert: Should add each player to the repository
        _playerRepositoryMock.Received(ExpectedPlayerCount).Add(Arg.Any<Player>());
    }

    [Fact]
    public async Task SeedAsync_WhenCalled_AddsEachPlayerFromGeneratedTeam()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        var addedPlayers = new List<Player>();
        _playerRepositoryMock.When(x => x.Add(Arg.Any<Player>()))
            .Do(callInfo => addedPlayers.Add(callInfo.ArgAt<Player>(0)));

        // Act
        await _sut.SeedAsync();

        // Assert: All generated players should be added
        addedPlayers.Should().HaveCount(ExpectedPlayerCount);
        addedPlayers.Should().OnlyContain(p => p.TeamId == team.Id,
            "all added players should have TeamId matching the generated team");
    }

    [Fact]
    public async Task SeedAsync_WhenCalled_AddsPlayersWithUniqueIds()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        var addedPlayers = new List<Player>();
        _playerRepositoryMock.When(x => x.Add(Arg.Any<Player>()))
            .Do(callInfo => addedPlayers.Add(callInfo.ArgAt<Player>(0)));

        // Act
        await _sut.SeedAsync();

        // Assert: All player IDs should be unique
        var playerIds = addedPlayers.Select(p => p.Id).ToList();
        playerIds.Should().OnlyHaveUniqueItems("each player should have a unique Id");
    }

    // -------------------------------------------------------------------------
    // SeedAsync - Team Repository Population
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_WhenCalled_AddsTeamToTeamRepository()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        // Act
        await _sut.SeedAsync();

        // Assert
        _teamRepositoryMock.Received(1).Add(team);
    }

    [Fact]
    public async Task SeedAsync_WhenCalled_AddsTeamWithCorrectId()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        Team? addedTeam = null;
        _teamRepositoryMock.When(x => x.Add(Arg.Any<Team>()))
            .Do(callInfo => addedTeam = callInfo.ArgAt<Team>(0));

        // Act
        await _sut.SeedAsync();

        // Assert
        addedTeam.Should().NotBeNull();
        addedTeam!.Id.Should().Be(team.Id);
    }

    [Fact]
    public async Task SeedAsync_WhenCalled_AddsTeamWithIsHumanControlledTrue()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        Team? addedTeam = null;
        _teamRepositoryMock.When(x => x.Add(Arg.Any<Team>()))
            .Do(callInfo => addedTeam = callInfo.ArgAt<Team>(0));

        // Act
        await _sut.SeedAsync();

        // Assert
        addedTeam.Should().NotBeNull();
        addedTeam!.IsHumanControlled.Should().BeTrue();
    }

    // -------------------------------------------------------------------------
    // SeedAsync - Game State Service Update
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_WhenCalled_SetsHumanPlayerTeamIdOnGameStateService()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        // Act
        await _sut.SeedAsync();

        // Assert
        _gameStateServiceMock.Received(1).HumanPlayerTeamId = team.Id;
    }

    [Fact]
    public async Task SeedAsync_WhenCalled_SetsHumanPlayerTeamIdToGeneratedTeamId()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        var expectedTeamId = team.Id;
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        Guid? setTeamId = null;
        _gameStateServiceMock.When(x => x.HumanPlayerTeamId = Arg.Any<Guid?>())
            .Do(callInfo => setTeamId = callInfo.ArgAt<Guid?>(0));

        // Act
        await _sut.SeedAsync();

        // Assert
        setTeamId.Should().Be(expectedTeamId);
    }

    // -------------------------------------------------------------------------
    // Idempotency Tests
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_WhenHumanPlayerTeamIdAlreadySet_DoesNotGenerateTeam()
    {
        // Arrange: HumanPlayerTeamId is already set (seeding was already done)
        _gameStateServiceMock.HumanPlayerTeamId.Returns(Guid.NewGuid());

        // Act
        await _sut.SeedAsync();

        // Assert
        _teamGenerationServiceMock.DidNotReceiveWithAnyArgs().GenerateTeam(Arg.Any<string>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task SeedAsync_WhenHumanPlayerTeamIdAlreadySet_DoesNotAddAnyPlayers()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns(Guid.NewGuid());

        // Act
        await _sut.SeedAsync();

        // Assert
        _playerRepositoryMock.DidNotReceiveWithAnyArgs().Add(Arg.Any<Player>());
    }

    [Fact]
    public async Task SeedAsync_WhenHumanPlayerTeamIdAlreadySet_DoesNotAddAnyTeam()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns(Guid.NewGuid());

        // Act
        await _sut.SeedAsync();

        // Assert
        _teamRepositoryMock.DidNotReceiveWithAnyArgs().Add(Arg.Any<Team>());
    }

    [Fact]
    public async Task SeedAsync_WhenHumanPlayerTeamIdAlreadySet_DoesNotModifyHumanPlayerTeamId()
    {
        // Arrange
        var existingTeamId = Guid.NewGuid();
        _gameStateServiceMock.HumanPlayerTeamId.Returns(existingTeamId);

        // Act
        await _sut.SeedAsync();

        // Assert: Should not receive any setter call
        _gameStateServiceMock.DidNotReceive().HumanPlayerTeamId = Arg.Any<Guid?>();
    }

    [Fact]
    public async Task SeedAsync_CalledTwice_OnlyGeneratesTeamOnce()
    {
        // Arrange: First call succeeds, second call should skip
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        // First call has null, second call has the team ID
        var callCount = 0;
        _gameStateServiceMock.HumanPlayerTeamId.Returns(callInfo =>
        {
            callCount++;
            return callCount == 1 ? null : team.Id;
        });

        // Act
        await _sut.SeedAsync();
        await _sut.SeedAsync();

        // Assert: GenerateTeam should only be called once
        _teamGenerationServiceMock.Received(1).GenerateTeam(Arg.Any<string>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task SeedAsync_CalledTwice_DoesNotCreateDuplicatePlayers()
    {
        // Arrange
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        var callCount = 0;
        _gameStateServiceMock.HumanPlayerTeamId.Returns(callInfo =>
        {
            callCount++;
            return callCount == 1 ? null : team.Id;
        });

        // Act
        await _sut.SeedAsync();
        await _sut.SeedAsync();

        // Assert: Should only add 25 players total (from first call)
        _playerRepositoryMock.Received(ExpectedPlayerCount).Add(Arg.Any<Player>());
    }

    [Fact]
    public async Task SeedAsync_CalledTwice_DoesNotCreateDuplicateTeams()
    {
        // Arrange
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        var callCount = 0;
        _gameStateServiceMock.HumanPlayerTeamId.Returns(callInfo =>
        {
            callCount++;
            return callCount == 1 ? null : team.Id;
        });

        // Act
        await _sut.SeedAsync();
        await _sut.SeedAsync();

        // Assert: Should only add 1 team total (from first call)
        _teamRepositoryMock.Received(1).Add(Arg.Any<Team>());
    }

    // -------------------------------------------------------------------------
    // Boundary/Edge Cases
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_WhenTeamGenerationReturnsTeam_AddsExactly25Players()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        // Act
        await _sut.SeedAsync();

        // Assert: Exactly 25 players, not more, not fewer
        _playerRepositoryMock.Received(ExpectedPlayerCount).Add(Arg.Any<Player>());
    }

    [Fact]
    public async Task SeedAsync_WhenCalled_AddsExactlyOneTeam()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        // Act
        await _sut.SeedAsync();

        // Assert
        _teamRepositoryMock.Received(1).Add(Arg.Any<Team>());
    }

    [Fact]
    public async Task SeedAsync_WhenTeamGenerationReturnsTeamWithNonEmptyId_UsesTeamIdForHumanPlayerTeamId()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        // Assert precondition: team has non-empty ID
        team.Id.Should().NotBe(Guid.Empty);

        // Act
        await _sut.SeedAsync();

        // Assert
        _gameStateServiceMock.Received(1).HumanPlayerTeamId = team.Id;
    }

    // -------------------------------------------------------------------------
    // Over-Correction / Adversarial Tests
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_WhenCalled_DoesNotRemoveAnyExistingPlayers()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        // Act
        await _sut.SeedAsync();

        // Assert: Remove should never be called
        _playerRepositoryMock.DidNotReceiveWithAnyArgs().Remove(Arg.Any<Guid>());
    }

    [Fact]
    public async Task SeedAsync_WhenCalled_DoesNotUpdateAnyExistingPlayers()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        // Act
        await _sut.SeedAsync();

        // Assert: Update should never be called
        _playerRepositoryMock.DidNotReceiveWithAnyArgs().Update(Arg.Any<Player>());
    }

    [Fact]
    public async Task SeedAsync_WhenCalled_DoesNotUpdateAnyExistingTeams()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        // Act
        await _sut.SeedAsync();

        // Assert: Update should never be called
        _teamRepositoryMock.DidNotReceiveWithAnyArgs().Update(Arg.Any<Team>());
    }

    [Fact]
    public async Task SeedAsync_WhenCalled_DoesNotResetGameState()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        // Act
        await _sut.SeedAsync();

        // Assert: Reset should never be called
        _gameStateServiceMock.DidNotReceive().Reset();
    }

    [Fact]
    public async Task SeedAsync_WhenAlreadySeeded_DoesNotResetGameState()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns(Guid.NewGuid());

        // Act
        await _sut.SeedAsync();

        // Assert
        _gameStateServiceMock.DidNotReceive().Reset();
    }

    // -------------------------------------------------------------------------
    // Operation Order Tests
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_WhenCalled_AddsTeamBeforeSettingHumanPlayerTeamId()
    {
        // Arrange
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));

        var operationOrder = new List<string>();
        _teamRepositoryMock.When(x => x.Add(Arg.Any<Team>()))
            .Do(_ => operationOrder.Add("AddTeam"));
        _gameStateServiceMock.When(x => x.HumanPlayerTeamId = Arg.Any<Guid?>())
            .Do(_ => operationOrder.Add("SetHumanPlayerTeamId"));

        // Act
        await _sut.SeedAsync();

        // Assert: Team should be added before setting HumanPlayerTeamId
        operationOrder.Should().ContainInOrder("AddTeam", "SetHumanPlayerTeamId");
    }

    // -------------------------------------------------------------------------
    // Helper Methods
    // -------------------------------------------------------------------------

    /// <summary>
    /// Sets up default mock behaviors.
    /// </summary>
    private void SetupDefaultMocks()
    {
        var (team, players) = CreateTestTeamWithPlayers();
        _teamGenerationServiceMock.GenerateTeam(Arg.Any<string>(), Arg.Any<bool>()).Returns((team, players));
        _gameStateServiceMock.HumanPlayerTeamId.Returns((Guid?)null);
    }

    /// <summary>
    /// Creates a test team with 25 players for testing.
    /// </summary>
    private static (Team Team, List<Player> Players) CreateTestTeamWithPlayers()
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = ExpectedTeamName,
            IsHumanControlled = true,
            Budget = 10_000_000m,
            TeamSpirit = 5.0,
            Confidence = 5.0,
            CoachLevel = 5
        };

        var players = new List<Player>();
        for (int i = 0; i < ExpectedPlayerCount; i++)
        {
            players.Add(CreateTestPlayer(team.Id, i));
        }

        return (team, players);
    }

    /// <summary>
    /// Creates a test player with minimal required properties.
    /// </summary>
    private static Player CreateTestPlayer(Guid teamId, int index)
    {
        return new Player
        {
            Id = Guid.NewGuid(),
            TeamId = teamId,
            Name = $"Test Player {index}",
            Age = 25,
            BestPosition = Position.Forward,
            Form = 6,
            Stamina = 7,
            Experience = 3,
            Skills = new Dictionary<SkillType, double>
            {
                [SkillType.Keeper] = 5,
                [SkillType.Defending] = 5,
                [SkillType.Playmaking] = 5,
                [SkillType.Winger] = 5,
                [SkillType.Passing] = 5,
                [SkillType.Scoring] = 5,
                [SkillType.SetPieces] = 5,
                [SkillType.Stamina] = 5
            }
        };
    }
}
