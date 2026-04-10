namespace Hattrick.Core.Models;

/// <summary>
/// Aggregate totals computed from a squad's player list.
/// Returned by <see cref="Services.IPlayerStatsService.GetTeamTotals"/>.
/// </summary>
public sealed class TeamTotals
{
    /// <summary>Sum of all player TSI values.</summary>
    public int TotalTSI { get; init; }

    /// <summary>Sum of all player weekly wages.</summary>
    public decimal TotalWage { get; init; }

    /// <summary>
    /// Sum of estimated market values. Formula: each player's TSI * 25.
    /// </summary>
    public decimal TotalEstimatedValue { get; init; }

    /// <summary>Count of distinct native country IDs across the squad.</summary>
    public int NationalityCount { get; init; }

    /// <summary>Count of players with InjuryWeeks > 0.</summary>
    public int InjuredCount { get; init; }

    /// <summary>Count of players with RedCard == true.</summary>
    public int RedCardCount { get; init; }

    /// <summary>Sum of all yellow cards across the squad.</summary>
    public int YellowCardCount { get; init; }
}
