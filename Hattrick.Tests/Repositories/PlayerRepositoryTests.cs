using Hattrick.Core.Models;
using Hattrick.Core.Repositories;

namespace Hattrick.Tests.Repositories;

public class PlayerRepositoryTests
{
    private readonly PlayerRepository _sut = new();

    private static Player CreatePlayer(Guid? teamId = null, string name = "Test Player")
    {
        return new Player
        {
            TeamId = teamId ?? Guid.NewGuid(),
            Name = name,
            Age = 22,
            AgeDays = 50,
            Form = 6,
            Stamina = 7,
            Experience = 5,
        };
    }

    #region Add

    [Fact]
    public void Add_PlayerIsRetrievableAfterAdd()
    {
        // Arrange
        var player = CreatePlayer();

        // Act
        _sut.Add(player);
        var result = _sut.GetById(player.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(player.Id);
        result.Name.Should().Be(player.Name);
        result.TeamId.Should().Be(player.TeamId);
    }

    [Fact]
    public void Add_MultiplePlayersAreAllRetrievable()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var player1 = CreatePlayer(teamId, "Player One");
        var player2 = CreatePlayer(teamId, "Player Two");
        var player3 = CreatePlayer(teamId, "Player Three");

        // Act
        _sut.Add(player1);
        _sut.Add(player2);
        _sut.Add(player3);

        // Assert
        _sut.GetById(player1.Id)!.Name.Should().Be("Player One");
        _sut.GetById(player2.Id)!.Name.Should().Be("Player Two");
        _sut.GetById(player3.Id)!.Name.Should().Be("Player Three");
        _sut.GetByTeamId(teamId).Should().HaveCount(3);
    }

