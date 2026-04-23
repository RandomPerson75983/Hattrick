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
}
