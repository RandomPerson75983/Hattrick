namespace Hattrick.Core.Services;

/// <summary>
/// Default implementation of IGameStateService.
/// </summary>
public class GameStateService : IGameStateService
{
    private const int DefaultTurnNumber = 1;
    private const int DefaultSeasonNumber = 1;
    private const int DefaultWeekNumber = 1;

    /// <inheritdoc />
    public int CurrentSeasonNumber { get; set; } = DefaultSeasonNumber;

    /// <inheritdoc />
    public int CurrentWeekNumber { get; set; } = DefaultWeekNumber;

    /// <inheritdoc />
    public int CurrentTurnNumber { get; set; } = DefaultTurnNumber;

    /// <inheritdoc />
    public bool IsGameLoaded { get; set; }

    /// <inheritdoc />
    public int? CurrentSaveSlot { get; set; }

    /// <inheritdoc />
    public Guid? HumanPlayerTeamId { get; set; }

    /// <inheritdoc />
    public void Reset()
    {
        CurrentSeasonNumber = DefaultSeasonNumber;
        CurrentWeekNumber = DefaultWeekNumber;
        CurrentTurnNumber = DefaultTurnNumber;
        IsGameLoaded = false;
        CurrentSaveSlot = null;
        HumanPlayerTeamId = null;
    }
}
