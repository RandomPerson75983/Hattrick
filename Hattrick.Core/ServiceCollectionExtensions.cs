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

        return services;
    }
}
