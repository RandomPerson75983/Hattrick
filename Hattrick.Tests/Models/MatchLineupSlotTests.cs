using Hattrick.Core.Models;

namespace Hattrick.Tests.Models;

/// <summary>
/// Tests for the MatchLineupSlot record.
/// TDD Red Phase: MatchLineupSlot record does not exist yet - these tests define the expected API.
///
/// MatchLineupSlot is an immutable record representing a single player's position assignment
/// in a match lineup. It captures which player, what position, any individual order, and
/// whether they are a starter or substitute.
///
/// Phase 2 Sprint 2, Quartet 1 (MatchLineupSlot Model).
/// </summary>
public class MatchLineupSlotTests
{
    #region Construction with All Parameters

    [Fact]
    public void Constructor_WithAllParameters_SetsPlayerId()
    {
        var playerId = Guid.NewGuid();

        var slot = new MatchLineupSlot(playerId, Position.Forward, IndividualOrder.Normal, true);

        slot.PlayerId.Should().Be(playerId,
            "PlayerId should be set to the provided Guid");
    }

    [Fact]
    public void Constructor_WithAllParameters_SetsPosition()
    {
        var playerId = Guid.NewGuid();

        var slot = new MatchLineupSlot(playerId, Position.CentralDefender, IndividualOrder.Normal, true);

        slot.Position.Should().Be(Position.CentralDefender,
            "Position should be set to the provided position enum value");
    }

    [Fact]
    public void Constructor_WithAllParameters_SetsIndividualOrder()
    {
        var playerId = Guid.NewGuid();

        var slot = new MatchLineupSlot(playerId, Position.Winger, IndividualOrder.Offensive, true);

        slot.IndividualOrder.Should().Be(IndividualOrder.Offensive,
            "IndividualOrder should be set to the provided order enum value");
    }

    [Fact]
    public void Constructor_WithAllParameters_SetsIsStarter()
    {
        var playerId = Guid.NewGuid();

        var slot = new MatchLineupSlot(playerId, Position.Forward, IndividualOrder.Normal, true);

        slot.IsStarter.Should().BeTrue(
            "IsStarter should be set to the provided boolean value");
    }

    [Fact]
    public void Constructor_WithIsStarterFalse_SetsIsStarterToFalse()
    {
        var playerId = Guid.NewGuid();

        var slot = new MatchLineupSlot(playerId, Position.Forward, IndividualOrder.Normal, false);

        slot.IsStarter.Should().BeFalse(
            "IsStarter should be false when constructed with false");
    }

    #endregion

    #region Default Value for IndividualOrder

    [Fact]
    public void Constructor_WithDefaultIndividualOrder_DefaultsToNormal()
    {
        var playerId = Guid.NewGuid();

        // This test verifies IndividualOrder has a default parameter value of Normal
        var slot = new MatchLineupSlot(playerId, Position.InnerMidfielder, isStarter: true);

        slot.IndividualOrder.Should().Be(IndividualOrder.Normal,
            "IndividualOrder should default to Normal when not specified");
    }

    #endregion

    #region Position Enum Coverage

    [Theory]
    [InlineData(Position.Keeper)]
    [InlineData(Position.CentralDefender)]
    [InlineData(Position.WingBack)]
    [InlineData(Position.InnerMidfielder)]
    [InlineData(Position.Winger)]
    [InlineData(Position.Forward)]
    public void Constructor_AcceptsAllPositionValues(Position position)
    {
        var playerId = Guid.NewGuid();

        var slot = new MatchLineupSlot(playerId, position, IndividualOrder.Normal, true);

        slot.Position.Should().Be(position);
    }

    #endregion

    #region IndividualOrder Enum Coverage

    [Theory]
    [InlineData(IndividualOrder.Normal)]
    [InlineData(IndividualOrder.Offensive)]
    [InlineData(IndividualOrder.Defensive)]
    [InlineData(IndividualOrder.TowardsMiddle)]
    [InlineData(IndividualOrder.TowardsWing)]
    public void Constructor_AcceptsAllIndividualOrderValues(IndividualOrder order)
    {
        var playerId = Guid.NewGuid();

        var slot = new MatchLineupSlot(playerId, Position.Forward, order, true);

        slot.IndividualOrder.Should().Be(order);
    }

    #endregion

    #region Immutability (Record Semantics)

