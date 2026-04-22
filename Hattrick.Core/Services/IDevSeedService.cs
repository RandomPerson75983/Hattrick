namespace Hattrick.Core.Services;

/// <summary>
/// Service for seeding development data on app startup.
/// </summary>
public interface IDevSeedService
{
    /// <summary>
    /// Seeds development data including a human-controlled team with players.
    /// This method is idempotent - if data already exists, it does nothing.
    /// </summary>
    Task SeedAsync();
}
