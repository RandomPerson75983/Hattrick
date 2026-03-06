namespace Hattrick.Core.Services;

/// <summary>
/// Provides current date/time for testability.
/// Services must never use <see cref="DateTime.Now"/> directly — always inject this interface.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current date and time (UTC).
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Gets the current date and time (local).
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    /// Gets today's date.
    /// </summary>
    DateTime Today { get; }
}
