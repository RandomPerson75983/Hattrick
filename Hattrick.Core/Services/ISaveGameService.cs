namespace Hattrick.Core.Services;

/// <summary>
/// Handles serialization and deserialization of game state to/from JSON files.
/// Uses atomic write pattern (temp-file → rename) to prevent corruption.
/// </summary>
public interface ISaveGameService
{
    /// <summary>
    /// Saves a game state object to a JSON file atomically.
    /// Writes to a temporary file first, then renames to the target path.
    /// If write fails, the temporary file is cleaned up and target file remains unchanged.
    /// </summary>
    /// <param name="filePath">The target file path (will be overwritten if exists).</param>
    /// <param name="gameState">The object to serialize.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task SaveAsync(string filePath, object gameState, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a game state object from a JSON file.
    /// </summary>
    /// <typeparam name="T">The type of game state to deserialize.</typeparam>
    /// <param name="filePath">The file path to load from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deserialized game state, or null if file does not exist.</returns>
    /// <exception cref="System.Text.Json.JsonException">If JSON is invalid.</exception>
    Task<T?> LoadAsync<T>(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a save file exists at the given path.
    /// </summary>
    bool SaveFileExists(string filePath);

    /// <summary>
    /// Deletes a save file if it exists.
    /// </summary>
    void DeleteSaveFile(string filePath);
}
