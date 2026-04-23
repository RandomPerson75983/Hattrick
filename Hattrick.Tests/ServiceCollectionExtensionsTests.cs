using Hattrick.Core;
using Hattrick.Core.Repositories;
using Hattrick.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hattrick.Tests;

public class ServiceCollectionExtensionsTests
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceCollectionExtensionsTests()
    {
        var services = new ServiceCollection();
        services.AddHattrickCoreServices();
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersRandomProvider()
    {
        var service = _serviceProvider.GetService<IRandomProvider>();
        service.Should().NotBeNull();
        service.Should().BeOfType<RandomProvider>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersDateTimeProvider()
    {
        var service = _serviceProvider.GetService<IDateTimeProvider>();
        service.Should().NotBeNull();
        service.Should().BeOfType<DateTimeProvider>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersSaveGameService()
    {
        var service = _serviceProvider.GetService<ISaveGameService>();
        service.Should().NotBeNull();
        service.Should().BeOfType<SaveGameService>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersSaveSlotService()
    {
        var service = _serviceProvider.GetService<ISaveSlotService>();
        service.Should().NotBeNull();
        service.Should().BeOfType<SaveSlotService>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersGameStateService()
    {
        var service = _serviceProvider.GetService<IGameStateService>();
        service.Should().NotBeNull();
        service.Should().BeOfType<GameStateService>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersPlayerRepository()
    {
        var service = _serviceProvider.GetService<IPlayerRepository>();
        service.Should().NotBeNull();
        service.Should().BeOfType<PlayerRepository>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersTeamRepository()
    {
        var service = _serviceProvider.GetService<ITeamRepository>();
        service.Should().NotBeNull();
        service.Should().BeOfType<TeamRepository>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersPlayerService()
    {
        var service = _serviceProvider.GetService<IPlayerService>();
        service.Should().NotBeNull();
        service.Should().BeOfType<PlayerService>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersPlayerDisplayService()
    {
        var service = _serviceProvider.GetService<IPlayerDisplayService>();
        service.Should().NotBeNull();
        service.Should().BeOfType<PlayerDisplayService>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersPlayerStatsService()
    {
        var service = _serviceProvider.GetService<IPlayerStatsService>();
        service.Should().NotBeNull();
        service.Should().BeOfType<PlayerStatsService>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersPlayersPageService()
    {
        var service = _serviceProvider.GetService<IPlayersPageService>();
        service.Should().NotBeNull();
        service.Should().BeOfType<PlayersPageService>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersPlayerGenerationService()
    {
        var service = _serviceProvider.GetService<IPlayerGenerationService>();
        service.Should().NotBeNull();
        service.Should().BeOfType<PlayerGenerationService>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersTeamGenerationService()
    {
        var service = _serviceProvider.GetService<ITeamGenerationService>();
        service.Should().NotBeNull();
        service.Should().BeOfType<TeamGenerationService>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersDevSeedService()
    {
        var service = _serviceProvider.GetService<IDevSeedService>();
        service.Should().NotBeNull();
        service.Should().BeOfType<DevSeedService>();
    }

    [Fact]
    public void AddHattrickCoreServices_AllServicesAreSingleton()
    {
        var service1 = _serviceProvider.GetService<IGameStateService>();
        var service2 = _serviceProvider.GetService<IGameStateService>();
        service1.Should().BeSameAs(service2);
    }

    [Fact]
    public void AddHattrickCoreServices_RepositoriesAreSingleton()
    {
        var repo1 = _serviceProvider.GetService<IPlayerRepository>();
        var repo2 = _serviceProvider.GetService<IPlayerRepository>();
        repo1.Should().BeSameAs(repo2);
    }

    [Fact]
    public void AddHattrickCoreServices_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        var result = services.AddHattrickCoreServices();
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersLineupService()
    {
        var service = _serviceProvider.GetService<ILineupService>();
        service.Should().NotBeNull();
        service.Should().BeOfType<LineupService>();
    }

    [Fact]
    public void AddHattrickCoreServices_RegistersLineupPageService()
    {
        var service = _serviceProvider.GetService<ILineupPageService>();
        service.Should().NotBeNull();
        service.Should().BeOfType<LineupPageService>();
    }

    [Fact]
    public void AddHattrickCoreServices_Registers16Services()
    {
        var services = new ServiceCollection();
        services.AddHattrickCoreServices();
        services.Should().HaveCount(16);
    }
}
