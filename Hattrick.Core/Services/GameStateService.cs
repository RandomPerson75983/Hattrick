using System.Threading;

namespace Hattrick.Core.Services;

/// <summary>
/// Default implementation of IGameStateService.
/// </summary>
public class GameStateService : IGameStateService
{
    private const int DefaultTurnNumber = 1;
    private const int DefaultSeasonNumber = 1;
    private const int DefaultWeekNumber = 1;

    private readonly Lock _lock = new();
    private int _currentSeasonNumber = DefaultSeasonNumber;
    private int _currentWeekNumber = DefaultWeekNumber;
    private int _currentTurnNumber = DefaultTurnNumber;
    private bool _isGameLoaded;
    private int? _currentSaveSlot;
    private Guid? _humanPlayerTeamId;

    /// <inheritdoc />
    public int CurrentSeasonNumber
    {
        get { lock (_lock) { return _currentSeasonNumber; } }
        set { lock (_lock) { _currentSeasonNumber = value; } }
    }

    /// <inheritdoc />
    public int CurrentWeekNumber
    {
        get { lock (_lock) { return _currentWeekNumber; } }
        set { lock (_lock) { _currentWeekNumber = value; } }
    }

    /// <inheritdoc />
    public int CurrentTurnNumber
    {
        get { lock (_lock) { return _currentTurnNumber; } }
        set { lock (_lock) { _currentTurnNumber = value; } }
    }

    /// <inheritdoc />
    public bool IsGameLoaded
    {
        get { lock (_lock) { return _isGameLoaded; } }
        set { lock (_lock) { _isGameLoaded = value; } }
    }

    /// <inheritdoc />
    public int? CurrentSaveSlot
    {
        get { lock (_lock) { return _currentSaveSlot; } }
        set { lock (_lock) { _currentSaveSlot = value; } }
    }

    /// <inheritdoc />
    public Guid? HumanPlayerTeamId
    {
        get { lock (_lock) { return _humanPlayerTeamId; } }
        set { lock (_lock) { _humanPlayerTeamId = value; } }
    }

    /// <inheritdoc />
    public void Reset()
    {
        lock (_lock)
        {
            _currentSeasonNumber = DefaultSeasonNumber;
            _currentWeekNumber = DefaultWeekNumber;
            _currentTurnNumber = DefaultTurnNumber;
            _isGameLoaded = false;
            _currentSaveSlot = null;
            _humanPlayerTeamId = null;
        }
    }
}
