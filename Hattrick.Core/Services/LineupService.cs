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
    private const int YellowCardSuspensionThreshold = 3;

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

            // Rule 10: No yellow-card-suspended players
            if (player.YellowCards >= YellowCardSuspensionThreshold)
            {
                errors.Add($"Player {player.Name} has a yellow card suspension and cannot be in the lineup.");
            }
        }

        return new LineupValidationResult(errors.Count == 0, errors);
    }

    /// <inheritdoc />
    public TeamLineup SuggestLineup(Guid teamId, IReadOnlyList<Player> squad)
    {
        ArgumentNullException.ThrowIfNull(squad);

        // Step 1: Filter available players (not injured, no red card, no yellow card suspension)
        var availablePlayers = squad
            .Where(p => p.InjuryWeeks == 0 && !p.RedCard && p.YellowCards < YellowCardSuspensionThreshold)
            .ToList();

        // Step 2: Validate minimum player count
        if (availablePlayers.Count < RequiredStarterCount)
        {
            throw new InvalidOperationException(
                $"Cannot suggest lineup: need at least {RequiredStarterCount} available players, but only {availablePlayers.Count} are available.");
        }

        var selectedPlayers = new HashSet<Guid>();
        var slots = new List<MatchLineupSlot>();

        // Step 3: Pick 1 best keeper
        var keeper = PickBestPlayerForPosition(
            availablePlayers,
            selectedPlayers,
            Position.Keeper,
            SkillType.Keeper,
            count: 1);
        slots.AddRange(keeper);

        // Step 4: Pick defenders for 4-4-2
        // 2 Central Defenders by Defending skill
        var centralDefenders = PickBestPlayerForPosition(
            availablePlayers,
            selectedPlayers,
            Position.CentralDefender,
            SkillType.Defending,
            count: 2);
        slots.AddRange(centralDefenders);

        // 2 Wing Backs by Defending skill
        var wingBacks = PickBestPlayerForPosition(
            availablePlayers,
            selectedPlayers,
            Position.WingBack,
            SkillType.Defending,
            count: 2);
        slots.AddRange(wingBacks);

        // Step 5: Pick midfielders for 4-4-2
        // 2 Inner Midfielders by Playmaking skill
        var innerMids = PickBestPlayerForPosition(
            availablePlayers,
            selectedPlayers,
            Position.InnerMidfielder,
            SkillType.Playmaking,
            count: 2);
        slots.AddRange(innerMids);

        // 2 Wingers by Winger skill
        var wingers = PickBestPlayerForPosition(
            availablePlayers,
            selectedPlayers,
            Position.Winger,
            SkillType.Winger,
            count: 2);
        slots.AddRange(wingers);

        // Step 6: Pick 2 Forwards by Scoring skill
        var forwards = PickBestPlayerForPosition(
            availablePlayers,
            selectedPlayers,
            Position.Forward,
            SkillType.Scoring,
            count: 2);
        slots.AddRange(forwards);

        // Step 7: Add up to 3 substitutes from remaining players
        var remainingPlayers = availablePlayers
            .Where(p => !selectedPlayers.Contains(p.Id))
            .Take(MaxSubstituteCount)
            .ToList();

        foreach (var sub in remainingPlayers)
        {
            slots.Add(new MatchLineupSlot(
                sub.Id,
                sub.BestPosition,
                IndividualOrder.Normal,
                isStarter: false));
            selectedPlayers.Add(sub.Id);
        }

        // Step 8: Set Captain = starter with highest Leadership
        var starterIds = slots.Where(s => s.IsStarter).Select(s => s.PlayerId).ToHashSet();
        var captain = availablePlayers
            .Where(p => starterIds.Contains(p.Id))
            .OrderByDescending(p => p.Leadership)
            .FirstOrDefault();

        // Step 9: Build and return TeamLineup
        return new TeamLineup
        {
            TeamId = teamId,
            Formation = Formation.Formation442,
            Tactic = Tactic.Normal,
            Attitude = TeamAttitude.Normal,
            Slots = slots,
            CaptainId = captain?.Id,
            SetPiecesTakerId = null
        };
    }

    /// <summary>
    /// Picks the best available players for a position based on skill, with BestPosition priority.
    /// </summary>
    private static List<MatchLineupSlot> PickBestPlayerForPosition(
        List<Player> availablePlayers,
        HashSet<Guid> selectedPlayers,
        Position targetPosition,
        SkillType primarySkill,
        int count)
    {
        var slots = new List<MatchLineupSlot>();

        // Get unselected players
        var candidates = availablePlayers
            .Where(p => !selectedPlayers.Contains(p.Id))
            .ToList();

        // First, try to pick players who have this as their BestPosition, sorted by skill
        var naturalPlayers = candidates
            .Where(p => p.BestPosition == targetPosition)
            .OrderByDescending(p => GetSkillValue(p, primarySkill))
            .Take(count)
            .ToList();

        foreach (var player in naturalPlayers)
        {
            slots.Add(new MatchLineupSlot(
                player.Id,
                targetPosition,
                IndividualOrder.Normal,
                isStarter: true));
            selectedPlayers.Add(player.Id);
        }

        // If we still need more players, fill from remaining candidates by skill
        var remaining = count - slots.Count;
        if (remaining > 0)
        {
            var fallbackPlayers = candidates
                .Where(p => !selectedPlayers.Contains(p.Id))
                .OrderByDescending(p => GetSkillValue(p, primarySkill))
                .Take(remaining)
                .ToList();

            foreach (var player in fallbackPlayers)
            {
                slots.Add(new MatchLineupSlot(
                    player.Id,
                    targetPosition,
                    IndividualOrder.Normal,
                    isStarter: true));
                selectedPlayers.Add(player.Id);
            }
        }

        return slots;
    }

    /// <summary>
    /// Gets the skill value for a player, returning 0 if the skill is not present.
    /// </summary>
    private static double GetSkillValue(Player player, SkillType skillType)
    {
        return player.Skills.TryGetValue(skillType, out var value) ? value : 0.0;
    }
}