    [Fact]
    public void Add_NullPlayer_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.Add(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Add_DuplicateId_ThrowsOrReplacesGracefully()
    {
        // Arrange
        var player = CreatePlayer(name: "Original");
        _sut.Add(player);

        var duplicate = new Player
        {
            Id = player.Id,
            TeamId = player.TeamId,
            Name = "Duplicate",
            Age = 25,
        };

        // Act
        var act = () => _sut.Add(duplicate);

        // Assert - either throws ArgumentException or replaces (implementation decides)
        // The key contract: after this, GetById returns a consistent result
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region GetById

    [Fact]
    public void GetById_ReturnsPlayerWhenExists()
    {
        // Arrange
        var player = CreatePlayer(name: "Findable Player");
        _sut.Add(player);

        // Act
        var result = _sut.GetById(player.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(player.Id);
        result.Name.Should().Be("Findable Player");
        result.Age.Should().Be(22);
        result.TeamId.Should().Be(player.TeamId);
    }

    [Fact]
    public void GetById_ReturnsNullWhenNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = _sut.GetById(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetById_EmptyRepository_ReturnsNull()
    {
        // Act
        var result = _sut.GetById(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetById_DoesNotReturnPlayerFromDifferentId()
    {
        // Arrange
        var player = CreatePlayer();
        _sut.Add(player);

        // Act
        var result = _sut.GetById(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetByTeamId

    [Fact]
    public void GetByTeamId_ReturnsAllPlayersForTeam()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var player1 = CreatePlayer(teamId, "Team Player 1");
        var player2 = CreatePlayer(teamId, "Team Player 2");
        var player3 = CreatePlayer(teamId, "Team Player 3");

        _sut.Add(player1);
        _sut.Add(player2);
        _sut.Add(player3);

        // Act
        var result = _sut.GetByTeamId(teamId);

        // Assert
        result.Should().HaveCount(3);
        result.Select(p => p.Name).Should().Contain(new[] { "Team Player 1", "Team Player 2", "Team Player 3" });
    }

    [Fact]
    public void GetByTeamId_ReturnsEmptyListForUnknownTeam()
    {
        // Arrange
        var player = CreatePlayer();
        _sut.Add(player);

        // Act
        var result = _sut.GetByTeamId(Guid.NewGuid());

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetByTeamId_ReturnsIReadOnlyList()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var player = CreatePlayer(teamId, "Type Check Player");
        _sut.Add(player);

        // Act
        var result = _sut.GetByTeamId(teamId);

        // Assert
        result.Should().BeAssignableTo<IReadOnlyList<Player>>();
        result.Should().ContainSingle()
            .Which.Name.Should().Be("Type Check Player");
    }

    [Fact]
    public void GetByTeamId_EmptyRepository_ReturnsEmptyList()
    {
        // Act
        var result = _sut.GetByTeamId(Guid.NewGuid());

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region Update

    [Fact]
    public void Update_ReplacesPlayerData()
    {
        // Arrange
        var player = CreatePlayer(name: "Original Name");
        _sut.Add(player);

        var updatedPlayer = new Player
        {
            Id = player.Id,
            TeamId = player.TeamId,
            Name = "Updated Name",
            Age = 30,
            Form = 8,
        };

        // Act
        _sut.Update(updatedPlayer);
        var result = _sut.GetById(player.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.Age.Should().Be(30);
        result.Form.Should().Be(8);
    }

    [Fact]
    public void Update_PreservesPlayerId()
    {
        // Arrange
        var player = CreatePlayer(name: "Original");
        var originalId = player.Id;
        _sut.Add(player);

        var updatedPlayer = new Player
        {
            Id = originalId,
            TeamId = player.TeamId,
            Name = "Changed",
        };

        // Act
        _sut.Update(updatedPlayer);
        var result = _sut.GetById(originalId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(originalId);
        result.Name.Should().Be("Changed");
    }

    [Fact]
    public void Update_NullPlayer_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.Update(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Update_NonExistentPlayer_ThrowsOrHandlesGracefully()
    {
        // Arrange
        var player = CreatePlayer();

        // Act
        var act = () => _sut.Update(player);

        // Assert - should throw because the player was never added
        act.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void Update_DoesNotAffectOtherPlayers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var player1 = CreatePlayer(teamId, "Player One");
        var player2 = CreatePlayer(teamId, "Player Two");
        _sut.Add(player1);
        _sut.Add(player2);

        var updatedPlayer1 = new Player
        {
            Id = player1.Id,
            TeamId = teamId,
            Name = "Updated Player One",
        };

        // Act
        _sut.Update(updatedPlayer1);

        // Assert
        var unchanged = _sut.GetById(player2.Id);
        unchanged.Should().NotBeNull();
        unchanged!.Name.Should().Be("Player Two");
    }

    [Fact]
    public void Update_CanChangeTeamId()
    {
        // Arrange
        var originalTeamId = Guid.NewGuid();
        var newTeamId = Guid.NewGuid();
        var player = CreatePlayer(originalTeamId, "Transferring Player");
        _sut.Add(player);

        var transferredPlayer = new Player
        {
            Id = player.Id,
            TeamId = newTeamId,
            Name = "Transferring Player",
        };

        // Act
        _sut.Update(transferredPlayer);

        // Assert
        var result = _sut.GetById(player.Id);
        result.Should().NotBeNull();
        result!.TeamId.Should().Be(newTeamId);

        // Player should now appear under new team, not old team
        _sut.GetByTeamId(newTeamId).Should().Contain(p => p.Id == player.Id);
        _sut.GetByTeamId(originalTeamId).Should().NotContain(p => p.Id == player.Id);
    }

    #endregion

    #region Remove

    [Fact]
    public void Remove_RemovesPlayerById()
    {
        // Arrange
        var player = CreatePlayer();
        _sut.Add(player);

        // Act
        _sut.Remove(player.Id);

        // Assert
        _sut.GetById(player.Id).Should().BeNull();
    }

    [Fact]
    public void Remove_NonExistentPlayer_DoesNotThrow()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var act = () => _sut.Remove(nonExistentId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Remove_PlayerNoLongerAppearsInTeamResults()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var player1 = CreatePlayer(teamId, "Staying");
        var player2 = CreatePlayer(teamId, "Leaving");
        _sut.Add(player1);
        _sut.Add(player2);

        // Act
        _sut.Remove(player2.Id);

        // Assert
        var teamPlayers = _sut.GetByTeamId(teamId);
        teamPlayers.Should().HaveCount(1);
        teamPlayers.Should().Contain(p => p.Name == "Staying");
        teamPlayers.Should().NotContain(p => p.Name == "Leaving");
    }

    [Fact]
    public void Remove_ThenGetById_ReturnsNull()
    {
        // Arrange
        var player = CreatePlayer();
        _sut.Add(player);

        // Act
        _sut.Remove(player.Id);
        var result = _sut.GetById(player.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Remove_DoesNotAffectOtherPlayers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var player1 = CreatePlayer(teamId, "Keep Me");
        var player2 = CreatePlayer(teamId, "Remove Me");
        _sut.Add(player1);
        _sut.Add(player2);

        // Act
        _sut.Remove(player2.Id);

        // Assert
        _sut.GetById(player1.Id).Should().NotBeNull();
        _sut.GetById(player1.Id)!.Name.Should().Be("Keep Me");
    }

    [Fact]
    public void Remove_EmptyRepository_DoesNotThrow()
    {
        // Act
        var act = () => _sut.Remove(Guid.NewGuid());

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Isolation - Players from different teams

    [Fact]
    public void GetByTeamId_PlayersFromDifferentTeamsDontMix()
    {
        // Arrange
        var teamAId = Guid.NewGuid();
        var teamBId = Guid.NewGuid();
        var playerA1 = CreatePlayer(teamAId, "Team A Player 1");
        var playerA2 = CreatePlayer(teamAId, "Team A Player 2");
        var playerB1 = CreatePlayer(teamBId, "Team B Player 1");
        var playerB2 = CreatePlayer(teamBId, "Team B Player 2");
        var playerB3 = CreatePlayer(teamBId, "Team B Player 3");

        _sut.Add(playerA1);
        _sut.Add(playerA2);
        _sut.Add(playerB1);
        _sut.Add(playerB2);
        _sut.Add(playerB3);

        // Act
        var teamAPlayers = _sut.GetByTeamId(teamAId);
        var teamBPlayers = _sut.GetByTeamId(teamBId);

        // Assert
        teamAPlayers.Should().HaveCount(2);
        teamAPlayers.Should().OnlyContain(p => p.TeamId == teamAId);
        teamAPlayers.Select(p => p.Name).Should().BeEquivalentTo(new[] { "Team A Player 1", "Team A Player 2" });

        teamBPlayers.Should().HaveCount(3);
        teamBPlayers.Should().OnlyContain(p => p.TeamId == teamBId);
        teamBPlayers.Select(p => p.Name).Should().BeEquivalentTo(new[] { "Team B Player 1", "Team B Player 2", "Team B Player 3" });
    }

    [Fact]
    public void Remove_FromOneTeam_DoesNotAffectOtherTeam()
    {
        // Arrange
        var teamAId = Guid.NewGuid();
        var teamBId = Guid.NewGuid();
        var playerA = CreatePlayer(teamAId, "Team A Player");
        var playerB = CreatePlayer(teamBId, "Team B Player");
        _sut.Add(playerA);
        _sut.Add(playerB);

        // Act
        _sut.Remove(playerA.Id);

        // Assert
        _sut.GetByTeamId(teamBId).Should().ContainSingle()
            .Which.Name.Should().Be("Team B Player");
        _sut.GetByTeamId(teamAId).Should().BeEmpty();
    }

    #endregion

    #region Thread Safety

    [Fact]
    public void ConcurrentAdds_DoNotCorruptState()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        const int iterationCount = 200;

        // Act
        Parallel.For(0, iterationCount, i =>
        {
            var player = CreatePlayer(teamId, $"Concurrent Player {i}");
            _sut.Add(player);
        });

        // Assert
        var allPlayers = _sut.GetByTeamId(teamId);
        allPlayers.Should().HaveCount(iterationCount);
    }

    [Fact]
    public void ConcurrentReadsAndWrites_DoNotThrow()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        const int iterationCount = 100;
        var preloadedPlayers = new List<Player>();

        // Pre-load some players
        for (var i = 0; i < 50; i++)
        {
            var player = CreatePlayer(teamId, $"Preloaded {i}");
            _sut.Add(player);
            preloadedPlayers.Add(player);
        }

        // Act - concurrent reads and writes
        var act = () => Parallel.For(0, iterationCount, i =>
        {
            if (i % 3 == 0)
            {
                // Add
                var player = CreatePlayer(teamId, $"New Player {i}");
                _sut.Add(player);
            }
            else if (i % 3 == 1)
            {
                // Read by team
                var _ = _sut.GetByTeamId(teamId);
            }
            else
            {
                // Read by id
                var idx = i % preloadedPlayers.Count;
                var _ = _sut.GetById(preloadedPlayers[idx].Id);
            }
        });

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ConcurrentAddAndRemove_DoNotCorruptState()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        const int iterationCount = 100;
        var playersToRemove = new List<Player>();

        // Pre-load players to remove
        for (var i = 0; i < iterationCount; i++)
        {
            var player = CreatePlayer(teamId, $"Removable {i}");
            _sut.Add(player);
            playersToRemove.Add(player);
        }

        // Act - concurrently add new players and remove existing ones
        var act = () => Parallel.For(0, iterationCount, i =>
        {
            // Remove one
            _sut.Remove(playersToRemove[i].Id);
            // Add a new one
            var newPlayer = CreatePlayer(teamId, $"Replacement {i}");
            _sut.Add(newPlayer);
        });

        // Assert
        act.Should().NotThrow();
        // All original players should be removed
        foreach (var player in playersToRemove)
        {
            _sut.GetById(player.Id).Should().BeNull();
        }
    }

    [Fact]
    public void ConcurrentUpdates_DoNotCorruptState()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var player = CreatePlayer(teamId, "Shared Player");
        _sut.Add(player);
        const int iterationCount = 200;

        // Act - multiple threads updating the same player
        var act = () => Parallel.For(0, iterationCount, i =>
        {
            var updatedPlayer = new Player
            {
                Id = player.Id,
                TeamId = teamId,
                Name = $"Updated by thread {i}",
                Age = 20 + (i % 15),
            };
            _sut.Update(updatedPlayer);
        });

        // Assert
        act.Should().NotThrow();
        var result = _sut.GetById(player.Id);
        result.Should().NotBeNull();
        result!.Name.Should().StartWith("Updated by thread ");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void GetByTeamId_AfterAllPlayersRemoved_ReturnsEmptyList()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var player1 = CreatePlayer(teamId, "Player 1");
        var player2 = CreatePlayer(teamId, "Player 2");
        _sut.Add(player1);
        _sut.Add(player2);

        // Act
        _sut.Remove(player1.Id);
        _sut.Remove(player2.Id);

        // Assert
        _sut.GetByTeamId(teamId).Should().BeEmpty();
    }

    [Fact]
    public void Add_ThenRemove_ThenAddAgain_WorksCorrectly()
    {
        // Arrange
        var player = CreatePlayer(name: "Comeback Player");
        _sut.Add(player);
        _sut.Remove(player.Id);

        // Act - re-add with same Id
        _sut.Add(player);
        var result = _sut.GetById(player.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Comeback Player");
    }

    [Fact]
    public void PlayerRepository_ImplementsIPlayerRepository()
    {
        // Assert
        _sut.Should().BeAssignableTo<IPlayerRepository>();
    }

    [Fact]
    public void GetByTeamId_ReturnedListIsSnapshot_NotLiveReference()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var player1 = CreatePlayer(teamId, "Player 1");
        _sut.Add(player1);

        // Act
        var snapshot = _sut.GetByTeamId(teamId);

        // Add another player after taking the snapshot
        var player2 = CreatePlayer(teamId, "Player 2");
        _sut.Add(player2);

        // Assert - snapshot should still have only 1 player
        snapshot.Should().HaveCount(1);

        // But a fresh query should return 2
        _sut.GetByTeamId(teamId).Should().HaveCount(2);
    }

    [Fact]
    public void Update_AllPlayerFields_ArePersisted()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var player = CreatePlayer(teamId, "Full Update Test");
        _sut.Add(player);

        var updatedPlayer = new Player
        {
            Id = player.Id,
            TeamId = teamId,
            Name = "Updated Full",
            Age = 28,
            AgeDays = 100,
            Form = 8,
            Stamina = 9,
            Experience = 15,
            Specialty = Specialty.Technical,
            InjuryWeeks = 2,
            YellowCards = 1,
            Wage = 5000m,
        };

        // Act
        _sut.Update(updatedPlayer);
        var result = _sut.GetById(player.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Full");
        result.Age.Should().Be(28);
        result.AgeDays.Should().Be(100);
        result.Form.Should().Be(8);
        result.Stamina.Should().Be(9);
        result.Experience.Should().Be(15);
        result.Specialty.Should().Be(Specialty.Technical);
        result.InjuryWeeks.Should().Be(2);
        result.YellowCards.Should().Be(1);
        result.Wage.Should().Be(5000m);
    }

    #endregion
}
