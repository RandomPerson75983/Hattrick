using Hattrick.Core.Services;

namespace Hattrick.Tests.Services;

public class GameStateServiceTests
{
    [Fact]
    public void Constructor_InitializesDefaultState()
    {
        // Act
        var service = new GameStateService();

        // Assert
        service.CurrentSeasonNumber.Should().Be(1);
        service.CurrentWeekNumber.Should().Be(1);
        service.IsGameLoaded.Should().BeFalse();
        service.CurrentSaveSlot.Should().BeNull();
    }

    [Fact]
    public void CurrentSeasonNumber_CanBeSet()
    {
        // Arrange
        var service = new GameStateService();

        // Act
        service.CurrentSeasonNumber = 5;

        // Assert
        service.CurrentSeasonNumber.Should().Be(5);
    }

    [Fact]
    public void CurrentWeekNumber_CanBeSet()
    {
        // Arrange
        var service = new GameStateService();

        // Act
        service.CurrentWeekNumber = 12;

        // Assert
        service.CurrentWeekNumber.Should().Be(12);
    }

    [Fact]
    public void IsGameLoaded_CanBeSet()
    {
        // Arrange
        var service = new GameStateService();

        // Act
        service.IsGameLoaded = true;

        // Assert
        service.IsGameLoaded.Should().BeTrue();
    }

    [Fact]
    public void CurrentSaveSlot_CanBeSet()
    {
        // Arrange
        var service = new GameStateService();

        // Act
        service.CurrentSaveSlot = 5;

        // Assert
        service.CurrentSaveSlot.Should().Be(5);
    }

    [Fact]
    public void Reset_ResetsAllPropertiesToDefault()
    {
        // Arrange
        var service = new GameStateService
        {
            CurrentSeasonNumber = 10,
            CurrentWeekNumber = 15,
            IsGameLoaded = true,
            CurrentSaveSlot = 25
        };

        // Act
        service.Reset();

        // Assert
        service.CurrentSeasonNumber.Should().Be(1);
        service.CurrentWeekNumber.Should().Be(1);
        service.IsGameLoaded.Should().BeFalse();
        service.CurrentSaveSlot.Should().BeNull();
    }

    [Fact]
    public void Reset_CanBeCalledMultipleTimes()
    {
        // Arrange
        var service = new GameStateService();

        // Act
        service.CurrentSeasonNumber = 5;
        service.Reset();
        service.CurrentSeasonNumber = 3;
        service.Reset();

        // Assert
        service.CurrentSeasonNumber.Should().Be(1);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(20)]
    public void CurrentSeasonNumber_CanHoldAnyPositiveValue(int seasonNumber)
    {
        // Arrange
        var service = new GameStateService();

        // Act
        service.CurrentSeasonNumber = seasonNumber;

        // Assert
        service.CurrentSeasonNumber.Should().Be(seasonNumber);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(8)]
    [InlineData(16)]
    public void CurrentWeekNumber_CanHoldAnyValidWeekValue(int weekNumber)
    {
        // Arrange
        var service = new GameStateService();

        // Act
        service.CurrentWeekNumber = weekNumber;

        // Assert
        service.CurrentWeekNumber.Should().Be(weekNumber);
    }
}
