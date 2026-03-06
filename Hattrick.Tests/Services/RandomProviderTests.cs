using Hattrick.Core.Services;

namespace Hattrick.Tests.Services;

public class RandomProviderTests
{
    [Fact]
    public void Next_WithMinAndMax_ReturnsValueInRange()
    {
        // Arrange
        var random = new RandomProvider();
        const int min = 1;
        const int max = 10;

        // Act
        var result = random.Next(min, max);

        // Assert
        result.Should().BeGreaterThanOrEqualTo(min);
        result.Should().BeLessThan(max);
    }

    [Fact]
    public void Next_WithMaxOnly_ReturnsValueInRange()
    {
        // Arrange
        var random = new RandomProvider();
        const int max = 100;

        // Act
        var result = random.Next(max);

        // Assert
        result.Should().BeGreaterThanOrEqualTo(0);
        result.Should().BeLessThan(max);
    }

    [Fact]
    public void NextDouble_ReturnsValueBetweenZeroAndOne()
    {
        // Arrange
        var random = new RandomProvider();

        // Act
        var result = random.NextDouble();

        // Assert
        result.Should().BeGreaterThanOrEqualTo(0.0);
        result.Should().BeLessThan(1.0);
    }

    [Fact]
    public void Next_WithoutParameters_ReturnsNonNegativeInt()
    {
        // Arrange
        var random = new RandomProvider();

        // Act
        var result = random.Next();

        // Assert
        result.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void NextBytes_FillsBuffer()
    {
        // Arrange
        var random = new RandomProvider();
        byte[] buffer = new byte[10];

        // Act
        random.NextBytes(buffer);

        // Assert
        buffer.Should().NotBeEmpty();
        buffer.Any(b => b != 0).Should().BeTrue();
    }

    [Fact]
    public void Next_MultipleCalls_ProducesRandomValues()
    {
        // Arrange
        var random = new RandomProvider();
        var values = new List<int>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            values.Add(random.Next(1, 1000000));
        }

        // Assert
        var uniqueCount = values.Distinct().Count();
        uniqueCount.Should().BeGreaterThan(1); // At least some variation
    }
}
