using Hattrick.Core;
using Hattrick.Core.Repositories;
using Hattrick.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hattrick.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddHattrickCoreServices_WhenCalled_RegistersAllServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHattrickCoreServices();
        var container = services.BuildServiceProvider();

        // Act & Assert — GetRequiredService throws if not registered
        container.GetRequiredService<IRandomProvider>().Should().NotBeNull();
        container.GetRequiredService<IDateTimeProvider>().Should().NotBeNull();
        container.GetRequiredService<ISaveGameService>().Should().NotBeNull();
        container.GetRequiredService<ISaveSlotService>().Should().NotBeNull();
        container.GetRequiredService<IGameStateService>().Should().NotBeNull();
        container.GetRequiredService<IPlayerRepository>().Should().NotBeNull();
        container.GetRequiredService<ITeamRepository>().Should().NotBeNull();
        container.GetRequiredService<IPlayerDisplayService>().Should().NotBeNull();
        container.GetRequiredService<IPlayerStatsService>().Should().NotBeNull();
    }

    [Fact]
    public void AddHattrickCoreServices_SharedServices_AreSingletons()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHattrickCoreServices();
        var container = services.BuildServiceProvider();

        // Act
        var service1 = container.GetRequiredService<IGameStateService>();
        var service2 = container.GetRequiredService<IGameStateService>();
        var repo1 = container.GetRequiredService<IPlayerRepository>();
        var repo2 = container.GetRequiredService<IPlayerRepository>();
        var teamRepo1 = container.GetRequiredService<ITeamRepository>();
        var teamRepo2 = container.GetRequiredService<ITeamRepository>();
        var displayService1 = container.GetRequiredService<IPlayerDisplayService>();
        var displayService2 = container.GetRequiredService<IPlayerDisplayService>();
        var statsService1 = container.GetRequiredService<IPlayerStatsService>();
        var statsService2 = container.GetRequiredService<IPlayerStatsService>();

        // Assert (same instance = singleton)
        service1.Should().BeSameAs(service2);
        repo1.Should().BeSameAs(repo2);
        teamRepo1.Should().BeSameAs(teamRepo2);
        displayService1.Should().BeSameAs(displayService2);
        statsService1.Should().BeSameAs(statsService2);
    }

    [Fact]
    public void AddHattrickCoreServices_Interfaces_ResolveToCorrectConcreteTypes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHattrickCoreServices();
        var container = services.BuildServiceProvider();

        // Act & Assert — verify concrete types, not just interface assignability
        container.GetRequiredService<IRandomProvider>().Should().BeOfType<RandomProvider>();
        container.GetRequiredService<IDateTimeProvider>().Should().BeOfType<DateTimeProvider>();
        container.GetRequiredService<ISaveGameService>().Should().BeOfType<SaveGameService>();
        container.GetRequiredService<ISaveSlotService>().Should().BeOfType<SaveSlotService>();
        container.GetRequiredService<IGameStateService>().Should().BeOfType<GameStateService>();
        container.GetRequiredService<IPlayerRepository>().Should().BeOfType<PlayerRepository>();
        container.GetRequiredService<ITeamRepository>().Should().BeOfType<TeamRepository>();
        container.GetRequiredService<IPlayerDisplayService>().Should().BeOfType<PlayerDisplayService>();
        container.GetRequiredService<IPlayerStatsService>().Should().BeOfType<PlayerStatsService>();
    }
}
