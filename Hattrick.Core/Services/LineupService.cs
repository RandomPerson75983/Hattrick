using Hattrick.Core.Models;

namespace Hattrick.Core.Services;

/// <summary>
/// Service for managing team lineups including validation.
/// Phase 2 Sprint 2, Quartet 3.
/// </summary>
public class LineupService : ILineupService
{
    private const int RequiredStarterCount = 11;
    private const int MaxSubstituteCount = 3;
    private const int RequiredKeeperCount = 1;

    /// <inheritdoc />
    public LineupValidationResult ValidateLineup(TeamLineup lineup, IReadOnlyList<Player> squad)
    {
        ArgumentNullException.ThrowIfNull(lineup);
        ArgumentNullException.ThrowIfNull(squad);

        var errors = new List<string>();
        var squadIds = new HashSet<Guid>(squad.Select(p => p.Id));
        var lineupPlayerIds = new HashSet<Guid>(lineup.Slots.Select(s => s.PlayerId));

        // Rule 1: Exactly 11 starters
        var starterCount = lineup.Slots.Count(s => s.IsStarter);
        if (starterCount != RequiredStarterCount)
        {
            errors.Add($"Lineup must have exactly {RequiredStarterCount} starters, but has {starterCount}.");
        }

        // Rule 2: Exactly 1 keeper among starters
        var starterKeeperCount = lineup.Slots.Count(s => s.IsStarter && s.Position == Position.Keeper);
        if (starterKeeperCount != RequiredKeeperCount)
        {
            errors.Add($"Lineup must have exactly 1 keeper among starters, but has {starterKeeperCount}.");
        }

        // Rule 3: Max 3 substitutes
        var substituteCount = lineup.Slots.Count(s => !s.IsStarter);
        if (substituteCount > MaxSubstituteCount)
        {
            errors.Add($"Lineup can have max {MaxSubstituteCount} substitutes, but has {substituteCount}.");
        }

        // Rule 4: No duplicate player IDs
        var allPlayerIds = lineup.Slots.Select(s => s.PlayerId).ToList();
        var duplicateIds = allPlayerIds
            .GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicateIds.Count > 0)
        {
            errors.Add("Lineup contains duplicate players.");
        }

        // Rule 5: All player IDs must exist in squad
        foreach (var slot in lineup.Slots)
        {
            if (!squadIds.Contains(slot.PlayerId))
            {
                errors.Add($"Player {slot.PlayerId} is not found in squad.");
                break; // Only add one error for non-existent players
            }
        }

        // Rule 6: SetPiecesTakerId must be in lineup if specified
        if (lineup.SetPiecesTakerId.HasValue && !lineupPlayerIds.Contains(lineup.SetPiecesTakerId.Value))
        {
            errors.Add("Set pieces taker must be in the lineup.");
        }

        // Rule 7: CaptainId must be in lineup if specified
        if (lineup.CaptainId.HasValue && !lineupPlayerIds.Contains(lineup.CaptainId.Value))
        {
            errors.Add("Captain must be in the lineup.");
        }

        // Rules 8 & 9: Check injury and red card status for players in lineup
        foreach (var slot in lineup.Slots)
        {
            var player = squad.FirstOrDefault(p => p.Id == slot.PlayerId);
            if (player == null)
            {
                continue; // Already handled in Rule 5
            }

            // Rule 8: No injured players
            if (player.InjuryWeeks > 0)
            {
                errors.Add($"Player {player.Name} is injured and cannot be in the lineup.");
            }

            // Rule 9: No red-carded players
            if (player.RedCard)
            {
                errors.Add($"Player {player.Name} has a red card suspension and cannot be in the lineup.");
            }
        }

        return new LineupValidationResult(errors.Count == 0, errors);
    }
}
