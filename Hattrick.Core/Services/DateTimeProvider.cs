namespace Hattrick.Core.Services;

/// <summary>
/// Default implementation of IDateTimeProvider.
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;

    /// <inheritdoc />
    public DateTime Now => DateTime.Now;

    /// <inheritdoc />
    public DateTime Today => DateTime.Today;
}
