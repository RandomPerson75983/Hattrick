using Hattrick.Core.Models;
using Hattrick.Core.Services;
using NSubstitute;

namespace Hattrick.Tests.Services;

/// <summary>
/// Unit tests for ITeamGenerationService.
///
/// TeamGenerationService generates a complete team with appropriate player distribution.
/// Uses IPlayerGenerationService for player generation, which is mocked in tests.
///
/// Requirements:
/// - GenerateTeam(string teamName, bool isHumanControlled) returns Team with populated squad
/// - Uses IPlayerGenerationService to generate individual players
/// - Position distribution: 3 Keepers, 6 Defenders (CentralDefender/WingBack mix),
///   8 Midfielders (InnerMidfielder/Winger mix), 8 Forwards = 25 total
/// - Team properties: Name and IsHumanControlled from parameters
/// - Reasonable defaults: Budget ~10M, TeamSpirit 5, Confidence 5, CoachLevel 5, etc.
/// - All generated players must have TeamId set to match the team's Id
/// </summary>
public class TeamGenerationServiceTests
{
    private readonly IPlayerGenerationService _playerGenMock;
    private readonly ITeamGenerationService _sut;

    // Constants matching expected implementation defaults
    private const decimal ExpectedDefaultBudget = 10_000_000m;
    private const double ExpectedDefaultTeamSpirit = 5.0;
    private const double ExpectedDefaultConfidence = 5.0;
    private const int ExpectedDefaultCoachLevel = 5;

    // Position distribution constants
    private const int ExpectedKeeperCount = 3;
    private const int ExpectedDefenderCount = 6;
    private const int ExpectedMidfielderCount = 8;
    private const int ExpectedForwardCount = 8;
    private const int ExpectedTotalPlayers = 25;

    // Test fixture constants
    private const int TestPlayerAge = 25;
    private const int TestPlayerForm = 6;
    private const int TestPlayerStamina = 7;
    private const int TestPlayerExperience = 3;
    private const double TestSkillValue = 5.0;

    public TeamGenerationServiceTests()
    {
        _playerGenMock = Substitute.For<IPlayerGenerationService>();
        SetupPlayerGenerationMock();
        _sut = new TeamGenerationService(_playerGenMock);
    }

