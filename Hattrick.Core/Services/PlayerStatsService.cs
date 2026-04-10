using Hattrick.Core.Models;

namespace Hattrick.Core.Services;

/// <summary>
/// Computes team-level aggregate statistics from a list of players.
/// Pure functions — no state, no dependencies. Safe to register as singleton.
///
/// Formula: EstimatedValue per player = TSI * 25
/// </summary>
public sealed class PlayerStatsService : IPlayerStatsService
{
    private const int EstimatedValueMultiplier = 25;

    /// <inheritdoc/>
    public TeamTotals GetTeamTotals(IReadOnlyList<Player> players)
    {
        return new TeamTotals
        {
            TotalTSI             = players.Sum(p => p.TSI),
            TotalWage            = players.Sum(p => p.Wage),
            TotalEstimatedValue  = players.Sum(p => p.TSI * EstimatedValueMultiplier),
            NationalityCount     = players.Select(p => p.NativeCountryId).Distinct().Count(),
            InjuredCount         = players.Count(p => p.InjuryWeeks > 0),
            RedCardCount         = players.Count(p => p.RedCard),
            YellowCardCount      = players.Sum(p => p.YellowCards),
        };
    }

    /// <inheritdoc/>
    public TeamAverages GetTeamAverages(IReadOnlyList<Player> players)
    {
        if (players.Count == 0)
        {
            return new TeamAverages();
        }

        return new TeamAverages
        {
            AvgTSI            = players.Average(p => (double)p.TSI),
            AvgWage           = players.Average(p => (double)p.Wage),
            AvgEstimatedValue = players.Average(p => (double)(p.TSI * EstimatedValueMultiplier)),
            AvgAge            = players.Average(p => (double)p.Age),
            AvgForm           = players.Average(p => (double)p.Form),
            AvgStamina        = players.Average(p => (double)p.Stamina),
            AvgExperience     = players.Average(p => (double)p.Experience),
        };
    }
}