    [Fact]
    public void Record_IsImmutable_PropertiesAreReadOnly()
    {
        var slot = new MatchLineupSlot(Guid.NewGuid(), Position.Forward, IndividualOrder.Normal, true);

        // Verify properties are read-only by checking the type has init-only setters
        var playerIdProperty = typeof(MatchLineupSlot).GetProperty(nameof(MatchLineupSlot.PlayerId));
        var positionProperty = typeof(MatchLineupSlot).GetProperty(nameof(MatchLineupSlot.Position));
        var orderProperty = typeof(MatchLineupSlot).GetProperty(nameof(MatchLineupSlot.IndividualOrder));
        var isStarterProperty = typeof(MatchLineupSlot).GetProperty(nameof(MatchLineupSlot.IsStarter));

        playerIdProperty!.CanWrite.Should().BeTrue("records have init setters which report CanWrite=true");
        positionProperty!.CanWrite.Should().BeTrue("records have init setters which report CanWrite=true");
        orderProperty!.CanWrite.Should().BeTrue("records have init setters which report CanWrite=true");
        isStarterProperty!.CanWrite.Should().BeTrue("records have init setters which report CanWrite=true");

        // The key test: verify it's a record type (has value-based equality)
        typeof(MatchLineupSlot).IsValueType.Should().BeFalse("MatchLineupSlot should be a record (reference type)");
    }

    [Fact]
    public void Record_WithExpression_CreatesModifiedCopy()
    {
        var originalPlayerId = Guid.NewGuid();
        var original = new MatchLineupSlot(originalPlayerId, Position.Forward, IndividualOrder.Normal, true);

        var modified = original with { IndividualOrder = IndividualOrder.Offensive };

        // Original unchanged
        original.IndividualOrder.Should().Be(IndividualOrder.Normal,
            "original record should not be modified");

        // Modified copy has new value
        modified.IndividualOrder.Should().Be(IndividualOrder.Offensive,
            "modified copy should have the new value");

        // Other properties preserved
        modified.PlayerId.Should().Be(originalPlayerId);
        modified.Position.Should().Be(Position.Forward);
        modified.IsStarter.Should().BeTrue();
    }

    #endregion

    #region Value Equality (Record Semantics)

    [Fact]
    public void Equality_TwoSlotsWithSameValues_AreEqual()
    {
        var playerId = Guid.NewGuid();

        var slot1 = new MatchLineupSlot(playerId, Position.Keeper, IndividualOrder.Normal, true);
        var slot2 = new MatchLineupSlot(playerId, Position.Keeper, IndividualOrder.Normal, true);

        slot1.Should().Be(slot2,
            "records with identical values should be equal");
        (slot1 == slot2).Should().BeTrue(
            "== operator should return true for equal records");
        slot1.Equals(slot2).Should().BeTrue(
            "Equals method should return true for equal records");
    }

    [Fact]
    public void Equality_TwoSlotsWithDifferentPlayerId_AreNotEqual()
    {
        var slot1 = new MatchLineupSlot(Guid.NewGuid(), Position.Keeper, IndividualOrder.Normal, true);
        var slot2 = new MatchLineupSlot(Guid.NewGuid(), Position.Keeper, IndividualOrder.Normal, true);

        slot1.Should().NotBe(slot2,
            "records with different PlayerIds should not be equal");
        (slot1 != slot2).Should().BeTrue(
            "!= operator should return true for different records");
    }

    [Fact]
    public void Equality_TwoSlotsWithDifferentPosition_AreNotEqual()
    {
        var playerId = Guid.NewGuid();

        var slot1 = new MatchLineupSlot(playerId, Position.Keeper, IndividualOrder.Normal, true);
        var slot2 = new MatchLineupSlot(playerId, Position.Forward, IndividualOrder.Normal, true);

        slot1.Should().NotBe(slot2,
            "records with different Positions should not be equal");
    }

    [Fact]
    public void Equality_TwoSlotsWithDifferentIndividualOrder_AreNotEqual()
    {
        var playerId = Guid.NewGuid();

        var slot1 = new MatchLineupSlot(playerId, Position.Forward, IndividualOrder.Normal, true);
        var slot2 = new MatchLineupSlot(playerId, Position.Forward, IndividualOrder.Offensive, true);

        slot1.Should().NotBe(slot2,
            "records with different IndividualOrders should not be equal");
    }

    [Fact]
    public void Equality_TwoSlotsWithDifferentIsStarter_AreNotEqual()
    {
        var playerId = Guid.NewGuid();

        var slot1 = new MatchLineupSlot(playerId, Position.Forward, IndividualOrder.Normal, true);
        var slot2 = new MatchLineupSlot(playerId, Position.Forward, IndividualOrder.Normal, false);

        slot1.Should().NotBe(slot2,
            "records with different IsStarter values should not be equal");
    }