    // -------------------------------------------------------------------------
    // Constructor / DI Tests
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_WithNullPlayerGenerationService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new TeamGenerationService(null!));
        ex.ParamName.Should().Be("playerGenerationService");
    }

    // -------------------------------------------------------------------------
    // GenerateTeam - Team Name Parameter
    // -------------------------------------------------------------------------

    [Fact]
    public void GenerateTeam_WithTeamName_SetsTeamNameCorrectly()
    {
        // Arrange
        const string teamName = "FC Test Club";

        // Act
        var (team, _) = _sut.GenerateTeam(teamName, isHumanControlled: true);

        // Assert
        team.Name.Should().Be(teamName);
    }

    [Fact]
    public void GenerateTeam_WithEmptyTeamName_SetsEmptyName()
    {
        // Arrange
        const string teamName = "";

        // Act
        var (team, _) = _sut.GenerateTeam(teamName, isHumanControlled: true);

        // Assert
        team.Name.Should().BeEmpty();
    }

    [Fact]
    public void GenerateTeam_WithWhitespaceTeamName_SetsWhitespaceName()
    {
        // Arrange
        const string teamName = "   ";

        // Act
        var (team, _) = _sut.GenerateTeam(teamName, isHumanControlled: true);

        // Assert
        team.Name.Should().Be("   ");
    }

    [Theory]
    [InlineData("Simple Name")]
    [InlineData("FC United 2024")]
    [InlineData("Team-With-Dashes")]
    [InlineData("Team With Special Chars!@#")]
    public void GenerateTeam_WithVariousTeamNames_SetsNameCorrectly(string teamName)
    {
        // Act
        var (team, _) = _sut.GenerateTeam(teamName, isHumanControlled: false);

        // Assert
        team.Name.Should().Be(teamName);
    }

    // -------------------------------------------------------------------------
    // GenerateTeam - IsHumanControlled Parameter
    // -------------------------------------------------------------------------

    [Fact]
    public void GenerateTeam_WithIsHumanControlledTrue_SetsIsHumanControlledTrue()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert
        team.IsHumanControlled.Should().BeTrue();
    }

    [Fact]
    public void GenerateTeam_WithIsHumanControlledFalse_SetsIsHumanControlledFalse()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: false);

        // Assert
        team.IsHumanControlled.Should().BeFalse();
    }

    // -------------------------------------------------------------------------
    // GenerateTeam - Team Identity
    // -------------------------------------------------------------------------

    [Fact]
    public void GenerateTeam_WhenCalled_ReturnsTeamWithNonEmptyId()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert
        team.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void GenerateTeam_CalledMultipleTimes_ReturnsUniqueTeamIds()
    {
        // Arrange
        const int teamCount = 10;

        // Act
        var teamIds = Enumerable.Range(0, teamCount)
            .Select(i => _sut.GenerateTeam($"Team {i}", isHumanControlled: true).Team.Id)
            .ToList();

        // Assert
        teamIds.Should().OnlyHaveUniqueItems("each team should have a unique Id");
    }

    // -------------------------------------------------------------------------
    // GenerateTeam - Reasonable Defaults (Budget, TeamSpirit, Confidence, etc.)
    // -------------------------------------------------------------------------

    [Fact]
    public void GenerateTeam_WhenCalled_SetsBudgetToReasonableDefault()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Budget should be around 10M (allow some variance)
        team.Budget.Should().BeInRange(5_000_000m, 15_000_000m,
            "budget should be a reasonable starting amount around 10M");
    }

    [Fact]
    public void GenerateTeam_WhenCalled_SetsBudgetToExpectedDefault()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Exact default value
        team.Budget.Should().Be(ExpectedDefaultBudget);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_SetsTeamSpiritToReasonableDefault()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: TeamSpirit should be in valid range [0.0, 10.0] with neutral default
        team.TeamSpirit.Should().BeInRange(0.0, 10.0);
        team.TeamSpirit.Should().Be(ExpectedDefaultTeamSpirit);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_SetsConfidenceToReasonableDefault()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Confidence should be in valid range [0.0, 10.0] with neutral default
        team.Confidence.Should().BeInRange(0.0, 10.0);
        team.Confidence.Should().Be(ExpectedDefaultConfidence);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_SetsCoachLevelToReasonableDefault()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: CoachLevel should be within valid bounds [1, 8]
        team.CoachLevel.Should().BeInRange(Team.MinCoachLevel, Team.MaxCoachLevel);
        team.CoachLevel.Should().Be(ExpectedDefaultCoachLevel);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_SetsAssistantCoachLevelToZero()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: New teams should not have assistant coach hired
        team.AssistantCoachLevel.Should().Be(0);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_SetsDoctorLevelToZero()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: New teams should not have doctor hired
        team.DoctorLevel.Should().Be(0);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_SetsSpokespersonLevelToZero()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: New teams should not have spokesperson hired
        team.SpokespersonLevel.Should().Be(0);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_SetsFinancialDirectorLevelToZero()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: New teams should not have financial director hired
        team.FinancialDirectorLevel.Should().Be(0);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_SetsFansToReasonableDefault()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Should have default fans (1000)
        team.Fans.Should().Be(1000);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_SetsFanClubSizeToReasonableDefault()
    {
        // Act
        var (team, _) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Should have default fan club size (100)
        team.FanClubSize.Should().Be(100);
    }

    // -------------------------------------------------------------------------
    // GenerateTeam - Player Generation
    // -------------------------------------------------------------------------

    [Fact]
    public void GenerateTeam_WhenCalled_CallsPlayerGenerationServiceForKeepers()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Should call GeneratePlayer for keeper position 3 times
        _playerGenMock.Received(ExpectedKeeperCount).GeneratePlayer(Position.Keeper);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_CallsPlayerGenerationServiceForDefenders()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Should call GeneratePlayer for defender positions 6 times total
        // Mix of CentralDefender and WingBack
        var centralDefenderCalls = _playerGenMock.ReceivedCalls()
            .Count(c => c.GetMethodInfo().Name == "GeneratePlayer" &&
                       c.GetArguments().Contains(Position.CentralDefender));
        var wingBackCalls = _playerGenMock.ReceivedCalls()
            .Count(c => c.GetMethodInfo().Name == "GeneratePlayer" &&
                       c.GetArguments().Contains(Position.WingBack));

        (centralDefenderCalls + wingBackCalls).Should().Be(ExpectedDefenderCount,
            "should generate 6 defenders (mix of CentralDefender and WingBack)");
    }

    [Fact]
    public void GenerateTeam_WhenCalled_CallsPlayerGenerationServiceForMidfielders()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Should call GeneratePlayer for midfielder positions 8 times total
        // Mix of InnerMidfielder and Winger
        var innerMidCalls = _playerGenMock.ReceivedCalls()
            .Count(c => c.GetMethodInfo().Name == "GeneratePlayer" &&
                       c.GetArguments().Contains(Position.InnerMidfielder));
        var wingerCalls = _playerGenMock.ReceivedCalls()
            .Count(c => c.GetMethodInfo().Name == "GeneratePlayer" &&
                       c.GetArguments().Contains(Position.Winger));

        (innerMidCalls + wingerCalls).Should().Be(ExpectedMidfielderCount,
            "should generate 8 midfielders (mix of InnerMidfielder and Winger)");
    }

    [Fact]
    public void GenerateTeam_WhenCalled_CallsPlayerGenerationServiceForForwards()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Should call GeneratePlayer for forward position 8 times
        _playerGenMock.Received(ExpectedForwardCount).GeneratePlayer(Position.Forward);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_GeneratesExactly25Players()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Total player generation calls should be 25
        _playerGenMock.ReceivedWithAnyArgs(ExpectedTotalPlayers).GeneratePlayer(default);
    }

    // -------------------------------------------------------------------------
    // GenerateTeam - Player Position Distribution (Return Value Verification)
    // -------------------------------------------------------------------------

    [Fact]
    public void GenerateTeam_WhenCalled_Returns25PlayersAttachedToTeam()
    {
        // Act
        var (_, players) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Verify 25 players are returned
        players.Should().HaveCount(ExpectedTotalPlayers);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_GeneratesCorrectKeeperCount()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert
        _playerGenMock.Received(ExpectedKeeperCount).GeneratePlayer(Position.Keeper);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_GeneratesCorrectDefenderDistribution()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: 6 defenders should be a mix (e.g., 4 CentralDefender + 2 WingBack)
        var calls = _playerGenMock.ReceivedCalls()
            .Where(c => c.GetMethodInfo().Name == "GeneratePlayer")
            .Select(c => c.GetArguments()[0])
            .Cast<Position>()
            .ToList();

        var defenderCount = calls.Count(p => p == Position.CentralDefender || p == Position.WingBack);
        defenderCount.Should().Be(ExpectedDefenderCount);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_GeneratesCorrectMidfielderDistribution()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: 8 midfielders should be a mix (e.g., 4 InnerMidfielder + 4 Winger)
        var calls = _playerGenMock.ReceivedCalls()
            .Where(c => c.GetMethodInfo().Name == "GeneratePlayer")
            .Select(c => c.GetArguments()[0])
            .Cast<Position>()
            .ToList();

        var midfielderCount = calls.Count(p => p == Position.InnerMidfielder || p == Position.Winger);
        midfielderCount.Should().Be(ExpectedMidfielderCount);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_GeneratesCorrectForwardCount()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert
        _playerGenMock.Received(ExpectedForwardCount).GeneratePlayer(Position.Forward);
    }

    // -------------------------------------------------------------------------
    // GenerateTeam - Player TeamId Assignment
    // -------------------------------------------------------------------------

    [Fact]
    public void GenerateTeam_WhenCalled_SetsTeamIdOnAllGeneratedPlayers()
    {
        // Act
        var (team, players) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: All generated players should have TeamId set to team's Id
        players.Should().HaveCount(ExpectedTotalPlayers);
        players.Should().OnlyContain(p => p.TeamId == team.Id,
            "all generated players should have TeamId matching the team's Id");
    }

    [Fact]
    public void GenerateTeam_WhenCalled_DoesNotLeavePlayersWithEmptyTeamId()
    {
        // Act
        var (_, players) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: No player should have empty TeamId
        players.Should().NotContain(p => p.TeamId == Guid.Empty,
            "no player should have empty TeamId after team generation");
    }

    [Fact]
    public void GenerateTeam_CalledTwice_AssignsDifferentTeamIdsToPlayers()
    {
        // Act
        var (team1, team1Players) = _sut.GenerateTeam("Team 1", isHumanControlled: true);
        var (team2, team2Players) = _sut.GenerateTeam("Team 2", isHumanControlled: false);

        // Assert
        team1Players.Should().OnlyContain(p => p.TeamId == team1.Id);
        team2Players.Should().OnlyContain(p => p.TeamId == team2.Id);
        team1.Id.Should().NotBe(team2.Id);
    }

    // -------------------------------------------------------------------------
    // GenerateTeam - Integration with PlayerGenerationService (Returned Players)
    // -------------------------------------------------------------------------

    [Fact]
    public void GenerateTeam_ReturnsPlayersFromPlayerGenerationService()
    {
        // Arrange: Create a specific mock player that we can verify is returned
        var mockPlayer = CreateTestPlayer(Position.Forward);
        _playerGenMock.GeneratePlayer(Position.Forward).Returns(mockPlayer);

        // Act
        var (_, players) = _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Verify the mock player is in the returned list
        players.Should().Contain(mockPlayer);
    }

    // -------------------------------------------------------------------------
    // GenerateTeam - Position Mix Distribution Tests
    // -------------------------------------------------------------------------

    [Fact]
    public void GenerateTeam_WhenCalled_GeneratesSomeCentralDefenders()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Should have at least some CentralDefenders
        _playerGenMock.Received().GeneratePlayer(Position.CentralDefender);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_GeneratesSomeWingBacks()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Should have at least some WingBacks
        _playerGenMock.Received().GeneratePlayer(Position.WingBack);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_GeneratesSomeInnerMidfielders()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Should have at least some InnerMidfielders
        _playerGenMock.Received().GeneratePlayer(Position.InnerMidfielder);
    }

    [Fact]
    public void GenerateTeam_WhenCalled_GeneratesSomeWingers()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Should have at least some Wingers
        _playerGenMock.Received().GeneratePlayer(Position.Winger);
    }

    // -------------------------------------------------------------------------
    // Boundary Tests - Over-Correction Prevention
    // -------------------------------------------------------------------------

    [Fact]
    public void GenerateTeam_WhenCalled_DoesNotGenerateTooManyPlayers()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Should not exceed 25 players
        var totalCalls = _playerGenMock.ReceivedCalls()
            .Count(c => c.GetMethodInfo().Name == "GeneratePlayer");
        totalCalls.Should().Be(ExpectedTotalPlayers,
            "should generate exactly 25 players, not more");
    }

    [Fact]
    public void GenerateTeam_WhenCalled_DoesNotGenerateTooFewPlayers()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: Should not be less than 25 players
        var totalCalls = _playerGenMock.ReceivedCalls()
            .Count(c => c.GetMethodInfo().Name == "GeneratePlayer");
        totalCalls.Should().Be(ExpectedTotalPlayers,
            "should generate exactly 25 players, not fewer");
    }

    [Fact]
    public void GenerateTeam_WhenCalled_DoesNotGenerateInvalidPositions()
    {
        // Act
        _sut.GenerateTeam("Test Team", isHumanControlled: true);

        // Assert: All generated positions should be valid enum values
        var positions = _playerGenMock.ReceivedCalls()
            .Where(c => c.GetMethodInfo().Name == "GeneratePlayer")
            .Select(c => c.GetArguments()[0])
            .Cast<Position>()
            .ToList();

        positions.Should().OnlyContain(p => Enum.IsDefined(typeof(Position), p));
    }

    // -------------------------------------------------------------------------
    // Edge Cases
    // -------------------------------------------------------------------------

    [Fact]
    public void GenerateTeam_WithNullTeamName_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => _sut.GenerateTeam(null!, isHumanControlled: true));
        ex.ParamName.Should().Be("teamName");
    }

    [Fact]
    public void GenerateTeam_HumanAndAiTeams_HaveSamePlayerDistribution()
    {
        // Arrange
        _playerGenMock.ClearReceivedCalls();

        // Act
        _sut.GenerateTeam("Human Team", isHumanControlled: true);
        var humanTeamCalls = _playerGenMock.ReceivedCalls()
            .Where(c => c.GetMethodInfo().Name == "GeneratePlayer")
            .Select(c => c.GetArguments()[0])
            .Cast<Position>()
            .ToList();

        _playerGenMock.ClearReceivedCalls();

        _sut.GenerateTeam("AI Team", isHumanControlled: false);
        var aiTeamCalls = _playerGenMock.ReceivedCalls()
            .Where(c => c.GetMethodInfo().Name == "GeneratePlayer")
            .Select(c => c.GetArguments()[0])
            .Cast<Position>()
            .ToList();

        // Assert: Both should have same distribution
        humanTeamCalls.Count.Should().Be(aiTeamCalls.Count);
        humanTeamCalls.Count(p => p == Position.Keeper).Should().Be(aiTeamCalls.Count(p => p == Position.Keeper));
        humanTeamCalls.Count(p => p == Position.Forward).Should().Be(aiTeamCalls.Count(p => p == Position.Forward));
    }

    // -------------------------------------------------------------------------
    // Helper Methods
    // -------------------------------------------------------------------------

    /// <summary>
    /// Sets up the player generation mock to return valid test players.
    /// </summary>
    private void SetupPlayerGenerationMock()
    {
        _playerGenMock.GeneratePlayer(Arg.Any<Position>()).Returns(callInfo =>
        {
            var position = callInfo.ArgAt<Position>(0);
            return CreateTestPlayer(position);
        });
    }

    /// <summary>
    /// Creates a test player for the given position with minimal required properties.
    /// </summary>
    private static Player CreateTestPlayer(Position position)
    {
        return new Player
        {
            Id = Guid.NewGuid(),
            Name = $"Test Player {position}",
            Age = TestPlayerAge,
            BestPosition = position,
            Form = TestPlayerForm,
            Stamina = TestPlayerStamina,
            Experience = TestPlayerExperience,
            TeamId = Guid.Empty, // Service should set this
            Skills = new Dictionary<SkillType, double>
            {
                [SkillType.Keeper] = TestSkillValue,
                [SkillType.Defending] = TestSkillValue,
                [SkillType.Playmaking] = TestSkillValue,
                [SkillType.Winger] = TestSkillValue,
                [SkillType.Passing] = TestSkillValue,
                [SkillType.Scoring] = TestSkillValue,
                [SkillType.SetPieces] = TestSkillValue,
                [SkillType.Stamina] = TestSkillValue
            }
        };
    }
}
