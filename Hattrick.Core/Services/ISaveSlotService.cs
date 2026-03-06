namespace Hattrick.Core.Services;

/// <summary>
/// Manages save slot file paths and naming conventions.
/// Slots 1-99 are manual saves (user-created).
/// Slots 100+ are auto-save sub-slots (system-managed, rotated).
/// </summary>
public interface ISaveSlotService
{
    /// <summary>
    /// Gets the directory where all save files are stored.
    /// </summary>
    string SaveDirectory { get; }

    /// <summary>
    /// Gets the file path for a given save slot.
    /// </summary>
    /// <param name="slotNumber">The slot number (1-99 for manual, 100+ for auto).</param>
    /// <returns>The full file path for that slot's save file.</returns>
    string GetSlotFilePath(int slotNumber);

    /// <summary>
    /// Gets metadata about all existing save slots.
    /// </summary>
    /// <returns>A list of slot info for all existing saves, sorted by slot number.</returns>
    IReadOnlyList<SaveSlotInfo> GetAllSaveSlots();

    /// <summary>
    /// Checks if a save file exists for the given slot.
    /// </summary>
    bool SaveSlotExists(int slotNumber);

    /// <summary>
    /// Gets the most recent auto-save slot number (100+).
    /// </summary>
    /// <returns>The slot number, or -1 if no auto-saves exist.</returns>
    int GetMostRecentAutoSaveSlot();

    /// <summary>
    /// Gets the next available auto-save slot number, rotating through slots 100-109.
    /// </summary>
    /// <returns>An auto-save slot number (100-109).</returns>
    int GetNextAutoSaveSlot();
}

/// <summary>
/// Information about a saved game slot.
/// </summary>
public record SaveSlotInfo(
    int SlotNumber,
    string FilePath,
    DateTime LastModified,
    long FileSizeBytes,
    bool IsAutoSave);
