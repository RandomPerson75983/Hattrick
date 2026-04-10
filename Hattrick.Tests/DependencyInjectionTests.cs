using Hattrick.Core;
using Hattrick.Core.Repositories;
using Hattrick.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hattrick.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void DI_ContainerRegistersAllInfrastructureServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHattrickCoreServices();
        var container = services.BuildServiceProvider();

        // Act & Assert — GetRequiredService throws if not registered
        container.GetRequiredService<IRandomProvider>();
        container.GetRequiredService<IDateTimeProvider>();
        container.GetRequiredService<ISaveGameService>();
        container.GetRequiredService<ISaveSlotService>();
        container.GetRequiredService<IGameStateService>();
        container.GetRequiredService<IPlayerRepository>();
        container.GetRequiredService<ITeamRepository>();
    }

    [Fact]
    public void DI_ContainerRegistersServicesAsSingletons()
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

        // Assert (same instance = singleton)
        service1.Should().BeSameAs(service2);
        repo1.Should().BeSameAs(repo2);
        teamRepo1.Should().BeSameAs(teamRepo2);
    }

    [Fact]
    public void DI_InterfacesResolveToCorrectConcreteTypes()
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
    }
}
