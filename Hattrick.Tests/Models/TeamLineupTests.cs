using Hattrick.Core.Models;

namespace Hattrick.Tests.Models;

/// <summary>
/// Tests for the TeamLineup model class.
/// TDD Red Phase: TeamLineup class does not exist yet - these tests define the expected API.
///
/// TeamLineup is a mutable class representing a team's lineup configuration for a match.
/// It includes formation, tactics, attitude, player slots, and special role assignments.
///
/// Phase 2 Sprint 2, Quartet 2 (TeamLineup Model).
/// </summary>
public class TeamLineupTests
{
    #region Constants

    [Fact]
    public void StarterCount_Constant_IsEleven()
    {
        TeamLineup.StarterCount.Should().Be(11,
            "a football match requires exactly 11 starting players");
    }

    [Fact]
    public void MaxSubstituteCount_Constant_IsThree()
    {
        TeamLineup.MaxSubstituteCount.Should().Be(3,
            "Hattrick allows a maximum of 3 substitutes on the bench");
    }

    [Fact]
    public void MaxTotalSlots_Constant_IsFourteen()
    {
        TeamLineup.MaxTotalSlots.Should().Be(14,
            "total slots = 11 starters + 3 substitutes = 14");
    }

    [Fact]
    public void MaxTotalSlots_EqualsStartersPlusSubstitutes()
    {
        var expected = TeamLineup.StarterCount + TeamLineup.MaxSubstituteCount;

        TeamLineup.MaxTotalSlots.Should().Be(expected,
            "MaxTotalSlots should equal StarterCount + MaxSubstituteCount");
    }

    #endregion

    #region Construction & Defaults

    [Fact]
    public void Constructor_Default_TeamIdIsEmptyGuid()
    {
        var lineup = new TeamLineup();

        lineup.TeamId.Should().Be(Guid.Empty,
            "TeamId should default to Guid.Empty until assigned to a team");
    }

    [Fact]
    public void Constructor_Default_FormationIsFormation442()
    {
        var lineup = new TeamLineup();

        lineup.Formation.Should().Be(Formation.Formation442,
            "Formation should default to 442 (the zero-value enum member)");
    }

    [Fact]
    public void Constructor_Default_TacticIsNormal()
    {
        var lineup = new TeamLineup();

        lineup.Tactic.Should().Be(Tactic.Normal,
            "Tactic should default to Normal (the zero-value enum member)");
    }

    [Fact]
    public void Constructor_Default_AttitudeIsPlayItCool()
    {
        var lineup = new TeamLineup();

        lineup.Attitude.Should().Be(TeamAttitude.PlayItCool,
            "Attitude should default to PlayItCool (the zero-value enum member)");
    }

    [Fact]
    public void Constructor_Default_SlotsIsInitializedToEmptyList()
    {
        var lineup = new TeamLineup();

        lineup.Slots.Should().NotBeNull(
            "Slots list must be initialized to avoid NullReferenceException");
        lineup.Slots.Should().BeEmpty(
            "Slots list should start empty until slots are assigned");
    }

    [Fact]
    public void Constructor_Default_SetPiecesTakerIdIsNull()
    {
        var lineup = new TeamLineup();

        lineup.SetPiecesTakerId.Should().BeNull(
            "SetPiecesTakerId should default to null (no set pieces taker assigned)");
    }

    [Fact]
    public void Constructor_Default_CaptainIdIsNull()
    {
        var lineup = new TeamLineup();

        lineup.CaptainId.Should().BeNull(
            "CaptainId should default to null (no captain assigned)");
    }

    #endregion

    #region TeamId Property

    [Fact]
    public void TeamId_CanBeSetToSpecificGuid()
    {
        var teamId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var lineup = new TeamLineup { TeamId = teamId };

        lineup.TeamId.Should().Be(teamId);
    }

    [Fact]
    public void TeamId_CanBeSetAndRetrieved()
    {
        var teamId = Guid.NewGuid();
        var lineup = new TeamLineup { TeamId = teamId };

        lineup.TeamId.Should().Be(teamId);
    }

    #endregion

    #region Formation Property

    [Theory]
    [InlineData(Formation.Formation442)]
    [InlineData(Formation.Formation352)]
    [InlineData(Formation.Formation433)]
    [InlineData(Formation.Formation343)]
    [InlineData(Formation.Formation541)]
    [InlineData(Formation.Formation451)]
    [InlineData(Formation.Formation532)]
    [InlineData(Formation.Formation523)]
    [InlineData(Formation.Formation550)]
    [InlineData(Formation.Formation253)]
    public void Formation_AcceptsAllValidValues(Formation formation)
    {
        var lineup = new TeamLineup { Formation = formation };

        lineup.Formation.Should().Be(formation);
    }