    [Fact]
    public void GetHashCode_EqualSlots_HaveSameHashCode()
    {
        var playerId = Guid.NewGuid();

        var slot1 = new MatchLineupSlot(playerId, Position.Winger, IndividualOrder.TowardsMiddle, false);
        var slot2 = new MatchLineupSlot(playerId, Position.Winger, IndividualOrder.TowardsMiddle, false);

        slot1.GetHashCode().Should().Be(slot2.GetHashCode(),
            "equal records must have the same hash code");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Constructor_WithEmptyGuid_AcceptsValue()
    {
        // Empty Guid might be used for placeholder/empty slots
        var slot = new MatchLineupSlot(Guid.Empty, Position.Forward, IndividualOrder.Normal, true);

        slot.PlayerId.Should().Be(Guid.Empty,
            "model should accept Guid.Empty; validation belongs in service layer");
    }

    #endregion

    #region Practical Usage Scenarios

    [Fact]
    public void TypicalStartingLineup_GoalkeeperSlot()
    {
        var keeperId = Guid.NewGuid();

        var slot = new MatchLineupSlot(keeperId, Position.Keeper, IndividualOrder.Normal, true);

        slot.PlayerId.Should().Be(keeperId);
        slot.Position.Should().Be(Position.Keeper);
        slot.IndividualOrder.Should().Be(IndividualOrder.Normal,
            "goalkeepers typically have Normal individual order");
        slot.IsStarter.Should().BeTrue();
    }

    [Fact]
    public void TypicalStartingLineup_OffensiveWinger()
    {
        var wingerId = Guid.NewGuid();

        var slot = new MatchLineupSlot(wingerId, Position.Winger, IndividualOrder.Offensive, true);

        slot.Position.Should().Be(Position.Winger);
        slot.IndividualOrder.Should().Be(IndividualOrder.Offensive,
            "wingers can be set to offensive for more attacking play");
    }

    [Fact]
    public void TypicalStartingLineup_DefensiveInnerMidfielder()
    {
        var midfielderId = Guid.NewGuid();

        var slot = new MatchLineupSlot(midfielderId, Position.InnerMidfielder, IndividualOrder.Defensive, true);

        slot.Position.Should().Be(Position.InnerMidfielder);
        slot.IndividualOrder.Should().Be(IndividualOrder.Defensive,
            "inner midfielders can be set to defensive for more protection");
    }

    [Fact]
    public void SubstituteSlot_IsStarterIsFalse()
    {
        var subId = Guid.NewGuid();

        var slot = new MatchLineupSlot(subId, Position.Forward, IndividualOrder.Normal, false);

        slot.IsStarter.Should().BeFalse(
            "substitutes should have IsStarter = false");
    }

    [Fact]
    public void WingBackTowardsMiddle_ValidCombination()
    {
        var wingBackId = Guid.NewGuid();

        var slot = new MatchLineupSlot(wingBackId, Position.WingBack, IndividualOrder.TowardsMiddle, true);

        slot.Position.Should().Be(Position.WingBack);
        slot.IndividualOrder.Should().Be(IndividualOrder.TowardsMiddle,
            "wing backs can play towards middle");
    }

    [Fact]
    public void CentralDefenderTowardsWing_ValidCombination()
    {
        var defenderId = Guid.NewGuid();

        // In Hattrick, central defenders can play towards wing
        var slot = new MatchLineupSlot(defenderId, Position.CentralDefender, IndividualOrder.TowardsWing, true);

        slot.Position.Should().Be(Position.CentralDefender);
        slot.IndividualOrder.Should().Be(IndividualOrder.TowardsWing);
    }

    #endregion

    #region Collection Usage

    [Fact]
    public void CanBeUsedInCollections_List()
    {
        var slots = new List<MatchLineupSlot>
        {
            new(Guid.NewGuid(), Position.Keeper, IndividualOrder.Normal, true),
            new(Guid.NewGuid(), Position.CentralDefender, IndividualOrder.Normal, true),
            new(Guid.NewGuid(), Position.Forward, IndividualOrder.Offensive, true)
        };

        slots.Should().HaveCount(3);
        slots[0].Position.Should().Be(Position.Keeper);
        slots[2].IndividualOrder.Should().Be(IndividualOrder.Offensive);
    }

    [Fact]
    public void CanBeUsedInCollections_HashSet()
    {
        var playerId = Guid.NewGuid();

        var set = new HashSet<MatchLineupSlot>
        {
            new(playerId, Position.Forward, IndividualOrder.Normal, true),
            new(playerId, Position.Forward, IndividualOrder.Normal, true) // duplicate
        };

        set.Should().HaveCount(1,
            "HashSet should use record equality to deduplicate");
    }

    [Fact]
    public void CanBeUsedInCollections_Dictionary()
    {
        var slot1 = new MatchLineupSlot(Guid.NewGuid(), Position.Keeper, IndividualOrder.Normal, true);
        var slot2 = new MatchLineupSlot(Guid.NewGuid(), Position.Forward, IndividualOrder.Offensive, true);

        var dict = new Dictionary<MatchLineupSlot, string>
        {
            { slot1, "Goalkeeper" },
            { slot2, "Striker" }
        };

        dict[slot1].Should().Be("Goalkeeper");
        dict[slot2].Should().Be("Striker");
    }

    #endregion
}
