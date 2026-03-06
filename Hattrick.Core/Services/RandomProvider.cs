namespace Hattrick.Core.Services;

/// <summary>
/// Default implementation of IRandomProvider using <see cref="Random.Shared"/>.
/// </summary>
public class RandomProvider : IRandomProvider
{
    /// <inheritdoc />
    public int Next(int minInclusive, int maxExclusive)
    {
        return Random.Shared.Next(minInclusive, maxExclusive);
    }

    /// <inheritdoc />
    public int Next(int maxExclusive)
    {
        return Random.Shared.Next(maxExclusive);
    }

    /// <inheritdoc />
    public double NextDouble()
    {
        return Random.Shared.NextDouble();
    }

    /// <inheritdoc />
    public int Next()
    {
        return Random.Shared.Next();
    }

    /// <inheritdoc />
    public void NextBytes(byte[] buffer)
    {
        Random.Shared.NextBytes(buffer);
    }
}