    [Fact]
    public void Formation_CanBeUpdatedAfterConstruction()
    {
        var lineup = new TeamLineup { Formation = Formation.Formation442 };
        lineup.Formation = Formation.Formation352;

        lineup.Formation.Should().Be(Formation.Formation352);
    }

    #endregion

    #region Tactic Property

    [Theory]
    [InlineData(Tactic.Normal)]
    [InlineData(Tactic.Pressing)]
    [InlineData(Tactic.CounterAttack)]
    [InlineData(Tactic.AttackInMiddle)]
    [InlineData(Tactic.AttackOnWings)]
    [InlineData(Tactic.PlayCreatively)]
    [InlineData(Tactic.LongShots)]
    public void Tactic_AcceptsAllValidValues(Tactic tactic)
    {
        var lineup = new TeamLineup { Tactic = tactic };

        lineup.Tactic.Should().Be(tactic);
    }

    [Fact]
    public void Tactic_CanBeUpdatedAfterConstruction()
    {
        var lineup = new TeamLineup { Tactic = Tactic.Normal };
        lineup.Tactic = Tactic.Pressing;

        lineup.Tactic.Should().Be(Tactic.Pressing);
    }

    #endregion

    #region Attitude Property

    [Theory]
    [InlineData(TeamAttitude.PlayItCool)]
    [InlineData(TeamAttitude.Normal)]
    [InlineData(TeamAttitude.MatchOfTheSeason)]
    public void Attitude_AcceptsAllValidValues(TeamAttitude attitude)
    {
        var lineup = new TeamLineup { Attitude = attitude };

        lineup.Attitude.Should().Be(attitude);
    }

    [Fact]
    public void Attitude_CanBeUpdatedAfterConstruction()
    {
        var lineup = new TeamLineup { Attitude = TeamAttitude.PlayItCool };
        lineup.Attitude = TeamAttitude.MatchOfTheSeason;

        lineup.Attitude.Should().Be(TeamAttitude.MatchOfTheSeason);
    }

    #endregion

    #region Slots Property (List<MatchLineupSlot>)

    [Fact]
    public void Slots_CanAddSlot()
    {
        var lineup = new TeamLineup();
        var slot = new MatchLineupSlot(Guid.NewGuid(), Position.Keeper, IndividualOrder.Normal, true);

        lineup.Slots.Add(slot);

        lineup.Slots.Should().HaveCount(1);
        lineup.Slots[0].Should().Be(slot);
    }

