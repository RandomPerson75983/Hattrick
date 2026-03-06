using Hattrick.Core.Services;

namespace Hattrick.Tests.Services;

public class SaveGameServiceTests : IDisposable
{
    private readonly string _testDir;
    private readonly SaveGameService _service;

    public SaveGameServiceTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"hattrick_test_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDir);
        _service = new SaveGameService();
    }

    [Fact]
    public async Task SaveAsync_CreatesJsonFile()
    {
        // Arrange
        string filePath = Path.Combine(_testDir, "test_save.json");
        var data = new TestGameState(1, 5);

        // Act
        await _service.SaveAsync(filePath, data);

        // Assert
        File.Exists(filePath).Should().BeTrue();
    }

    [Fact]
    public async Task SaveAsync_WritesValidJson()
    {
        // Arrange
        string filePath = Path.Combine(_testDir, "test_save.json");
        var data = new TestGameState(2, 10);

        // Act
        await _service.SaveAsync(filePath, data);

        // Assert
        var json = await File.ReadAllTextAsync(filePath);
        json.Should().Contain("\"seasonNumber\"");
        json.Should().Contain("\"week\"");
    }

    [Fact]
    public async Task LoadAsync_ReturnsDeserializedObject()
    {
        // Arrange
        string filePath = Path.Combine(_testDir, "test_save.json");
        var data = new TestGameState(3, 8);
        await _service.SaveAsync(filePath, data);

        // Act
        var loaded = await _service.LoadAsync<TestGameState>(filePath);

        // Assert
        loaded.Should().NotBeNull();
        loaded!.SeasonNumber.Should().Be(3);
        loaded.Week.Should().Be(8);
    }

    [Fact]
    public async Task LoadAsync_WithNonExistentFile_ReturnsNull()
    {
        // Arrange
        string filePath = Path.Combine(_testDir, "nonexistent.json");

        // Act
        var loaded = await _service.LoadAsync<TestGameState>(filePath);

        // Assert
        loaded.Should().BeNull();
    }

    [Fact]
    public async Task SaveAsync_Overwrite_ReplacesExistingFile()
    {
        // Arrange
        string filePath = Path.Combine(_testDir, "test_overwrite.json");
        var data1 = new TestGameState(1, 1);
        var data2 = new TestGameState(5, 16);

        await _service.SaveAsync(filePath, data1);

        // Act
        await _service.SaveAsync(filePath, data2);

        // Assert
        var loaded = await _service.LoadAsync<TestGameState>(filePath);
        loaded!.SeasonNumber.Should().Be(5);
        loaded.Week.Should().Be(16);
    }

    [Fact]
    public void SaveFileExists_WithExistingFile_ReturnsTrue()
    {
        // Arrange
        string filePath = Path.Combine(_testDir, "test_exists.json");
        File.WriteAllText(filePath, "{}");

        // Act
        bool exists = _service.SaveFileExists(filePath);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public void SaveFileExists_WithNonExistentFile_ReturnsFalse()
    {
        // Arrange
        string filePath = Path.Combine(_testDir, "nonexistent.json");

        // Act
        bool exists = _service.SaveFileExists(filePath);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public void DeleteSaveFile_DeletesFile()
    {
        // Arrange
        string filePath = Path.Combine(_testDir, "test_delete.json");
        File.WriteAllText(filePath, "{}");
        File.Exists(filePath).Should().BeTrue();

        // Act
        _service.DeleteSaveFile(filePath);

        // Assert
        File.Exists(filePath).Should().BeFalse();
    }

    [Fact]
    public void DeleteSaveFile_WithNonExistentFile_DoesNotThrow()
    {
        // Arrange
        string filePath = Path.Combine(_testDir, "nonexistent.json");

        // Act
        var action = () => _service.DeleteSaveFile(filePath);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public async Task SaveAsync_RoundTrip_PreservesData()
    {
        // Arrange
        string filePath = Path.Combine(_testDir, "test_roundtrip.json");
        var original = new TestGameState(7, 14);

        // Act
        await _service.SaveAsync(filePath, original);
        var loaded = await _service.LoadAsync<TestGameState>(filePath);

        // Assert
        loaded.Should().NotBeNull();
        loaded!.SeasonNumber.Should().Be(original.SeasonNumber);
        loaded.Week.Should().Be(original.Week);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, recursive: true);
    }

    private record TestGameState(int SeasonNumber, int Week);
}
