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
    private const decimal EstimatedValueMultiplier = 25m;

    private static decimal ComputeEstimatedValue(Player player) =>
        (decimal)player.TSI * EstimatedValueMultiplier;

    /// <inheritdoc/>
    public TeamTotals GetTeamTotals(IReadOnlyList<Player> players)
    {
        ArgumentNullException.ThrowIfNull(players);
        int totalTSI = 0, injured = 0, redCards = 0, yellowCards = 0;
        decimal totalWage = 0m, totalEstimatedValue = 0m;
        var nationalities = new HashSet<int>();

        foreach (var p in players)
        {
            totalTSI += p.TSI;
            totalWage += p.Wage;
            totalEstimatedValue += ComputeEstimatedValue(p);
            nationalities.Add(p.NativeCountryId);
            if (p.InjuryWeeks > 0) injured++;
            if (p.RedCard) redCards++;
            yellowCards += p.YellowCards;
        }

        return new TeamTotals
        {
            TotalTSI             = totalTSI,
            TotalWage            = totalWage,
            TotalEstimatedValue  = totalEstimatedValue,
            NationalityCount     = nationalities.Count,
            InjuredCount         = injured,
            RedCardCount         = redCards,
            YellowCardCount      = yellowCards,
        };
    }

    /// <inheritdoc/>
    public TeamAverages GetTeamAverages(IReadOnlyList<Player> players)
    {
        ArgumentNullException.ThrowIfNull(players);
        if (players.Count == 0) return new TeamAverages();

        double totalTSI = 0, totalWage = 0, totalEstValue = 0, totalAge = 0, totalForm = 0, totalStamina = 0, totalExp = 0;

        foreach (var p in players)
        {
            totalTSI      += p.TSI;
            totalWage     += (double)p.Wage;
            totalEstValue += (double)ComputeEstimatedValue(p);
            totalAge      += p.Age;
            totalForm     += p.Form;
            totalStamina  += p.Stamina;
            totalExp      += p.Experience;
        }

        double count = players.Count;
        return new TeamAverages
        {
            AvgTSI            = totalTSI      / count,
            AvgWage           = totalWage     / count,
            AvgEstimatedValue = (decimal)(totalEstValue / count),
            AvgAge            = totalAge      / count,
            AvgForm           = totalForm     / count,
            AvgStamina        = totalStamina  / count,
            AvgExperience     = totalExp      / count,
        };
    }
}