    [Fact]
    public void Slots_CanAddMultipleSlots()
    {
        var lineup = new TeamLineup();

        lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.Keeper, IndividualOrder.Normal, true));
        lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.CentralDefender, IndividualOrder.Normal, true));
        lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.Forward, IndividualOrder.Offensive, true));

        lineup.Slots.Should().HaveCount(3);
    }

    [Fact]
    public void Slots_CanRemoveSlot()
    {
        var lineup = new TeamLineup();
        var slot = new MatchLineupSlot(Guid.NewGuid(), Position.Forward, IndividualOrder.Normal, true);
        lineup.Slots.Add(slot);

        lineup.Slots.Remove(slot);

        lineup.Slots.Should().BeEmpty();
    }

    [Fact]
    public void Slots_CanClear()
    {
        var lineup = new TeamLineup();
        lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.Keeper, IndividualOrder.Normal, true));
        lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.Forward, IndividualOrder.Normal, true));

        lineup.Slots.Clear();

        lineup.Slots.Should().BeEmpty();
    }

    [Fact]
    public void Slots_CanBeReplacedWithNewList()
    {
        var lineup = new TeamLineup();
        var newSlots = new List<MatchLineupSlot>
        {
            new(Guid.NewGuid(), Position.Keeper, IndividualOrder.Normal, true),
            new(Guid.NewGuid(), Position.Forward, IndividualOrder.Offensive, true)
        };

        lineup.Slots = newSlots;

        lineup.Slots.Should().HaveCount(2);
        lineup.Slots.Should().BeSameAs(newSlots);
    }

    [Fact]
    public void Slots_CanAddElevenStarters()
    {
        var lineup = new TeamLineup();

        for (int i = 0; i < TeamLineup.StarterCount; i++)
        {
            lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.Forward, IndividualOrder.Normal, true));
        }

        lineup.Slots.Should().HaveCount(TeamLineup.StarterCount);
        lineup.Slots.Should().OnlyContain(s => s.IsStarter,
            "all 11 slots should be starters");
    }

    [Fact]
    public void Slots_CanAddStartersAndSubstitutes()
    {
        var lineup = new TeamLineup();

        // Add 11 starters
        for (int i = 0; i < TeamLineup.StarterCount; i++)
        {
            lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.Forward, IndividualOrder.Normal, true));
        }

        // Add 3 substitutes
        for (int i = 0; i < TeamLineup.MaxSubstituteCount; i++)
        {
            lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.Forward, IndividualOrder.Normal, false));
        }

        lineup.Slots.Should().HaveCount(TeamLineup.MaxTotalSlots);
        lineup.Slots.Count(s => s.IsStarter).Should().Be(11, "should have 11 starters");
        lineup.Slots.Count(s => !s.IsStarter).Should().Be(3, "should have 3 substitutes");
    }

    #endregion

    #region SetPiecesTakerId Property (Guid?)

    [Fact]
    public void SetPiecesTakerId_CanBeSetToSpecificGuid()
    {
        var playerId = Guid.NewGuid();
        var lineup = new TeamLineup { SetPiecesTakerId = playerId };

        lineup.SetPiecesTakerId.Should().Be(playerId);
    }

    [Fact]
    public void SetPiecesTakerId_CanBeSetToNull()
    {
        var lineup = new TeamLineup { SetPiecesTakerId = Guid.NewGuid() };
        lineup.SetPiecesTakerId = null;

        lineup.SetPiecesTakerId.Should().BeNull();
    }

    [Fact]
    public void SetPiecesTakerId_HasValueReturnsTrueWhenSet()
    {
        var lineup = new TeamLineup { SetPiecesTakerId = Guid.NewGuid() };

        lineup.SetPiecesTakerId.HasValue.Should().BeTrue();
    }

    [Fact]
    public void SetPiecesTakerId_HasValueReturnsFalseWhenNull()
    {
        var lineup = new TeamLineup { SetPiecesTakerId = null };

        lineup.SetPiecesTakerId.HasValue.Should().BeFalse();
    }

    #endregion

    #region CaptainId Property (Guid?)

    [Fact]
    public void CaptainId_CanBeSetToSpecificGuid()
    {
        var playerId = Guid.NewGuid();
        var lineup = new TeamLineup { CaptainId = playerId };

        lineup.CaptainId.Should().Be(playerId);
    }

    [Fact]
    public void CaptainId_CanBeSetToNull()
    {
        var lineup = new TeamLineup { CaptainId = Guid.NewGuid() };
        lineup.CaptainId = null;

        lineup.CaptainId.Should().BeNull();
    }

    [Fact]
    public void CaptainId_HasValueReturnsTrueWhenSet()
    {
        var lineup = new TeamLineup { CaptainId = Guid.NewGuid() };

        lineup.CaptainId.HasValue.Should().BeTrue();
    }

    [Fact]
    public void CaptainId_HasValueReturnsFalseWhenNull()
    {
        var lineup = new TeamLineup { CaptainId = null };

        lineup.CaptainId.HasValue.Should().BeFalse();
    }

    #endregion

    #region Mutability

    [Fact]
    public void Properties_WhenMutatedAfterConstruction_ReflectNewValues()
    {
        var lineup = new TeamLineup
        {
            TeamId = Guid.NewGuid(),
            Formation = Formation.Formation442,
            Tactic = Tactic.Normal,
            Attitude = TeamAttitude.Normal
        };

        var newTeamId = Guid.NewGuid();
        var newCaptainId = Guid.NewGuid();
        var newSetPiecesTakerId = Guid.NewGuid();

        lineup.TeamId = newTeamId;
        lineup.Formation = Formation.Formation352;
        lineup.Tactic = Tactic.CounterAttack;
        lineup.Attitude = TeamAttitude.MatchOfTheSeason;
        lineup.CaptainId = newCaptainId;
        lineup.SetPiecesTakerId = newSetPiecesTakerId;

        lineup.TeamId.Should().Be(newTeamId);
        lineup.Formation.Should().Be(Formation.Formation352);
        lineup.Tactic.Should().Be(Tactic.CounterAttack);
        lineup.Attitude.Should().Be(TeamAttitude.MatchOfTheSeason);
        lineup.CaptainId.Should().Be(newCaptainId);
        lineup.SetPiecesTakerId.Should().Be(newSetPiecesTakerId);
    }

    [Fact]
    public void Slots_WhenMutatedAfterConstruction_ReflectsChanges()
    {
        var lineup = new TeamLineup();
        var slot1 = new MatchLineupSlot(Guid.NewGuid(), Position.Keeper, IndividualOrder.Normal, true);

        lineup.Slots.Add(slot1);

        lineup.Slots.Should().HaveCount(1);

        var slot2 = new MatchLineupSlot(Guid.NewGuid(), Position.Forward, IndividualOrder.Offensive, true);
        lineup.Slots.Add(slot2);

        lineup.Slots.Should().HaveCount(2);
        lineup.Slots[1].Should().Be(slot2);
    }

    #endregion

    #region Full Construction (Integration)

    [Fact]
    public void ObjectInitializer_WhenAllPropertiesProvided_AllAreStored()
    {
        var teamId = Guid.NewGuid();
        var captainId = Guid.NewGuid();
        var setPiecesTakerId = Guid.NewGuid();
        var keeperId = Guid.NewGuid();
        var forwardId = Guid.NewGuid();

        var lineup = new TeamLineup
        {
            TeamId = teamId,
            Formation = Formation.Formation433,
            Tactic = Tactic.AttackOnWings,
            Attitude = TeamAttitude.MatchOfTheSeason,
            CaptainId = captainId,
            SetPiecesTakerId = setPiecesTakerId,
            Slots = new List<MatchLineupSlot>
            {
                new(keeperId, Position.Keeper, IndividualOrder.Normal, true),
                new(forwardId, Position.Forward, IndividualOrder.Offensive, true)
            }
        };

        lineup.TeamId.Should().Be(teamId);
        lineup.Formation.Should().Be(Formation.Formation433);
        lineup.Tactic.Should().Be(Tactic.AttackOnWings);
        lineup.Attitude.Should().Be(TeamAttitude.MatchOfTheSeason);
        lineup.CaptainId.Should().Be(captainId);
        lineup.SetPiecesTakerId.Should().Be(setPiecesTakerId);
        lineup.Slots.Should().HaveCount(2);
        lineup.Slots[0].PlayerId.Should().Be(keeperId);
        lineup.Slots[0].Position.Should().Be(Position.Keeper);
        lineup.Slots[1].PlayerId.Should().Be(forwardId);
        lineup.Slots[1].Position.Should().Be(Position.Forward);
    }

    [Fact]
    public void Constructor_MinimalLineup_HasCorrectDefaults()
    {
        var lineup = new TeamLineup
        {
            TeamId = Guid.NewGuid()
        };

        lineup.Formation.Should().Be(Formation.Formation442);
        lineup.Tactic.Should().Be(Tactic.Normal);
        lineup.Attitude.Should().Be(TeamAttitude.PlayItCool);
        lineup.CaptainId.Should().BeNull();
        lineup.SetPiecesTakerId.Should().BeNull();
        lineup.Slots.Should().BeEmpty();
    }

    #endregion

    #region Practical Usage Scenarios

    [Fact]
    public void TypicalMatchLineup_Full442Formation()
    {
        var teamId = Guid.NewGuid();
        var lineup = new TeamLineup
        {
            TeamId = teamId,
            Formation = Formation.Formation442,
            Tactic = Tactic.Normal,
            Attitude = TeamAttitude.Normal
        };

        // Add 11 starters for a 4-4-2 formation
        var keeperId = Guid.NewGuid();
        lineup.Slots.Add(new MatchLineupSlot(keeperId, Position.Keeper, IndividualOrder.Normal, true));

        // 4 defenders
        for (int i = 0; i < 4; i++)
        {
            lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.CentralDefender, IndividualOrder.Normal, true));
        }

        // 4 midfielders
        for (int i = 0; i < 4; i++)
        {
            lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.InnerMidfielder, IndividualOrder.Normal, true));
        }

        // 2 forwards
        for (int i = 0; i < 2; i++)
        {
            lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.Forward, IndividualOrder.Normal, true));
        }

        // Set captain and set pieces taker
        lineup.CaptainId = keeperId;
        lineup.SetPiecesTakerId = lineup.Slots[5].PlayerId; // First midfielder

        lineup.Slots.Should().HaveCount(TeamLineup.StarterCount);
        lineup.CaptainId.Should().Be(keeperId);
        lineup.SetPiecesTakerId.Should().NotBeNull();
    }

    [Fact]
    public void TypicalMatchLineup_WithSubstitutes()
    {
        var lineup = new TeamLineup
        {
            TeamId = Guid.NewGuid(),
            Formation = Formation.Formation442,
            Tactic = Tactic.CounterAttack,
            Attitude = TeamAttitude.Normal
        };

        // Add 11 starters
        for (int i = 0; i < TeamLineup.StarterCount; i++)
        {
            lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.Forward, IndividualOrder.Normal, true));
        }

        // Add 3 substitutes
        lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.Keeper, IndividualOrder.Normal, false));
        lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.CentralDefender, IndividualOrder.Normal, false));
        lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.Forward, IndividualOrder.Normal, false));

        lineup.Slots.Should().HaveCount(TeamLineup.MaxTotalSlots);
        lineup.Slots.Count(s => s.IsStarter).Should().Be(TeamLineup.StarterCount);
        lineup.Slots.Count(s => !s.IsStarter).Should().Be(TeamLineup.MaxSubstituteCount);
    }

    [Fact]
    public void AggressiveTactics_MatchOfTheSeasonWithPressing()
    {
        var lineup = new TeamLineup
        {
            TeamId = Guid.NewGuid(),
            Formation = Formation.Formation433,
            Tactic = Tactic.Pressing,
            Attitude = TeamAttitude.MatchOfTheSeason
        };

        lineup.Formation.Should().Be(Formation.Formation433);
        lineup.Tactic.Should().Be(Tactic.Pressing,
            "pressing is an aggressive tactic");
        lineup.Attitude.Should().Be(TeamAttitude.MatchOfTheSeason,
            "MatchOfTheSeason gives temporary morale boost but risks spirit loss");
    }

    [Fact]
    public void DefensiveTactics_PlayItCoolWithCounterAttack()
    {
        var lineup = new TeamLineup
        {
            TeamId = Guid.NewGuid(),
            Formation = Formation.Formation541,
            Tactic = Tactic.CounterAttack,
            Attitude = TeamAttitude.PlayItCool
        };

        lineup.Formation.Should().Be(Formation.Formation541);
        lineup.Tactic.Should().Be(Tactic.CounterAttack,
            "counter-attack works well with defensive formations");
        lineup.Attitude.Should().Be(TeamAttitude.PlayItCool,
            "PlayItCool preserves team spirit");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Slots_CanHaveZeroSlots()
    {
        var lineup = new TeamLineup
        {
            TeamId = Guid.NewGuid(),
            Formation = Formation.Formation442
        };

        lineup.Slots.Should().BeEmpty(
            "model should allow empty slots; validation belongs in service layer");
    }

    [Fact]
    public void SetPiecesTakerAndCaptain_CanBeSamePlayer()
    {
        var playerId = Guid.NewGuid();
        var lineup = new TeamLineup
        {
            CaptainId = playerId,
            SetPiecesTakerId = playerId
        };

        lineup.CaptainId.Should().Be(playerId);
        lineup.SetPiecesTakerId.Should().Be(playerId);
        lineup.CaptainId.Should().Be(lineup.SetPiecesTakerId,
            "same player can be both captain and set pieces taker");
    }

    [Fact]
    public void SetPiecesTakerAndCaptain_CanBeDifferentPlayers()
    {
        var captainId = Guid.NewGuid();
        var setTakerId = Guid.NewGuid();
        var lineup = new TeamLineup
        {
            CaptainId = captainId,
            SetPiecesTakerId = setTakerId
        };

        lineup.CaptainId.Value.Should().NotBe(lineup.SetPiecesTakerId!.Value,
            "captain and set pieces taker are typically different players");
    }

    [Fact]
    public void Slots_ModelAcceptsMoreThanMaxTotalSlots()
    {
        var lineup = new TeamLineup();

        // Add more than 14 slots - model should accept it (validation is in service)
        for (int i = 0; i < 20; i++)
        {
            lineup.Slots.Add(new MatchLineupSlot(Guid.NewGuid(), Position.Forward, IndividualOrder.Normal, true));
        }

        lineup.Slots.Should().HaveCount(20,
            "model should accept any number of slots; validation belongs in service layer");
    }

    [Fact]
    public void Slots_ModelAcceptsDuplicatePlayerIds()
    {
        var lineup = new TeamLineup();
        var playerId = Guid.NewGuid();

        // Add same player twice - model should accept it (validation is in service)
        lineup.Slots.Add(new MatchLineupSlot(playerId, Position.Forward, IndividualOrder.Normal, true));
        lineup.Slots.Add(new MatchLineupSlot(playerId, Position.Keeper, IndividualOrder.Normal, true));

        lineup.Slots.Should().HaveCount(2,
            "model should accept duplicate player IDs; validation belongs in service layer");
    }

    #endregion
}
