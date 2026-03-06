namespace Hattrick.Core.Services;

/// <summary>
/// Provides random number generation for testability.
/// All randomness in gameplay must flow through this interface.
/// </summary>
public interface IRandomProvider
{
    /// <summary>
    /// Returns a random integer between <paramref name="minInclusive"/> (inclusive) and <paramref name="maxExclusive"/> (exclusive).
    /// </summary>
    int Next(int minInclusive, int maxExclusive);

    /// <summary>
    /// Returns a random integer between 0 (inclusive) and <paramref name="maxExclusive"/> (exclusive).
    /// </summary>
    int Next(int maxExclusive);

    /// <summary>
    /// Returns a random decimal between 0.0 (inclusive) and 1.0 (exclusive).
    /// </summary>
    double NextDouble();

    /// <summary>
    /// Returns a random integer between 0 (inclusive) and <see cref="int.MaxValue"/> (exclusive).
    /// </summary>
    int Next();

    /// <summary>
    /// Fills the specified array with random bytes.
    /// </summary>
    void NextBytes(byte[] buffer);
}
