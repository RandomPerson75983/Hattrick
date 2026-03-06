using Hattrick.Core.Services;

namespace Hattrick.Tests.Services;

public class SaveSlotServiceTests : IDisposable
{
    private readonly string _testDir;
    private readonly SaveSlotService _service;

    public SaveSlotServiceTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"hattrick_slots_{Guid.NewGuid()}");
        _service = new SaveSlotService(_testDir);
    }

    [Fact]
    public void GetSlotFilePath_ReturnsValidPath()
    {
        // Act
        string path = _service.GetSlotFilePath(1);

        // Assert
        path.Should().EndWith("save_slot_001.json");
        path.Should().StartWith(_testDir);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(99)]
    [InlineData(100)]
    [InlineData(109)]
    public void GetSlotFilePath_WithValidSlot_ReturnsPath(int slot)
    {
        // Act
        var path = _service.GetSlotFilePath(slot);

        // Assert
        path.Should().NotBeNullOrEmpty();
        File.Exists(path).Should().BeFalse(); // Path is just generated, not created
    }

    [Fact]
    public void GetSlotFilePath_WithInvalidSlot_ThrowsException()
    {
        // Act
        var action = () => _service.GetSlotFilePath(0);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetSlotFilePath_WithInvalidSlot_HighThrowsException()
    {
        // Act
        var action = () => _service.GetSlotFilePath(110);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SaveSlotExists_WithExistingSlot_ReturnsTrue()
    {
        // Arrange
        string path = _service.GetSlotFilePath(5);
        File.WriteAllText(path, "{}");

        // Act
        bool exists = _service.SaveSlotExists(5);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public void SaveSlotExists_WithNonExistentSlot_ReturnsFalse()
    {
        // Act
        bool exists = _service.SaveSlotExists(50);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public void GetAllSaveSlots_WithNoSaves_ReturnsEmptyList()
    {
        // Act
        var slots = _service.GetAllSaveSlots();

        // Assert
        slots.Should().BeEmpty();
    }

    [Fact]
    public void GetAllSaveSlots_WithMultipleSaves_ReturnsAllSlots()
    {
        // Arrange
        File.WriteAllText(_service.GetSlotFilePath(1), "{}");
        File.WriteAllText(_service.GetSlotFilePath(5), "{}");
        File.WriteAllText(_service.GetSlotFilePath(10), "{}");

        // Act
        var slots = _service.GetAllSaveSlots();

        // Assert
        slots.Should().HaveCount(3);
        slots.Select(s => s.SlotNumber).Should().ContainInOrder(1, 5, 10);
    }

    [Fact]
    public void GetAllSaveSlots_MarksSlotsAsManualOrAuto()
    {
        // Arrange
        File.WriteAllText(_service.GetSlotFilePath(1), "{}");  // Manual
        File.WriteAllText(_service.GetSlotFilePath(100), "{}"); // Auto

        // Act
        var slots = _service.GetAllSaveSlots();

        // Assert
        var manual = slots.Single(s => s.SlotNumber == 1);
        var auto = slots.Single(s => s.SlotNumber == 100);

        manual.IsAutoSave.Should().BeFalse();
        auto.IsAutoSave.Should().BeTrue();
    }

    [Fact]
    public void GetMostRecentAutoSaveSlot_WithNoAutoSaves_ReturnsNegativeOne()
    {
        // Act
        int slot = _service.GetMostRecentAutoSaveSlot();

        // Assert
        slot.Should().Be(-1);
    }

    [Fact]
    public void GetMostRecentAutoSaveSlot_ReturnsMostRecentSlot()
    {
        // Arrange
        File.WriteAllText(_service.GetSlotFilePath(100), "{}");
        File.WriteAllText(_service.GetSlotFilePath(101), "{}");
        System.Threading.Thread.Sleep(100); // Ensure 101 is newer
        File.WriteAllText(_service.GetSlotFilePath(102), "{}");

        // Act
        int slot = _service.GetMostRecentAutoSaveSlot();

        // Assert
        slot.Should().Be(102);
    }

    [Fact]
    public void GetNextAutoSaveSlot_WithNoSaves_ReturnsFirstAutoSlot()
    {
        // Act
        int slot = _service.GetNextAutoSaveSlot();

        // Assert
        slot.Should().Be(100);
    }

    [Fact]
    public void GetNextAutoSaveSlot_WithExistingSaves_RotatesSlots()
    {
        // Arrange
        File.WriteAllText(_service.GetSlotFilePath(100), "{}");

        // Act
        int nextSlot = _service.GetNextAutoSaveSlot();

        // Assert
        nextSlot.Should().Be(101);
    }

    [Fact]
    public void GetNextAutoSaveSlot_WrapsAround()
    {
        // Arrange
        File.WriteAllText(_service.GetSlotFilePath(109), "{}");

        // Act
        int nextSlot = _service.GetNextAutoSaveSlot();

        // Assert
        nextSlot.Should().Be(100); // Wraps back to first slot after 109
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, recursive: true);
    }
}
