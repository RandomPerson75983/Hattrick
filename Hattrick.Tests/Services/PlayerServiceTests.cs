using Hattrick.Core.Models;
using Hattrick.Core.Repositories;
using Hattrick.Core.Services;
using NSubstitute;

namespace Hattrick.Tests.Services;

/// <summary>
/// Unit tests for IPlayerService.
///
/// PlayerService wraps IPlayerRepository and exposes a service-layer API so
/// that Blazor components never depend on a repository directly.
///
/// These tests are RED until IPlayerService and PlayerService are created.
/// </summary>
public class PlayerServiceTests
{
    private readonly IPlayerRepository _repoMock;
    private readonly IPlayerService _sut;

    public PlayerServiceTests()
    {
        _repoMock = Substitute.For<IPlayerRepository>();
        _sut = new PlayerService(_repoMock);
    }

    // ─────────────────────────────────────────────────────────────
    // GetAllPlayers
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void GetAllPlayers_WhenCalled_DelegatesToRepository()
    {
        // Arrange
        _repoMock.GetByTeamId(Guid.Empty).Returns(new List<Player>());

        // Act
        _sut.GetAllPlayers();

        // Assert
        _repoMock.Received(1).GetByTeamId(Guid.Empty);
    }

    [Fact]
    public void GetAllPlayers_WithPlayers_ReturnsAllPlayers()
    {
        // Arrange
        var player1 = new Player { TeamId = Guid.Empty, Name = "Aldric Voss", Age = 22 };
        var player2 = new Player { TeamId = Guid.Empty, Name = "Bram Holst", Age = 25 };
        _repoMock.GetByTeamId(Guid.Empty).Returns(new List<Player> { player1, player2 });

        // Act
        var result = _sut.GetAllPlayers();

        // Assert
        result.Count.Should().Be(2);
    }

    [Fact]
    public void GetAllPlayers_WhenRepositoryReturnsEmpty_ReturnsEmptyList()
    {
        // Arrange
        _repoMock.GetByTeamId(Guid.Empty).Returns(new List<Player>());

        // Act
        var result = _sut.GetAllPlayers();

        // Assert
        result.Should().BeEmpty();
    }
}
