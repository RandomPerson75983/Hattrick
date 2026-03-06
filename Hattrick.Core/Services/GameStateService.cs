namespace Hattrick.Core.Services;

/// <summary>
/// Default implementation of IGameStateService.
/// </summary>
public class GameStateService : IGameStateService
{
    /// <inheritdoc />
    public int CurrentSeasonNumber { get; set; } = 1;

    /// <inheritdoc />
    public int CurrentWeekNumber { get; set; } = 1;

    /// <inheritdoc />
    public bool IsGameLoaded { get; set; }

    /// <inheritdoc />
    public int? CurrentSaveSlot { get; set; }

    /// <inheritdoc />
    public void Reset()
    {
        CurrentSeasonNumber = 1;
        CurrentWeekNumber = 1;
        IsGameLoaded = false;
        CurrentSaveSlot = null;
    }
}
