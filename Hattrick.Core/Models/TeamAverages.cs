namespace Hattrick.Core.Models;

/// <summary>
/// Arithmetic mean averages computed from a squad's player list.
/// Returns 0.0 for every field when the squad is empty (no divide-by-zero).
/// Returned by <see cref="Services.IPlayerStatsService.GetTeamAverages"/>.
/// </summary>
public sealed class TeamAverages
{
    /// <summary>Arithmetic mean of player TSI values.</summary>
    public double AvgTSI { get; init; }

    /// <summary>Arithmetic mean of player weekly wages.</summary>
    public double AvgWage { get; init; }

    /// <summary>
    /// Arithmetic mean of estimated market values. Formula: each player's TSI * 25.
    /// </summary>
    public decimal AvgEstimatedValue { get; init; }

    /// <summary>Arithmetic mean of player ages.</summary>
    public double AvgAge { get; init; }

    /// <summary>Arithmetic mean of player form levels.</summary>
    public double AvgForm { get; init; }

    /// <summary>Arithmetic mean of player stamina levels.</summary>
    public double AvgStamina { get; init; }

    /// <summary>Arithmetic mean of player experience levels.</summary>
    public double AvgExperience { get; init; }
}
