using Hattrick.Core.Services;

namespace Hattrick.Tests.Services;

public class DateTimeProviderTests
{
    [Fact]
    public void UtcNow_ReturnsCurrentUtcTime()
    {
        // Arrange
        var provider = new DateTimeProvider();
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = provider.UtcNow;

        var afterCall = DateTime.UtcNow;

        // Assert
        result.Should().BeOnOrAfter(beforeCall.AddSeconds(-1));
        result.Should().BeOnOrBefore(afterCall.AddSeconds(1));
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Now_ReturnsCurrentLocalTime()
    {
        // Arrange
        var provider = new DateTimeProvider();
        var beforeCall = DateTime.Now;

        // Act
        var result = provider.Now;

        var afterCall = DateTime.Now;

        // Assert
        result.Should().BeOnOrAfter(beforeCall.AddSeconds(-1));
        result.Should().BeOnOrBefore(afterCall.AddSeconds(1));
        result.Kind.Should().Be(DateTimeKind.Local);
    }

    [Fact]
    public void Today_ReturnsCurrentDate()
    {
        // Arrange
        var provider = new DateTimeProvider();

        // Act
        var result = provider.Today;

        // Assert
        result.Should().Be(DateTime.Today);
        result.Hour.Should().Be(0);
        result.Minute.Should().Be(0);
        result.Second.Should().Be(0);
    }

    [Fact]
    public void Now_UtcNow_AreBothValid()
    {
        // Arrange
        var provider = new DateTimeProvider();

        // Act
        var now = provider.Now;
        var utcNow = provider.UtcNow;
        var today = provider.Today;

        // Assert
        now.Should().NotBe(DateTime.MinValue);
        utcNow.Should().NotBe(DateTime.MinValue);
        today.Should().NotBe(DateTime.MinValue);
    }
}
