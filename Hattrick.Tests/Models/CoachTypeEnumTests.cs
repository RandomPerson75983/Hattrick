using Hattrick.Core.Models;

namespace Hattrick.Tests.Models;

/// <summary>
/// Tests for the CoachType enum.
/// Verifies ordinal values are stable for serialization and that the zero-value
/// is Offensive (required for correct default on the Team.CoachType property).
/// </summary>
public class CoachTypeEnumTests
{
    #region CoachType Ordinal Values

    [Theory]
    [InlineData(CoachType.Offensive, 0)]
    [InlineData(CoachType.Defensive, 1)]
    [InlineData(CoachType.Balanced, 2)]
    public void CoachType_MemberHasExpectedOrdinal(CoachType member, int expectedValue)
    {
        ((int)member).Should().Be(expectedValue,
            $"CoachType.{member} must have ordinal {expectedValue} for serialization stability and correct default behaviour");
    }

    #endregion
}
