namespace Hattrick.Core.Services;

/// <summary>
/// Holds the current in-memory game state.
/// This is a singleton service that maintains game data during the current session.
/// </summary>
public interface IGameStateService
{
    /// <summary>
    /// Gets or sets the current season number (1-based).
    /// </summary>
    int CurrentSeasonNumber { get; set; }

    /// <summary>
    /// Gets or sets the current week number (1-16 per season).
    /// </summary>
    int CurrentWeekNumber { get; set; }

    /// <summary>
    /// Gets or sets the current turn within the week (1-4).
    /// Turn 1: Pre-midweek match. Turn 2: Post-midweek management.
    /// Turn 3: Pre-weekend match. Turn 4: End of week processing.
    /// </summary>
    int CurrentTurnNumber { get; set; }

    /// <summary>
    /// Gets or sets whether a game is currently loaded/active.
    /// </summary>
    bool IsGameLoaded { get; set; }

    /// <summary>
    /// Gets the save slot number of the currently loaded game (if loaded).
    /// </summary>
    int? CurrentSaveSlot { get; set; }

    /// <summary>
    /// Gets or sets the team ID of the human player's team.
    /// All other teams in the league are AI-managed.
    /// </summary>
    Guid? HumanPlayerTeamId { get; set; }

    /// <summary>
    /// Resets the game state to default (used when starting a new game or loading).
    /// </summary>
    void Reset();
}
