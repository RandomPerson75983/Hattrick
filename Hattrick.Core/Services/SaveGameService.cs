using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hattrick.Core.Services;

/// <summary>
/// Default implementation of ISaveGameService using System.Text.Json.
/// </summary>
public class SaveGameService : ISaveGameService
{
    private readonly JsonSerializerOptions _jsonOptions;

    public SaveGameService()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
    }

    /// <inheritdoc />
    public async Task SaveAsync(string filePath, object gameState, CancellationToken cancellationToken = default)
    {
        if (gameState == null)
            throw new ArgumentNullException(nameof(gameState));

        string tempPath = $"{filePath}.tmp";
        string directory = Path.GetDirectoryName(filePath) ?? ".";

        try
        {
            // Ensure directory exists
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Serialize to JSON
            var json = JsonSerializer.Serialize(gameState, _jsonOptions);

            // Write to temporary file
            await File.WriteAllTextAsync(tempPath, json, cancellationToken);

            // Atomic rename: delete target if exists, then move temp to target
            if (File.Exists(filePath))
                File.Delete(filePath);

            File.Move(tempPath, filePath);
        }
        catch
        {
            // Clean up temp file if something went wrong
            if (File.Exists(tempPath))
                File.Delete(tempPath);

            throw;
        }
    }

    /// <inheritdoc />
    public async Task<T?> LoadAsync<T>(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            return default;

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    /// <inheritdoc />
    public bool SaveFileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    /// <inheritdoc />
    public void DeleteSaveFile(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
