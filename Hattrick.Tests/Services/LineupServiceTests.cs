using Hattrick.Core.Models;
using Hattrick.Core.Services;

namespace Hattrick.Tests.Services;

/// <summary>
/// Unit tests for ILineupService.ValidateLineup method.
///
/// Phase 2 Sprint 2, Quartet 3 (LineupService - Validation).
///
/// Validation rules tested:
/// 1. Exactly 11 starters (IsStarter=true)
/// 2. Exactly 1 keeper among starters (Position.Keeper)
/// 3. Max 3 substitutes (IsStarter=false)
/// 4. No duplicate player IDs in slots
/// 5. All player IDs must exist in squad
/// 6. SetPiecesTakerId must be in lineup if specified
/// 7. CaptainId must be in lineup if specified
/// 8. No injured players (InjuryWeeks > 0)
/// 9. No red-carded players (RedCard=true)
///
/// ValidateLineup returns LineupValidationResult with:
/// - IsValid (bool): true if all rules pass
/// - Errors (IReadOnlyList&lt;string&gt;): list of validation error messages
/// </summary>
public class LineupServiceTests
{
    private readonly ILineupService _sut;

    // Constants matching TeamLineup bounds
    private const int RequiredStarterCount = 11;
    private const int MaxSubstituteCount = 3;
    private const int RequiredKeeperCount = 1;

