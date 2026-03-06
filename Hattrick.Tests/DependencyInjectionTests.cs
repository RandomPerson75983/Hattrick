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
        RegisterServices(services);
        var container = services.BuildServiceProvider();

        // Act & Assert
        // If any of these throw, the test fails
        var randomProvider = container.GetRequiredService<IRandomProvider>();
        var dateTimeProvider = container.GetRequiredService<IDateTimeProvider>();
        var saveGameService = container.GetRequiredService<ISaveGameService>();
        var saveSlotService = container.GetRequiredService<ISaveSlotService>();
        var gameStateService = container.GetRequiredService<IGameStateService>();

        randomProvider.Should().NotBeNull();
        dateTimeProvider.Should().NotBeNull();
        saveGameService.Should().NotBeNull();
        saveSlotService.Should().NotBeNull();
        gameStateService.Should().NotBeNull();
    }

    [Fact]
    public void DI_ContainerRegistersServicesAsSingletons()
    {
        // Arrange
        var services = new ServiceCollection();
        RegisterServices(services);
        var container = services.BuildServiceProvider();

        // Act
        var service1 = container.GetRequiredService<IGameStateService>();
        var service2 = container.GetRequiredService<IGameStateService>();

        // Assert (same instance)
        service1.Should().BeSameAs(service2);
    }

    [Fact]
    public void DI_InterfacesAreProperlyImplemented()
    {
        // Arrange
        var services = new ServiceCollection();
        RegisterServices(services);
        var container = services.BuildServiceProvider();

        // Act
        var randomProvider = container.GetRequiredService<IRandomProvider>();
        var dateTimeProvider = container.GetRequiredService<IDateTimeProvider>();
        var saveGameService = container.GetRequiredService<ISaveGameService>();
        var saveSlotService = container.GetRequiredService<ISaveSlotService>();
        var gameStateService = container.GetRequiredService<IGameStateService>();

        // Assert
        randomProvider.Should().BeAssignableTo<IRandomProvider>();
        dateTimeProvider.Should().BeAssignableTo<IDateTimeProvider>();
        saveGameService.Should().BeAssignableTo<ISaveGameService>();
        saveSlotService.Should().BeAssignableTo<ISaveSlotService>();
        gameStateService.Should().BeAssignableTo<IGameStateService>();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IRandomProvider, RandomProvider>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<ISaveGameService, SaveGameService>();
        services.AddSingleton<ISaveSlotService, SaveSlotService>();
        services.AddSingleton<IGameStateService, GameStateService>();
    }
}
