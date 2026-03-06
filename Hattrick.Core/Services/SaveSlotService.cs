namespace Hattrick.Core.Services;

/// <summary>
/// Default implementation of ISaveSlotService.
/// </summary>
public class SaveSlotService : ISaveSlotService
{
    private const int FirstManualSlot = 1;
    private const int LastManualSlot = 99;
    private const int FirstAutoSaveSlot = 100;
    private const int LastAutoSaveSlot = 109;
    private const int TotalAutoSaveSlots = LastAutoSaveSlot - FirstAutoSaveSlot + 1;

    /// <inheritdoc />
    public string SaveDirectory { get; }

    public SaveSlotService(string? customSaveDir = null)
    {
        SaveDirectory = customSaveDir ?? GetDefaultSaveDirectory();

        // Ensure directory exists
        if (!Directory.Exists(SaveDirectory))
            Directory.CreateDirectory(SaveDirectory);
    }

    /// <inheritdoc />
    public string GetSlotFilePath(int slotNumber)
    {
        if (slotNumber < FirstManualSlot || slotNumber > LastAutoSaveSlot)
            throw new ArgumentOutOfRangeException(nameof(slotNumber),
                $"Slot number must be between {FirstManualSlot} and {LastAutoSaveSlot}");

        string fileName = $"save_slot_{slotNumber:000}.json";
        return Path.Combine(SaveDirectory, fileName);
    }

    /// <inheritdoc />
    public IReadOnlyList<SaveSlotInfo> GetAllSaveSlots()
    {
        var slots = new List<SaveSlotInfo>();

        for (int slot = FirstManualSlot; slot <= LastAutoSaveSlot; slot++)
        {
            string filePath = GetSlotFilePath(slot);
            if (!File.Exists(filePath))
                continue;

            var info = new FileInfo(filePath);
            bool isAutoSave = slot >= FirstAutoSaveSlot;

            slots.Add(new SaveSlotInfo(
                SlotNumber: slot,
                FilePath: filePath,
                LastModified: info.LastWriteTimeUtc,
                FileSizeBytes: info.Length,
                IsAutoSave: isAutoSave));
        }

        return slots.OrderBy(s => s.SlotNumber).ToList();
    }

    /// <inheritdoc />
    public bool SaveSlotExists(int slotNumber)
    {
        string filePath = GetSlotFilePath(slotNumber);
        return File.Exists(filePath);
    }

    /// <inheritdoc />
    public int GetMostRecentAutoSaveSlot()
    {
        var autoSaves = GetAllSaveSlots()
            .Where(s => s.IsAutoSave)
            .OrderByDescending(s => s.LastModified)
            .FirstOrDefault();

        return autoSaves?.SlotNumber ?? -1;
    }

    /// <inheritdoc />
    public int GetNextAutoSaveSlot()
    {
        // Find the oldest auto-save slot and reuse it (round-robin)
        var autoSaves = GetAllSaveSlots()
            .Where(s => s.IsAutoSave)
            .OrderBy(s => s.LastModified)
            .FirstOrDefault();

        // If we have an old auto-save, rotate to the next slot
        if (autoSaves != null)
        {
            int nextSlot = autoSaves.SlotNumber + 1;
            if (nextSlot > LastAutoSaveSlot)
                nextSlot = FirstAutoSaveSlot;
            return nextSlot;
        }

        // No auto-saves yet, start with the first slot
        return FirstAutoSaveSlot;
    }

    private static string GetDefaultSaveDirectory()
    {
        // Use platform-specific application data directory
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, "Hattrick", "Saves");
    }
}