    public LineupServiceTests()
    {
        _sut = new LineupService();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ValidateLineup — Valid Lineup (Happy Path)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_WithValidLineup_ReturnsIsValidTrue()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateLineup_WithValidLineup_ReturnsEmptyErrors()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateLineup_WithValidLineupIncludingSubstitutes_ReturnsIsValidTrue()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquadAndSubstitutes(substituteCount: 3);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Rule 1: Exactly 11 starters
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_With10Starters_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateLineupWithStarterCount(starterCount: 10);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLineup_With10Starters_ReturnsStarterCountError()
    {
        // Arrange
        var (lineup, squad) = CreateLineupWithStarterCount(starterCount: 10);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Contains("11") && e.Contains("starter", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateLineup_With12Starters_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateLineupWithStarterCount(starterCount: 12);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLineup_With0Starters_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateLineupWithStarterCount(starterCount: 0);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Rule 2: Exactly 1 keeper among starters
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_With0Keepers_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        // Replace keeper slot with a forward
        var keeperSlot = lineup.Slots.First(s => s.Position == Position.Keeper && s.IsStarter);
        var newSlot = new MatchLineupSlot(keeperSlot.PlayerId, Position.Forward, isStarter: true);
        lineup.Slots.Remove(keeperSlot);
        lineup.Slots.Add(newSlot);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLineup_With0Keepers_ReturnsKeeperError()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        // Replace keeper slot with a forward
        var keeperSlot = lineup.Slots.First(s => s.Position == Position.Keeper && s.IsStarter);
        var newSlot = new MatchLineupSlot(keeperSlot.PlayerId, Position.Forward, isStarter: true);
        lineup.Slots.Remove(keeperSlot);
        lineup.Slots.Add(newSlot);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Contains("keeper", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateLineup_With2Keepers_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateLineupWithKeeperCount(keeperCount: 2);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLineup_With2Keepers_ReturnsKeeperCountError()
    {
        // Arrange
        var (lineup, squad) = CreateLineupWithKeeperCount(keeperCount: 2);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.Errors.Should().ContainSingle(e =>
            e.Contains("keeper", StringComparison.OrdinalIgnoreCase) &&
            (e.Contains("1") || e.Contains("exactly one", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void ValidateLineup_WithKeeperAsSubstitute_ReturnsIsValidFalse()
    {
        // Arrange: Lineup has no keeper among starters, but keeper is on bench
        var (lineup, squad) = CreateValidLineupWithSquad();
        // Move keeper to substitute
        var keeperSlot = lineup.Slots.First(s => s.Position == Position.Keeper && s.IsStarter);
        var keeperAsSub = new MatchLineupSlot(keeperSlot.PlayerId, Position.Keeper, isStarter: false);
        lineup.Slots.Remove(keeperSlot);
        lineup.Slots.Add(keeperAsSub);
        // Add extra forward as starter to maintain 11 starters
        var extraPlayer = CreatePlayer(Position.Forward);
        squad = squad.Concat([extraPlayer]).ToList();
        lineup.Slots.Add(new MatchLineupSlot(extraPlayer.Id, Position.Forward, isStarter: true));

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse("keeper must be among starters, not substitutes");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Rule 3: Max 3 substitutes
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_With3Substitutes_ReturnsIsValidTrue()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquadAndSubstitutes(substituteCount: 3);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateLineup_With4Substitutes_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquadAndSubstitutes(substituteCount: 4);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLineup_With4Substitutes_ReturnsSubstituteCountError()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquadAndSubstitutes(substituteCount: 4);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.Errors.Should().ContainSingle(e =>
            e.Contains("substitute", StringComparison.OrdinalIgnoreCase) &&
            (e.Contains("3") || e.Contains("max", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void ValidateLineup_With0Substitutes_ReturnsIsValidTrue()
    {
        // Arrange: Valid lineup with no substitutes (all starters)
        var (lineup, squad) = CreateValidLineupWithSquadAndSubstitutes(substituteCount: 0);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue("substitutes are optional");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Rule 4: No duplicate player IDs in slots
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_WithDuplicatePlayerInStarters_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        // Duplicate the first player in another starter slot
        var duplicatePlayerId = lineup.Slots.First(s => s.IsStarter).PlayerId;
        var existingSlot = lineup.Slots.Last(s => s.IsStarter && s.PlayerId != duplicatePlayerId);
        var duplicateSlot = new MatchLineupSlot(duplicatePlayerId, existingSlot.Position, isStarter: true);
        lineup.Slots.Remove(existingSlot);
        lineup.Slots.Add(duplicateSlot);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLineup_WithDuplicatePlayerInStarters_ReturnsDuplicateError()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var duplicatePlayerId = lineup.Slots.First(s => s.IsStarter).PlayerId;
        var existingSlot = lineup.Slots.Last(s => s.IsStarter && s.PlayerId != duplicatePlayerId);
        var duplicateSlot = new MatchLineupSlot(duplicatePlayerId, existingSlot.Position, isStarter: true);
        lineup.Slots.Remove(existingSlot);
        lineup.Slots.Add(duplicateSlot);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Contains("duplicate", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateLineup_WithSamePlayerAsStarterAndSubstitute_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquadAndSubstitutes(substituteCount: 1);
        var starterPlayerId = lineup.Slots.First(s => s.IsStarter).PlayerId;
        // Change substitute to be same player as a starter
        var subSlot = lineup.Slots.First(s => !s.IsStarter);
        var duplicateSlot = new MatchLineupSlot(starterPlayerId, subSlot.Position, isStarter: false);
        lineup.Slots.Remove(subSlot);
        lineup.Slots.Add(duplicateSlot);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse("a player cannot be both starter and substitute");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Rule 5: All player IDs must exist in squad
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_WithPlayerNotInSquad_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        // Replace one player with a non-existent player
        var oldSlot = lineup.Slots.First(s => s.IsStarter && s.Position != Position.Keeper);
        var nonExistentPlayerId = Guid.NewGuid();
        var invalidSlot = new MatchLineupSlot(nonExistentPlayerId, oldSlot.Position, isStarter: true);
        lineup.Slots.Remove(oldSlot);
        lineup.Slots.Add(invalidSlot);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLineup_WithPlayerNotInSquad_ReturnsSquadError()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var oldSlot = lineup.Slots.First(s => s.IsStarter && s.Position != Position.Keeper);
        var nonExistentPlayerId = Guid.NewGuid();
        var invalidSlot = new MatchLineupSlot(nonExistentPlayerId, oldSlot.Position, isStarter: true);
        lineup.Slots.Remove(oldSlot);
        lineup.Slots.Add(invalidSlot);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.Errors.Should().ContainSingle(e =>
            e.Contains("squad", StringComparison.OrdinalIgnoreCase) ||
            e.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
            e.Contains("does not exist", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateLineup_WithEmptySquad_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, _) = CreateValidLineupWithSquad();
        var emptySquad = new List<Player>();

        // Act
        var result = _sut.ValidateLineup(lineup, emptySquad);

        // Assert
        result.IsValid.Should().BeFalse("lineup players must exist in squad");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Rule 6: SetPiecesTakerId must be in lineup if specified
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_WithSetPiecesTakerInLineup_ReturnsIsValidTrue()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var starterPlayerId = lineup.Slots.First(s => s.IsStarter).PlayerId;
        lineup.SetPiecesTakerId = starterPlayerId;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateLineup_WithSetPiecesTakerNotInLineup_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        // Add extra player to squad but not to lineup
        var extraPlayer = CreatePlayer(Position.InnerMidfielder);
        squad = squad.Concat([extraPlayer]).ToList();
        lineup.SetPiecesTakerId = extraPlayer.Id;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLineup_WithSetPiecesTakerNotInLineup_ReturnsSetPiecesError()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var extraPlayer = CreatePlayer(Position.InnerMidfielder);
        squad = squad.Concat([extraPlayer]).ToList();
        lineup.SetPiecesTakerId = extraPlayer.Id;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.Errors.Should().ContainSingle(e =>
            e.Contains("set piece", StringComparison.OrdinalIgnoreCase) ||
            e.Contains("SetPieces", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateLineup_WithNullSetPiecesTaker_ReturnsIsValidTrue()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        lineup.SetPiecesTakerId = null;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue("set pieces taker is optional");
    }

    [Fact]
    public void ValidateLineup_WithSetPiecesTakerAsSubstitute_ReturnsIsValidFalse()
    {
        // Arrange: Set pieces taker is on bench, not starting
        var (lineup, squad) = CreateValidLineupWithSquadAndSubstitutes(substituteCount: 1);
        var substitutePlayerId = lineup.Slots.First(s => !s.IsStarter).PlayerId;
        lineup.SetPiecesTakerId = substitutePlayerId;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert - Set pieces taker must be in lineup (starter or sub), so this should be valid
        // Actually re-reading requirement: "must be in lineup" - substitute IS in lineup
        result.IsValid.Should().BeTrue("substitute is in lineup");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Rule 7: CaptainId must be in lineup if specified
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_WithCaptainInLineup_ReturnsIsValidTrue()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var starterPlayerId = lineup.Slots.First(s => s.IsStarter).PlayerId;
        lineup.CaptainId = starterPlayerId;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateLineup_WithCaptainNotInLineup_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var extraPlayer = CreatePlayer(Position.CentralDefender);
        squad = squad.Concat([extraPlayer]).ToList();
        lineup.CaptainId = extraPlayer.Id;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLineup_WithCaptainNotInLineup_ReturnsCaptainError()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var extraPlayer = CreatePlayer(Position.CentralDefender);
        squad = squad.Concat([extraPlayer]).ToList();
        lineup.CaptainId = extraPlayer.Id;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Contains("captain", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateLineup_WithNullCaptain_ReturnsIsValidTrue()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        lineup.CaptainId = null;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue("captain is optional");
    }

    [Fact]
    public void ValidateLineup_WithCaptainAsSubstitute_ReturnsIsValidTrue()
    {
        // Arrange: Captain is on bench (substitute)
        var (lineup, squad) = CreateValidLineupWithSquadAndSubstitutes(substituteCount: 1);
        var substitutePlayerId = lineup.Slots.First(s => !s.IsStarter).PlayerId;
        lineup.CaptainId = substitutePlayerId;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue("substitute is in lineup");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Rule 8: No injured players (InjuryWeeks > 0)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_WithInjuredStarter_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var starterPlayerId = lineup.Slots.First(s => s.IsStarter && s.Position != Position.Keeper).PlayerId;
        var injuredPlayer = squad.First(p => p.Id == starterPlayerId);
        injuredPlayer.InjuryWeeks = 1;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLineup_WithInjuredStarter_ReturnsInjuryError()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var starterPlayerId = lineup.Slots.First(s => s.IsStarter && s.Position != Position.Keeper).PlayerId;
        var injuredPlayer = squad.First(p => p.Id == starterPlayerId);
        injuredPlayer.InjuryWeeks = 1;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Contains("injur", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateLineup_WithInjuredSubstitute_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquadAndSubstitutes(substituteCount: 1);
        var substitutePlayerId = lineup.Slots.First(s => !s.IsStarter).PlayerId;
        var injuredPlayer = squad.First(p => p.Id == substitutePlayerId);
        injuredPlayer.InjuryWeeks = 2;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse("injured players cannot be substitutes either");
    }

    [Fact]
    public void ValidateLineup_WithInjuryWeeksZero_ReturnsIsValidTrue()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        // Ensure all players have InjuryWeeks = 0 (healthy)
        foreach (var player in squad)
        {
            player.InjuryWeeks = 0;
        }

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateLineup_WithInjuredPlayerNotInLineup_ReturnsIsValidTrue()
    {
        // Arrange: Player in squad is injured but not in lineup
        var (lineup, squad) = CreateValidLineupWithSquad();
        var extraPlayer = CreatePlayer(Position.Forward);
        extraPlayer.InjuryWeeks = 5;
        squad = squad.Concat([extraPlayer]).ToList();

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue("only lineup players are validated for injuries");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Rule 9: No red-carded players (RedCard=true)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_WithRedCardedStarter_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var starterPlayerId = lineup.Slots.First(s => s.IsStarter && s.Position != Position.Keeper).PlayerId;
        var redCardedPlayer = squad.First(p => p.Id == starterPlayerId);
        redCardedPlayer.RedCard = true;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLineup_WithRedCardedStarter_ReturnsRedCardError()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var starterPlayerId = lineup.Slots.First(s => s.IsStarter && s.Position != Position.Keeper).PlayerId;
        var redCardedPlayer = squad.First(p => p.Id == starterPlayerId);
        redCardedPlayer.RedCard = true;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.Errors.Should().ContainSingle(e =>
            e.Contains("red card", StringComparison.OrdinalIgnoreCase) ||
            e.Contains("suspend", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateLineup_WithRedCardedSubstitute_ReturnsIsValidFalse()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquadAndSubstitutes(substituteCount: 1);
        var substitutePlayerId = lineup.Slots.First(s => !s.IsStarter).PlayerId;
        var redCardedPlayer = squad.First(p => p.Id == substitutePlayerId);
        redCardedPlayer.RedCard = true;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse("red-carded players cannot be substitutes either");
    }

    [Fact]
    public void ValidateLineup_WithRedCardFalse_ReturnsIsValidTrue()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        foreach (var player in squad)
        {
            player.RedCard = false;
        }

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateLineup_WithRedCardedPlayerNotInLineup_ReturnsIsValidTrue()
    {
        // Arrange: Player in squad has red card but not in lineup
        var (lineup, squad) = CreateValidLineupWithSquad();
        var extraPlayer = CreatePlayer(Position.Forward);
        extraPlayer.RedCard = true;
        squad = squad.Concat([extraPlayer]).ToList();

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue("only lineup players are validated for red cards");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Multiple Errors / Combined Scenarios
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_WithMultipleViolations_ReturnsAllErrors()
    {
        // Arrange: 10 starters + injured player
        var (lineup, squad) = CreateLineupWithStarterCount(starterCount: 10);
        // Also make one player injured
        if (lineup.Slots.Any())
        {
            var firstPlayerId = lineup.Slots.First().PlayerId;
            var player = squad.FirstOrDefault(p => p.Id == firstPlayerId);
            if (player != null)
            {
                player.InjuryWeeks = 1;
            }
        }

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterOrEqualTo(2);
    }

    [Fact]
    public void ValidateLineup_WithInjuredAndRedCarded_ReturnsBothErrors()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var starters = lineup.Slots.Where(s => s.IsStarter && s.Position != Position.Keeper).ToList();

        // Set one player injured
        var injuredPlayerId = starters[0].PlayerId;
        squad.First(p => p.Id == injuredPlayerId).InjuryWeeks = 1;

        // Set another player red-carded
        var redCardedPlayerId = starters[1].PlayerId;
        squad.First(p => p.Id == redCardedPlayerId).RedCard = true;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(e => e.Contains("injur", StringComparison.OrdinalIgnoreCase));
        result.Errors.Should().Contain(e =>
            e.Contains("red card", StringComparison.OrdinalIgnoreCase) ||
            e.Contains("suspend", StringComparison.OrdinalIgnoreCase));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Edge Cases
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_WithNullLineup_ThrowsArgumentNullException()
    {
        // Arrange
        var squad = new List<Player>();

        // Act
        var act = () => _sut.ValidateLineup(null!, squad);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("lineup");
    }

    [Fact]
    public void ValidateLineup_WithNullSquad_ThrowsArgumentNullException()
    {
        // Arrange
        var lineup = new TeamLineup();

        // Act
        var act = () => _sut.ValidateLineup(lineup, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("squad");
    }

    [Fact]
    public void ValidateLineup_WithEmptySlots_ReturnsIsValidFalse()
    {
        // Arrange
        var lineup = new TeamLineup { Slots = new List<MatchLineupSlot>() };
        var squad = new List<Player>();

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse("empty lineup is not valid");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Adversarial / Over-Correction Tests
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_WithExactly11Starters_DoesNotFail()
    {
        // Arrange: Boundary test - exactly 11 starters should pass
        var (lineup, squad) = CreateValidLineupWithSquad();

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        lineup.Slots.Count(s => s.IsStarter).Should().Be(RequiredStarterCount);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateLineup_WithExactly1KeeperStarter_DoesNotFail()
    {
        // Arrange: Boundary test - exactly 1 keeper among starters should pass
        var (lineup, squad) = CreateValidLineupWithSquad();

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        lineup.Slots.Count(s => s.IsStarter && s.Position == Position.Keeper).Should().Be(RequiredKeeperCount);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateLineup_WithExactly3Substitutes_DoesNotFail()
    {
        // Arrange: Boundary test - exactly 3 substitutes should pass
        var (lineup, squad) = CreateValidLineupWithSquadAndSubstitutes(substituteCount: MaxSubstituteCount);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        lineup.Slots.Count(s => !s.IsStarter).Should().Be(MaxSubstituteCount);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateLineup_WithYellowCardsButNoRed_ReturnsIsValidTrue()
    {
        // Arrange: Yellow cards don't prevent lineup (only red cards do)
        var (lineup, squad) = CreateValidLineupWithSquad();
        var starterPlayerId = lineup.Slots.First(s => s.IsStarter).PlayerId;
        var playerWithYellow = squad.First(p => p.Id == starterPlayerId);
        playerWithYellow.YellowCards = 2; // Has yellow cards but no red
        playerWithYellow.RedCard = false;

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue("yellow cards alone don't prevent playing");
    }

    [Fact]
    public void ValidateLineup_DoesNotModifyLineup()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var originalSlotCount = lineup.Slots.Count;
        var originalFormation = lineup.Formation;
        var originalTactic = lineup.Tactic;

        // Act
        _sut.ValidateLineup(lineup, squad);

        // Assert
        lineup.Slots.Should().HaveCount(originalSlotCount);
        lineup.Formation.Should().Be(originalFormation);
        lineup.Tactic.Should().Be(originalTactic);
    }

    [Fact]
    public void ValidateLineup_DoesNotModifySquad()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();
        var originalSquadCount = squad.Count;
        var originalPlayerIds = squad.Select(p => p.Id).ToList();

        // Act
        _sut.ValidateLineup(lineup, squad);

        // Assert
        squad.Should().HaveCount(originalSquadCount);
        squad.Select(p => p.Id).Should().BeEquivalentTo(originalPlayerIds);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // LineupValidationResult Tests
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void ValidateLineup_WhenValid_ReturnsResultWithIsValidTrueAndEmptyErrors()
    {
        // Arrange
        var (lineup, squad) = CreateValidLineupWithSquad();

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().NotBeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateLineup_WhenInvalid_ReturnsResultWithIsValidFalseAndNonEmptyErrors()
    {
        // Arrange
        var (lineup, squad) = CreateLineupWithStarterCount(starterCount: 10);

        // Act
        var result = _sut.ValidateLineup(lineup, squad);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeNull();
        result.Errors.Should().NotBeEmpty();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helper Methods
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a valid lineup with exactly 11 starters including 1 keeper,
    /// and a squad containing those players.
    /// </summary>
    private static (TeamLineup Lineup, List<Player> Squad) CreateValidLineupWithSquad()
    {
        var squad = new List<Player>();
        var slots = new List<MatchLineupSlot>();

        // Add 1 keeper
        var keeper = CreatePlayer(Position.Keeper);
        squad.Add(keeper);
        slots.Add(new MatchLineupSlot(keeper.Id, Position.Keeper, isStarter: true));

        // Add 4 defenders (2 central, 2 wingbacks)
        for (var i = 0; i < 2; i++)
        {
            var centralDef = CreatePlayer(Position.CentralDefender);
            squad.Add(centralDef);
            slots.Add(new MatchLineupSlot(centralDef.Id, Position.CentralDefender, isStarter: true));
        }
        for (var i = 0; i < 2; i++)
        {
            var wingBack = CreatePlayer(Position.WingBack);
            squad.Add(wingBack);
            slots.Add(new MatchLineupSlot(wingBack.Id, Position.WingBack, isStarter: true));
        }

        // Add 4 midfielders (2 inner, 2 wingers)
        for (var i = 0; i < 2; i++)
        {
            var innerMid = CreatePlayer(Position.InnerMidfielder);
            squad.Add(innerMid);
            slots.Add(new MatchLineupSlot(innerMid.Id, Position.InnerMidfielder, isStarter: true));
        }
        for (var i = 0; i < 2; i++)
        {
            var winger = CreatePlayer(Position.Winger);
            squad.Add(winger);
            slots.Add(new MatchLineupSlot(winger.Id, Position.Winger, isStarter: true));
        }

        // Add 2 forwards
        for (var i = 0; i < 2; i++)
        {
            var forward = CreatePlayer(Position.Forward);
            squad.Add(forward);
            slots.Add(new MatchLineupSlot(forward.Id, Position.Forward, isStarter: true));
        }

        var lineup = new TeamLineup { Slots = slots };
        return (lineup, squad);
    }

    /// <summary>
    /// Creates a valid lineup with 11 starters and specified number of substitutes.
    /// </summary>
    private static (TeamLineup Lineup, List<Player> Squad) CreateValidLineupWithSquadAndSubstitutes(int substituteCount)
    {
        var (lineup, squad) = CreateValidLineupWithSquad();

        for (var i = 0; i < substituteCount; i++)
        {
            var sub = CreatePlayer(Position.Forward);
            squad.Add(sub);
            lineup.Slots.Add(new MatchLineupSlot(sub.Id, Position.Forward, isStarter: false));
        }

        return (lineup, squad);
    }

    /// <summary>
    /// Creates a lineup with the specified number of starters (may be invalid).
    /// </summary>
    private static (TeamLineup Lineup, List<Player> Squad) CreateLineupWithStarterCount(int starterCount)
    {
        var squad = new List<Player>();
        var slots = new List<MatchLineupSlot>();

        // Always add a keeper if we have at least 1 starter
        if (starterCount > 0)
        {
            var keeper = CreatePlayer(Position.Keeper);
            squad.Add(keeper);
            slots.Add(new MatchLineupSlot(keeper.Id, Position.Keeper, isStarter: true));
        }

        // Add remaining starters as forwards
        for (var i = 1; i < starterCount; i++)
        {
            var forward = CreatePlayer(Position.Forward);
            squad.Add(forward);
            slots.Add(new MatchLineupSlot(forward.Id, Position.Forward, isStarter: true));
        }

        var lineup = new TeamLineup { Slots = slots };
        return (lineup, squad);
    }

    /// <summary>
    /// Creates a lineup with the specified number of keepers among starters.
    /// </summary>
    private static (TeamLineup Lineup, List<Player> Squad) CreateLineupWithKeeperCount(int keeperCount)
    {
        var squad = new List<Player>();
        var slots = new List<MatchLineupSlot>();

        // Add specified number of keepers
        for (var i = 0; i < keeperCount; i++)
        {
            var keeper = CreatePlayer(Position.Keeper);
            squad.Add(keeper);
            slots.Add(new MatchLineupSlot(keeper.Id, Position.Keeper, isStarter: true));
        }

        // Fill remaining slots with forwards to reach 11 starters
        for (var i = keeperCount; i < RequiredStarterCount; i++)
        {
            var forward = CreatePlayer(Position.Forward);
            squad.Add(forward);
            slots.Add(new MatchLineupSlot(forward.Id, Position.Forward, isStarter: true));
        }

        var lineup = new TeamLineup { Slots = slots };
        return (lineup, squad);
    }

    /// <summary>
    /// Creates a test player with the specified position.
    /// </summary>
    private static Player CreatePlayer(Position position)
    {
        return new Player
        {
            Id = Guid.NewGuid(),
            Name = $"Test Player {Guid.NewGuid():N}",
            Age = 25,
            Form = 7,
            Stamina = 7,
            Experience = 3,
            BestPosition = position,
            InjuryWeeks = 0,
            RedCard = false,
            YellowCards = 0
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // SuggestLineup Tests - Phase 2 Sprint 2, Quartet 4 (AutoSuggest)
    // ═══════════════════════════════════════════════════════════════════════════
    //
    // Tests for SuggestLineup(Guid teamId, IReadOnlyList<Player> squad) method.
    //
    // Algorithm:
    // 1. Filter available players (InjuryWeeks=0, RedCard=false)
    // 2. Pick 1 best keeper by Keeper skill
    // 3. For 4-4-2: pick 4 defenders, 4 midfielders, 2 forwards based on BestPosition and skill
    //    - 2 Central Defenders (by Defending skill)
    //    - 2 Wing Backs (by Defending skill)
    //    - 2 Inner Midfielders (by Playmaking skill)
    //    - 2 Wingers (by Winger skill)
    //    - 2 Forwards (by Scoring skill)
    // 4. Add up to 3 best remaining players as substitutes
    // 5. Return TeamLineup with Formation442, Tactic.Normal, TeamAttitude.Normal
    // 6. Captain = player with highest Leadership among starters
    // 7. All slots have IndividualOrder.Normal
    // ═══════════════════════════════════════════════════════════════════════════

    // ─────────────────────────────────────────────────────────────────────────
    // SuggestLineup — Happy Path
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void SuggestLineup_With25PlayerSquad_Returns11StartersAnd3Substitutes()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 25);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var starters = lineup.Slots.Where(s => s.IsStarter).ToList();
        var subs = lineup.Slots.Where(s => !s.IsStarter).ToList();

        starters.Should().HaveCount(RequiredStarterCount, "must have exactly 11 starters");
        subs.Should().HaveCount(MaxSubstituteCount, "must have exactly 3 substitutes with large squad");
    }

    [Fact]
    public void SuggestLineup_WithValidSquad_ReturnsFormation442()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 25);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        lineup.Formation.Should().Be(Formation.Formation442);
    }

    [Fact]
    public void SuggestLineup_WithValidSquad_ReturnsTacticNormal()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 25);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        lineup.Tactic.Should().Be(Tactic.Normal);
    }

    [Fact]
    public void SuggestLineup_WithValidSquad_ReturnsTeamAttitudeNormal()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 25);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        lineup.Attitude.Should().Be(TeamAttitude.Normal);
    }

    [Fact]
    public void SuggestLineup_WithValidSquad_SetsTeamId()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 25);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        lineup.TeamId.Should().Be(teamId);
    }

    [Fact]
    public void SuggestLineup_WithValidSquad_AllSlotsHaveIndividualOrderNormal()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 25);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        lineup.Slots.Should().AllSatisfy(slot =>
            slot.IndividualOrder.Should().Be(IndividualOrder.Normal));
    }

    [Fact]
    public void SuggestLineup_WithValidSquad_StartersInclude1Keeper()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 25);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var keeperSlots = lineup.Slots.Where(s => s.IsStarter && s.Position == Position.Keeper);
        keeperSlots.Should().HaveCount(1, "4-4-2 requires exactly 1 keeper");
    }

    [Fact]
    public void SuggestLineup_WithValidSquad_StartersInclude4Defenders()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 25);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var defenderSlots = lineup.Slots.Where(s =>
            s.IsStarter &&
            (s.Position == Position.CentralDefender || s.Position == Position.WingBack));
        defenderSlots.Should().HaveCount(4, "4-4-2 requires exactly 4 defenders (2 CD + 2 WB)");
    }

    [Fact]
    public void SuggestLineup_WithValidSquad_StartersInclude4Midfielders()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 25);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var midSlots = lineup.Slots.Where(s =>
            s.IsStarter &&
            (s.Position == Position.InnerMidfielder || s.Position == Position.Winger));
        midSlots.Should().HaveCount(4, "4-4-2 requires exactly 4 midfielders (2 IM + 2 W)");
    }

    [Fact]
    public void SuggestLineup_WithValidSquad_StartersInclude2Forwards()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 25);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var forwardSlots = lineup.Slots.Where(s => s.IsStarter && s.Position == Position.Forward);
        forwardSlots.Should().HaveCount(2, "4-4-2 requires exactly 2 forwards");
    }

    [Fact]
    public void SuggestLineup_WithValidSquad_HasNoDuplicatePlayers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 25);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var playerIds = lineup.Slots.Select(s => s.PlayerId).ToList();
        playerIds.Should().OnlyHaveUniqueItems("each player should appear only once");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SuggestLineup — Skill-Based Selection
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void SuggestLineup_PicksHighestKeeperSkillForKeeperPosition()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 15);

        // Add two keepers with known skill values
        var bestKeeper = CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 18.0, leadership: 3);
        var worstKeeper = CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 10.0, leadership: 3);
        squad.Add(bestKeeper);
        squad.Add(worstKeeper);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var keeperSlot = lineup.Slots.Single(s => s.IsStarter && s.Position == Position.Keeper);
        keeperSlot.PlayerId.Should().Be(bestKeeper.Id,
            "keeper with highest Keeper skill (18.0) should be selected over one with (10.0)");
    }

    [Fact]
    public void SuggestLineup_PicksHighestDefendingSkillForCentralDefenders()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = new List<Player>();

        // Add keeper
        squad.Add(CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 10.0, leadership: 3));

        // Add CDs with varying Defending skill - best two should be picked
        var bestCD = CreatePlayerWithSkill(Position.CentralDefender, SkillType.Defending, 17.0, leadership: 3);
        var goodCD = CreatePlayerWithSkill(Position.CentralDefender, SkillType.Defending, 15.0, leadership: 3);
        var worstCD = CreatePlayerWithSkill(Position.CentralDefender, SkillType.Defending, 8.0, leadership: 3);
        squad.Add(bestCD);
        squad.Add(goodCD);
        squad.Add(worstCD);

        // Add other players to fill positions
        squad.AddRange(CreatePlayersForPosition(Position.WingBack, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.InnerMidfielder, 2, SkillType.Playmaking, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.Winger, 2, SkillType.Winger, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.Forward, 3, SkillType.Scoring, 10.0));

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var cdSlots = lineup.Slots.Where(s => s.IsStarter && s.Position == Position.CentralDefender).ToList();
        cdSlots.Should().HaveCount(2);
        cdSlots.Select(s => s.PlayerId).Should().Contain(bestCD.Id, "best CD should be selected");
        cdSlots.Select(s => s.PlayerId).Should().Contain(goodCD.Id, "second best CD should be selected");
        cdSlots.Select(s => s.PlayerId).Should().NotContain(worstCD.Id, "worst CD should NOT be selected");
    }

    [Fact]
    public void SuggestLineup_PicksHighestPlaymakingSkillForInnerMidfielders()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = new List<Player>();

        // Add keeper and defenders
        squad.Add(CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 10.0, leadership: 3));
        squad.AddRange(CreatePlayersForPosition(Position.CentralDefender, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.WingBack, 2, SkillType.Defending, 10.0));

        // Add IMs with varying Playmaking skill
        var bestIM = CreatePlayerWithSkill(Position.InnerMidfielder, SkillType.Playmaking, 16.0, leadership: 3);
        var goodIM = CreatePlayerWithSkill(Position.InnerMidfielder, SkillType.Playmaking, 14.0, leadership: 3);
        var worstIM = CreatePlayerWithSkill(Position.InnerMidfielder, SkillType.Playmaking, 7.0, leadership: 3);
        squad.Add(bestIM);
        squad.Add(goodIM);
        squad.Add(worstIM);

        // Add other players
        squad.AddRange(CreatePlayersForPosition(Position.Winger, 2, SkillType.Winger, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.Forward, 3, SkillType.Scoring, 10.0));

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var imSlots = lineup.Slots.Where(s => s.IsStarter && s.Position == Position.InnerMidfielder).ToList();
        imSlots.Should().HaveCount(2);
        imSlots.Select(s => s.PlayerId).Should().Contain(bestIM.Id);
        imSlots.Select(s => s.PlayerId).Should().Contain(goodIM.Id);
        imSlots.Select(s => s.PlayerId).Should().NotContain(worstIM.Id);
    }

