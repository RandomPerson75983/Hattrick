using Hattrick.Core.Repositories;
using Hattrick.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hattrick.Core;

/// <summary>
/// Extension methods for registering Hattrick core services in the DI container.
/// Used by both the MAUI app and test setup to ensure consistent registration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Hattrick core services and repositories as singletons.
    /// </summary>
    public static IServiceCollection AddHattrickCoreServices(this IServiceCollection services)
    {
        // Infrastructure services
        services.AddSingleton<IRandomProvider, RandomProvider>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<ISaveGameService, SaveGameService>();
        services.AddSingleton<ISaveSlotService, SaveSlotService>();
        services.AddSingleton<IGameStateService, GameStateService>();

        // Repositories
        services.AddSingleton<IPlayerRepository, PlayerRepository>();
        services.AddSingleton<ITeamRepository, TeamRepository>();

        // Domain services
        services.AddSingleton<IPlayerService, PlayerService>();

        // Display services
        services.AddSingleton<IPlayerDisplayService, PlayerDisplayService>();

        // Stats services
        services.AddSingleton<IPlayerStatsService, PlayerStatsService>();

        // Generation services
        services.AddSingleton<IPlayerGenerationService, PlayerGenerationService>();
        services.AddSingleton<ITeamGenerationService, TeamGenerationService>();

        // Seeding services
        services.AddSingleton<IDevSeedService, DevSeedService>();

        return services;
    }
}