    [Fact]
    public void SuggestLineup_PicksHighestWingerSkillForWingers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = new List<Player>();

        // Add keeper, defenders, IMs
        squad.Add(CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 10.0, leadership: 3));
        squad.AddRange(CreatePlayersForPosition(Position.CentralDefender, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.WingBack, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.InnerMidfielder, 2, SkillType.Playmaking, 10.0));

        // Add wingers with varying Winger skill
        var bestWinger = CreatePlayerWithSkill(Position.Winger, SkillType.Winger, 15.0, leadership: 3);
        var goodWinger = CreatePlayerWithSkill(Position.Winger, SkillType.Winger, 13.0, leadership: 3);
        var worstWinger = CreatePlayerWithSkill(Position.Winger, SkillType.Winger, 6.0, leadership: 3);
        squad.Add(bestWinger);
        squad.Add(goodWinger);
        squad.Add(worstWinger);

        // Add forwards
        squad.AddRange(CreatePlayersForPosition(Position.Forward, 3, SkillType.Scoring, 10.0));

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var wingerSlots = lineup.Slots.Where(s => s.IsStarter && s.Position == Position.Winger).ToList();
        wingerSlots.Should().HaveCount(2);
        wingerSlots.Select(s => s.PlayerId).Should().Contain(bestWinger.Id);
        wingerSlots.Select(s => s.PlayerId).Should().Contain(goodWinger.Id);
        wingerSlots.Select(s => s.PlayerId).Should().NotContain(worstWinger.Id);
    }

    [Fact]
    public void SuggestLineup_PicksHighestScoringSkillForForwards()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = new List<Player>();

        // Add keeper, defenders, midfielders
        squad.Add(CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 10.0, leadership: 3));
        squad.AddRange(CreatePlayersForPosition(Position.CentralDefender, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.WingBack, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.InnerMidfielder, 2, SkillType.Playmaking, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.Winger, 2, SkillType.Winger, 10.0));

        // Add forwards with varying Scoring skill
        var bestFwd = CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 18.0, leadership: 3);
        var goodFwd = CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 14.0, leadership: 3);
        var worstFwd = CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 5.0, leadership: 3);
        squad.Add(bestFwd);
        squad.Add(goodFwd);
        squad.Add(worstFwd);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var fwdSlots = lineup.Slots.Where(s => s.IsStarter && s.Position == Position.Forward).ToList();
        fwdSlots.Should().HaveCount(2);
        fwdSlots.Select(s => s.PlayerId).Should().Contain(bestFwd.Id);
        fwdSlots.Select(s => s.PlayerId).Should().Contain(goodFwd.Id);
        fwdSlots.Select(s => s.PlayerId).Should().NotContain(worstFwd.Id);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SuggestLineup — Excludes Injured/Red-Carded Players
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void SuggestLineup_ExcludesInjuredPlayers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = new List<Player>();

        // Best keeper is injured
        var injuredKeeper = CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 20.0, leadership: 3);
        injuredKeeper.InjuryWeeks = 2;

        var healthyKeeper = CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 10.0, leadership: 3);
        squad.Add(injuredKeeper);
        squad.Add(healthyKeeper);

        // Add other players
        squad.AddRange(CreatePlayersForPosition(Position.CentralDefender, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.WingBack, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.InnerMidfielder, 2, SkillType.Playmaking, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.Winger, 2, SkillType.Winger, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.Forward, 3, SkillType.Scoring, 10.0));

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var allPlayerIds = lineup.Slots.Select(s => s.PlayerId).ToList();
        allPlayerIds.Should().NotContain(injuredKeeper.Id, "injured player should be excluded");
        allPlayerIds.Should().Contain(healthyKeeper.Id, "healthy player should be selected instead");
    }

    [Fact]
    public void SuggestLineup_ExcludesRedCardedPlayers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = new List<Player>();

        // Best forward is red-carded
        var redCardedFwd = CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 20.0, leadership: 3);
        redCardedFwd.RedCard = true;

        var healthyFwd1 = CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 12.0, leadership: 3);
        var healthyFwd2 = CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 10.0, leadership: 3);

        squad.Add(redCardedFwd);
        squad.Add(healthyFwd1);
        squad.Add(healthyFwd2);

        // Add other players
        squad.Add(CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 10.0, leadership: 3));
        squad.AddRange(CreatePlayersForPosition(Position.CentralDefender, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.WingBack, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.InnerMidfielder, 2, SkillType.Playmaking, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.Winger, 2, SkillType.Winger, 10.0));
        squad.Add(CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 8.0, leadership: 3)); // Extra for subs

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var allPlayerIds = lineup.Slots.Select(s => s.PlayerId).ToList();
        allPlayerIds.Should().NotContain(redCardedFwd.Id, "red-carded player should be excluded");
    }

    [Fact]
    public void SuggestLineup_ExcludesBothInjuredAndRedCarded()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = new List<Player>();

        // Create unavailable players
        var injuredCD = CreatePlayerWithSkill(Position.CentralDefender, SkillType.Defending, 20.0, leadership: 3);
        injuredCD.InjuryWeeks = 3;

        var redCardedWinger = CreatePlayerWithSkill(Position.Winger, SkillType.Winger, 19.0, leadership: 3);
        redCardedWinger.RedCard = true;

        squad.Add(injuredCD);
        squad.Add(redCardedWinger);

        // Add available players
        squad.Add(CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 10.0, leadership: 3));
        squad.AddRange(CreatePlayersForPosition(Position.CentralDefender, 3, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.WingBack, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.InnerMidfielder, 2, SkillType.Playmaking, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.Winger, 3, SkillType.Winger, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.Forward, 3, SkillType.Scoring, 10.0));

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var allPlayerIds = lineup.Slots.Select(s => s.PlayerId).ToList();
        allPlayerIds.Should().NotContain(injuredCD.Id);
        allPlayerIds.Should().NotContain(redCardedWinger.Id);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SuggestLineup — Captain Selection (Highest Leadership)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void SuggestLineup_SelectsCaptainWithHighestLeadership()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = new List<Player>();

        // Add keeper with low leadership
        var keeper = CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 12.0, leadership: 3);
        squad.Add(keeper);

        // Add CD with highest leadership - should be captain
        var leaderCD = CreatePlayerWithSkill(Position.CentralDefender, SkillType.Defending, 14.0, leadership: 8);
        squad.Add(leaderCD);

        // Add other players with lower leadership
        squad.Add(CreatePlayerWithSkill(Position.CentralDefender, SkillType.Defending, 12.0, leadership: 5));
        squad.AddRange(CreatePlayersForPosition(Position.WingBack, 2, SkillType.Defending, 10.0, leadership: 4));
        squad.AddRange(CreatePlayersForPosition(Position.InnerMidfielder, 2, SkillType.Playmaking, 10.0, leadership: 4));
        squad.AddRange(CreatePlayersForPosition(Position.Winger, 2, SkillType.Winger, 10.0, leadership: 4));
        squad.AddRange(CreatePlayersForPosition(Position.Forward, 3, SkillType.Scoring, 10.0, leadership: 4));

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        lineup.CaptainId.Should().Be(leaderCD.Id,
            "player with highest Leadership (8) should be captain");
    }

    [Fact]
    public void SuggestLineup_CaptainMustBeInStartingLineup()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 20);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        lineup.CaptainId.Should().NotBeNull();
        var starterIds = lineup.Slots.Where(s => s.IsStarter).Select(s => s.PlayerId).ToList();
        starterIds.Should().Contain(lineup.CaptainId.Value,
            "captain must be among the starting 11");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SuggestLineup — Edge Cases
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void SuggestLineup_WithExactly11AvailablePlayers_Returns11StartersAnd0Substitutes()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = new List<Player>
        {
            CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 10.0, leadership: 5),
            CreatePlayerWithSkill(Position.CentralDefender, SkillType.Defending, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.CentralDefender, SkillType.Defending, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.WingBack, SkillType.Defending, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.WingBack, SkillType.Defending, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.InnerMidfielder, SkillType.Playmaking, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.InnerMidfielder, SkillType.Playmaking, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Winger, SkillType.Winger, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Winger, SkillType.Winger, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 10.0, leadership: 3)
        };

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        lineup.Slots.Where(s => s.IsStarter).Should().HaveCount(RequiredStarterCount);
        lineup.Slots.Where(s => !s.IsStarter).Should().BeEmpty("no players left for substitutes");
    }

    [Fact]
    public void SuggestLineup_WithExactly12Players_Returns11StartersAnd1Substitute()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = new List<Player>
        {
            CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 10.0, leadership: 5),
            CreatePlayerWithSkill(Position.CentralDefender, SkillType.Defending, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.CentralDefender, SkillType.Defending, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.WingBack, SkillType.Defending, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.WingBack, SkillType.Defending, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.InnerMidfielder, SkillType.Playmaking, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.InnerMidfielder, SkillType.Playmaking, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Winger, SkillType.Winger, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Winger, SkillType.Winger, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 8.0, leadership: 3) // Extra = sub
        };

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        lineup.Slots.Where(s => s.IsStarter).Should().HaveCount(RequiredStarterCount);
        lineup.Slots.Where(s => !s.IsStarter).Should().HaveCount(1, "1 player left for substitute");
    }

    [Fact]
    public void SuggestLineup_WithFewerThan11AvailablePlayers_ThrowsInvalidOperationException()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = new List<Player>
        {
            CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 10.0, leadership: 5),
            CreatePlayerWithSkill(Position.CentralDefender, SkillType.Defending, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 10.0, leadership: 3)
            // Only 3 players - not enough for 11
        };

        // Act
        var act = () => _sut.SuggestLineup(teamId, squad);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*11*", "should indicate minimum 11 players needed");
    }

    [Fact]
    public void SuggestLineup_WithNoKeeper_FallsBackToOtherPlayer()
    {
        // Arrange: Squad has no keeper, must pick someone else for that slot
        var teamId = Guid.NewGuid();
        var squad = new List<Player>();

        // No keepers - all field players
        squad.AddRange(CreatePlayersForPosition(Position.CentralDefender, 3, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.WingBack, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.InnerMidfielder, 3, SkillType.Playmaking, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.Winger, 2, SkillType.Winger, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.Forward, 3, SkillType.Scoring, 10.0));

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert: Should still produce a lineup with someone in keeper slot
        var keeperSlot = lineup.Slots.SingleOrDefault(s => s.IsStarter && s.Position == Position.Keeper);
        keeperSlot.Should().NotBeNull("a keeper slot must be filled even without natural keepers");
        lineup.Slots.Where(s => s.IsStarter).Should().HaveCount(RequiredStarterCount);
    }

    [Fact]
    public void SuggestLineup_WithNullSquad_ThrowsArgumentNullException()
    {
        // Arrange
        var teamId = Guid.NewGuid();

        // Act
        var act = () => _sut.SuggestLineup(teamId, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("squad");
    }

    [Fact]
    public void SuggestLineup_WithEmptySquad_ThrowsInvalidOperationException()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = new List<Player>();

        // Act
        var act = () => _sut.SuggestLineup(teamId, squad);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SuggestLineup — Fallback Behavior (Not Enough Position-Specific Players)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void SuggestLineup_WithOnly1CentralDefender_FillsOtherDefenderFromAvailable()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = new List<Player>
        {
            CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 10.0, leadership: 5),
            // Only 1 natural CD
            CreatePlayerWithSkill(Position.CentralDefender, SkillType.Defending, 12.0, leadership: 3),
            // 2 WBs - expected
            CreatePlayerWithSkill(Position.WingBack, SkillType.Defending, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.WingBack, SkillType.Defending, 10.0, leadership: 3),
            // Extra players with Defending skill that can fill CD
            CreatePlayerWithSkill(Position.InnerMidfielder, SkillType.Playmaking, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.InnerMidfielder, SkillType.Playmaking, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Winger, SkillType.Winger, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Winger, SkillType.Winger, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 10.0, leadership: 3),
            CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 8.0, leadership: 3) // Extra for fallback
        };

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert: Should still have 2 CD slots filled
        var cdSlots = lineup.Slots.Where(s => s.IsStarter && s.Position == Position.CentralDefender);
        cdSlots.Should().HaveCount(2, "2 CD positions must be filled even if not all are natural CDs");
        lineup.Slots.Where(s => s.IsStarter).Should().HaveCount(RequiredStarterCount);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SuggestLineup — Adversarial / Over-Correction Tests
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void SuggestLineup_DoesNotModifyInputSquad()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 20);
        var originalCount = squad.Count;
        var originalIds = squad.Select(p => p.Id).ToList();
        var originalInjuryWeeks = squad.Select(p => p.InjuryWeeks).ToList();

        // Act
        _sut.SuggestLineup(teamId, squad);

        // Assert: Squad should be unchanged
        squad.Should().HaveCount(originalCount);
        squad.Select(p => p.Id).Should().BeEquivalentTo(originalIds);
        squad.Select(p => p.InjuryWeeks).Should().BeEquivalentTo(originalInjuryWeeks);
    }

    [Fact]
    public void SuggestLineup_ReturnedLineupPassesValidation()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 25);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert: The suggested lineup should be valid according to ValidateLineup
        var validationResult = _sut.ValidateLineup(lineup, squad);
        validationResult.IsValid.Should().BeTrue(
            "auto-suggested lineup should pass all validation rules. Errors: {0}",
            string.Join(", ", validationResult.Errors));
    }

    [Fact]
    public void SuggestLineup_YellowCardsDoNotExcludePlayers()
    {
        // Arrange: Player with 2 yellow cards should still be available
        var teamId = Guid.NewGuid();
        var squad = new List<Player>();

        var yellowCardedFwd = CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 18.0, leadership: 5);
        yellowCardedFwd.YellowCards = 2;
        yellowCardedFwd.RedCard = false;

        squad.Add(yellowCardedFwd);
        squad.Add(CreatePlayerWithSkill(Position.Keeper, SkillType.Keeper, 10.0, leadership: 3));
        squad.AddRange(CreatePlayersForPosition(Position.CentralDefender, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.WingBack, 2, SkillType.Defending, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.InnerMidfielder, 2, SkillType.Playmaking, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.Winger, 2, SkillType.Winger, 10.0));
        squad.AddRange(CreatePlayersForPosition(Position.Forward, 2, SkillType.Scoring, 10.0));

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert: Player with yellow cards should be included (best forward)
        var forwardSlots = lineup.Slots.Where(s => s.IsStarter && s.Position == Position.Forward);
        forwardSlots.Select(s => s.PlayerId).Should().Contain(yellowCardedFwd.Id,
            "yellow cards do not prevent selection, only red cards");
    }

    [Fact]
    public void SuggestLineup_SubstitutesAreNotStarters()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var squad = CreateFullSquad(playerCount: 20);

        // Act
        var lineup = _sut.SuggestLineup(teamId, squad);

        // Assert
        var subs = lineup.Slots.Where(s => !s.IsStarter).ToList();
        var starterIds = lineup.Slots.Where(s => s.IsStarter).Select(s => s.PlayerId).ToHashSet();

        subs.Should().AllSatisfy(sub =>
            starterIds.Should().NotContain(sub.PlayerId, "substitute should not also be a starter"));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SuggestLineup — Helper Methods
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a full squad with a mix of positions suitable for SuggestLineup tests.
    /// </summary>
    private static List<Player> CreateFullSquad(int playerCount)
    {
        var squad = new List<Player>();
        var positions = new[]
        {
            (Position.Keeper, SkillType.Keeper, 2),
            (Position.CentralDefender, SkillType.Defending, 4),
            (Position.WingBack, SkillType.Defending, 3),
            (Position.InnerMidfielder, SkillType.Playmaking, 4),
            (Position.Winger, SkillType.Winger, 3),
            (Position.Forward, SkillType.Scoring, 4)
        };

        var leadershipCounter = 1;
        foreach (var (position, skillType, count) in positions)
        {
            for (var i = 0; i < count && squad.Count < playerCount; i++)
            {
                squad.Add(CreatePlayerWithSkill(position, skillType, 8.0 + (i * 0.5), leadershipCounter++));
            }
        }

        // Fill remaining with forwards
        while (squad.Count < playerCount)
        {
            squad.Add(CreatePlayerWithSkill(Position.Forward, SkillType.Scoring, 6.0, leadershipCounter++));
        }

        return squad;
    }

    /// <summary>
    /// Creates a player with a specific skill value for testing skill-based selection.
    /// </summary>
    private static Player CreatePlayerWithSkill(Position position, SkillType primarySkill, double skillValue, int leadership)
    {
        var player = new Player
        {
            Id = Guid.NewGuid(),
            Name = $"Test Player {Guid.NewGuid():N}",
            Age = 25,
            Form = 7,
            Stamina = 7,
            Experience = 3,
            BestPosition = position,
            InjuryWeeks = 0,
            RedCard = false,
            YellowCards = 0,
            Leadership = leadership,
            Skills = new Dictionary<SkillType, double>
            {
                { SkillType.Keeper, primarySkill == SkillType.Keeper ? skillValue : 1.0 },
                { SkillType.Defending, primarySkill == SkillType.Defending ? skillValue : 1.0 },
                { SkillType.Playmaking, primarySkill == SkillType.Playmaking ? skillValue : 1.0 },
                { SkillType.Winger, primarySkill == SkillType.Winger ? skillValue : 1.0 },
                { SkillType.Passing, 1.0 },
                { SkillType.Scoring, primarySkill == SkillType.Scoring ? skillValue : 1.0 },
                { SkillType.SetPieces, 1.0 },
                { SkillType.Stamina, 7.0 }
            }
        };
        return player;
    }

    /// <summary>
    /// Creates multiple players for a specific position.
    /// </summary>
    private static List<Player> CreatePlayersForPosition(Position position, int count, SkillType primarySkill, double skillValue, int leadership = 3)
    {
        var players = new List<Player>();
        for (var i = 0; i < count; i++)
        {
            players.Add(CreatePlayerWithSkill(position, primarySkill, skillValue, leadership));
        }
        return players;
    }
}
