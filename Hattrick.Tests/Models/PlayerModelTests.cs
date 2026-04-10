using Hattrick.Core.Models;

namespace Hattrick.Tests.Models;

/// <summary>
/// Tests for the Player model class.
/// TDD Red Phase: Player class does not exist yet — these tests define the expected API.
/// </summary>
public class PlayerModelTests
{
    #region Construction & Defaults

    [Fact]
    public void Constructor_Default_CreatesPlayerWithNonEmptyId()
    {
        var player = new Player();

        player.Id.Should().NotBe(Guid.Empty,
            "a new Player should have a non-empty Guid by default");
    }

    [Fact]
    public void Constructor_Default_TeamIdIsEmptyGuid()
    {
        var player = new Player();

        player.TeamId.Should().Be(Guid.Empty,
            "TeamId should default to Guid.Empty until assigned to a team");
    }

    [Fact]
    public void Constructor_Default_NameIsEmpty()
    {
        var player = new Player();

        player.Name.Should().BeEmpty(
            "Name should default to empty string, not null");
    }

    [Fact]
    public void Constructor_Default_SkillsDictionaryIsInitialized()
    {
        var player = new Player();

        player.Skills.Should().NotBeNull(
            "Skills dictionary must be initialized to avoid NullReferenceException");
        player.Skills.Should().BeEmpty(
            "Skills dictionary should start empty until skills are assigned");
    }

    [Fact]
    public void Constructor_Default_SpecialtyIsNone()
    {
        var player = new Player();

        player.Specialty.Should().Be(Specialty.None,
            "default Specialty should be None");
    }

    [Fact]
    public void Constructor_Default_BooleanPropertiesAreFalse()
    {
        var player = new Player();

        player.IsMotherClub.Should().BeFalse("IsMotherClub should default to false");
        player.RedCard.Should().BeFalse("RedCard should default to false");
    }

    [Fact]
    public void Constructor_Default_NumericPropertiesAreZero()
    {
        var player = new Player();

        player.Age.Should().Be(0);
        player.AgeDays.Should().Be(0);
        player.TSI.Should().Be(0);
        player.InjuryWeeks.Should().Be(0);
        player.YellowCards.Should().Be(0);
        player.JerseyNumber.Should().Be(0);
        player.HTMS.Should().Be(0);
        player.Potential.Should().Be(0);
        player.NativeCountryId.Should().Be(0);
    }

    [Fact]
    public void Constructor_Default_BoundedPropertiesDefaultToMinimum()
    {
        var player = new Player();

        player.Form.Should().Be(1, "Form minimum is 1");
        player.Stamina.Should().Be(1, "Stamina minimum is 1");
        player.Experience.Should().Be(1, "Experience minimum is 1");
        player.Leadership.Should().Be(1, "Leadership minimum is 1");
    }

    [Fact]
    public void Constructor_Default_WageIsZero()
    {
        var player = new Player();

        player.Wage.Should().Be(0m,
            "Wage should default to zero decimal");
    }

    [Fact]
    public void Constructor_Default_LoyaltyIsZero()
    {
        var player = new Player();

        player.Loyalty.Should().Be(0.0,
            "Loyalty should default to 0.0");
    }

    #endregion

    #region Id Property

    [Fact]
    public void Id_CanBeSetToSpecificGuid()
    {
        var specificId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var player = new Player { Id = specificId };

        player.Id.Should().Be(specificId);
    }

    [Fact]
    public void Id_TwoNewPlayers_HaveDifferentIds()
    {
        var player1 = new Player();
        var player2 = new Player();

        player1.Id.Should().NotBe(player2.Id,
            "each new Player should get a unique Guid");
    }

    #endregion

    #region Name & Identity Properties

    [Fact]
    public void Name_CanBeSetAndRetrieved()
    {
        var player = new Player { Name = "Viktor Larsson" };

        player.Name.Should().Be("Viktor Larsson");
    }

    [Fact]
    public void TeamId_CanBeSetAndRetrieved()
    {
        var teamId = Guid.NewGuid();
        var player = new Player { TeamId = teamId };

        player.TeamId.Should().Be(teamId);
    }

    [Fact]
    public void NativeCountryId_CanBeSetAndRetrieved()
    {
        var player = new Player { NativeCountryId = 46 };

        player.NativeCountryId.Should().Be(46);
    }

    [Fact]
    public void JerseyNumber_CanBeSetAndRetrieved()
    {
        var player = new Player { JerseyNumber = 10 };

        player.JerseyNumber.Should().Be(10);
    }

    #endregion

    #region Age Properties

    [Fact]
    public void Age_CanBeSetToTypicalPlayerAge()
    {
        var player = new Player { Age = 25 };

        player.Age.Should().Be(25);
    }

    [Fact]
    public void Age_MinimumReasonableValue_Is17()
    {
        var player = new Player { Age = 17 };

        player.Age.Should().Be(17,
            "17 is the minimum player age in Hattrick");
    }

    [Fact]
    public void AgeDays_CanBeSetToValidValue()
    {
        var player = new Player { AgeDays = 100 };

        player.AgeDays.Should().Be(100);
    }

    [Fact]
    public void AgeDays_BoundaryMinimum_Zero()
    {
        var player = new Player { AgeDays = 0 };

        player.AgeDays.Should().Be(0);
    }

    [Fact]
    public void AgeDays_BoundaryMaximum_111()
    {
        // Hattrick uses 112-day seasons (16 weeks * 7 days)
        var player = new Player { AgeDays = 111 };

        player.AgeDays.Should().Be(111);
    }

    #endregion

    #region Skills Dictionary

    [Fact]
    public void Skills_CanAddSkillWithSubLevel()
    {
        var player = new Player();
        player.Skills[SkillType.Scoring] = 7.73;

        player.Skills[SkillType.Scoring].Should().Be(7.73,
            "Skills should support sub-level precision like 7.73");
    }

    [Fact]
    public void Skills_CanSetMultipleSkills()
    {
        var player = new Player();
        player.Skills[SkillType.Keeper] = 3.0;
        player.Skills[SkillType.Defending] = 12.45;
        player.Skills[SkillType.Playmaking] = 8.99;
        player.Skills[SkillType.Scoring] = 5.01;

        player.Skills.Should().HaveCount(4);
        player.Skills[SkillType.Keeper].Should().Be(3.0);
        player.Skills[SkillType.Defending].Should().Be(12.45);
        player.Skills[SkillType.Playmaking].Should().Be(8.99);
        player.Skills[SkillType.Scoring].Should().Be(5.01);
    }

    [Fact]
    public void Skills_CanUpdateExistingSkill()
    {
        var player = new Player();
        player.Skills[SkillType.Passing] = 6.50;
        player.Skills[SkillType.Passing] = 6.73;

        player.Skills[SkillType.Passing].Should().Be(6.73,
            "updating a skill should overwrite the previous value");
    }

    [Fact]
    public void Skills_CanSetAllEightSkillTypes()
    {
        var player = new Player();
        foreach (var skillType in Enum.GetValues(typeof(SkillType)).Cast<SkillType>())
        {
            player.Skills[skillType] = 5.0 + (int)skillType * 0.5;
        }

        var expectedCount = Enum.GetValues<SkillType>().Length;
        player.Skills.Should().HaveCount(expectedCount,
            "a player should be able to have all skill types");
        player.Skills.Keys.Should().BeEquivalentTo(Enum.GetValues<SkillType>());
    }

    [Fact]
    public void Skills_SubLevelZero_IsValid()
    {
        var player = new Player();
        player.Skills[SkillType.Keeper] = 0.0;

        player.Skills[SkillType.Keeper].Should().Be(0.0);
    }

    [Fact]
    public void Skills_SubLevelMaximum_IsValid()
    {
        // SkillLevel goes up to 20 (Divine), sub-level can approach but not reach 21
        var player = new Player();
        player.Skills[SkillType.Scoring] = 20.99;

        player.Skills[SkillType.Scoring].Should().Be(20.99);
    }

    [Fact]
    public void Skills_CanInitializeWithDictionaryInitializer()
    {
        var player = new Player
        {
            Skills = new Dictionary<SkillType, double>
            {
                { SkillType.Defending, 10.5 },
                { SkillType.Scoring, 14.25 }
            }
        };

        player.Skills.Should().HaveCount(2);
        player.Skills[SkillType.Defending].Should().Be(10.5);
        player.Skills[SkillType.Scoring].Should().Be(14.25);
    }

    #endregion

    #region Enum-Typed Properties

    [Theory]
    [InlineData(Specialty.None)]
    [InlineData(Specialty.Technical)]
    [InlineData(Specialty.Quick)]
    [InlineData(Specialty.Head)]
    [InlineData(Specialty.Powerful)]
    [InlineData(Specialty.Unpredictable)]
    [InlineData(Specialty.Resilient)]
    [InlineData(Specialty.Support)]
    public void Specialty_AcceptsAllValidValues(Specialty specialty)
    {
        var player = new Player { Specialty = specialty };

        player.Specialty.Should().Be(specialty);
    }

    [Theory]
    [InlineData(PlayerPersonality.Nice)]
    [InlineData(PlayerPersonality.Nasty)]
    [InlineData(PlayerPersonality.Leader)]
    [InlineData(PlayerPersonality.Loner)]
    [InlineData(PlayerPersonality.Temperamental)]
    [InlineData(PlayerPersonality.Calm)]
    public void Personality_AcceptsAllValidValues(PlayerPersonality personality)
    {
        var player = new Player { Personality = personality };

        player.Personality.Should().Be(personality);
    }

    [Theory]
    [InlineData(Position.Keeper)]
    [InlineData(Position.CentralDefender)]
    [InlineData(Position.WingBack)]
    [InlineData(Position.InnerMidfielder)]
    [InlineData(Position.Winger)]
    [InlineData(Position.Forward)]
    public void BestPosition_AcceptsAllValidValues(Position position)
    {
        var player = new Player { BestPosition = position };

        player.BestPosition.Should().Be(position);
    }

    #endregion

    #region Form (1-8)

    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(8)]
    public void Form_AcceptsValidValues(int form)
    {
        var player = new Player { Form = form };

        player.Form.Should().Be(form);
    }

    [Fact]
    public void Form_BoundaryMinimum_IsOne()
    {
        var player = new Player { Form = 1 };

        player.Form.Should().Be(1, "minimum Form is 1");
    }

    [Fact]
    public void Form_BoundaryMaximum_IsEight()
    {
        var player = new Player { Form = 8 };

        player.Form.Should().Be(8, "maximum Form is 8");
    }

    #endregion

    #region Stamina (1-9)

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(9)]
    public void Stamina_AcceptsValidValues(int stamina)
    {
        var player = new Player { Stamina = stamina };

        player.Stamina.Should().Be(stamina);
    }

    [Fact]
    public void Stamina_BoundaryMinimum_IsOne()
    {
        var player = new Player { Stamina = 1 };

        player.Stamina.Should().Be(1, "minimum Stamina is 1");
    }

    [Fact]
    public void Stamina_BoundaryMaximum_IsNine()
    {
        var player = new Player { Stamina = 9 };

        player.Stamina.Should().Be(9, "maximum Stamina is 9");
    }

    #endregion

    #region Experience (1-20)

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(20)]
    public void Experience_AcceptsValidValues(int experience)
    {
        var player = new Player { Experience = experience };

        player.Experience.Should().Be(experience);
    }

    [Fact]
    public void Experience_BoundaryMinimum_IsOne()
    {
        var player = new Player { Experience = 1 };

        player.Experience.Should().Be(1, "minimum Experience is 1");
    }

    [Fact]
    public void Experience_BoundaryMaximum_IsTwenty()
    {
        var player = new Player { Experience = 20 };

        player.Experience.Should().Be(20, "maximum Experience is 20");
    }

    #endregion

    #region Loyalty (0.0-1.0)

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    public void Loyalty_AcceptsValidValues(double loyalty)
    {
        var player = new Player { Loyalty = loyalty };

        player.Loyalty.Should().Be(loyalty);
    }

    [Fact]
    public void Loyalty_BoundaryMinimum_IsZero()
    {
        var player = new Player { Loyalty = 0.0 };

        player.Loyalty.Should().Be(0.0, "minimum Loyalty is 0.0");
    }

    [Fact]
    public void Loyalty_BoundaryMaximum_IsOne()
    {
        var player = new Player { Loyalty = 1.0 };

        player.Loyalty.Should().Be(1.0, "maximum Loyalty is 1.0");
    }

    [Fact]
    public void Loyalty_SupportsFractionalValues()
    {
        var player = new Player { Loyalty = 0.7345 };

        player.Loyalty.Should().Be(0.7345);
    }

    #endregion

    #region Leadership (1-8)

    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(8)]
    public void Leadership_AcceptsValidValues(int leadership)
    {
        var player = new Player { Leadership = leadership };

        player.Leadership.Should().Be(leadership);
    }

    [Fact]
    public void Leadership_BoundaryMinimum_IsOne()
    {
        var player = new Player { Leadership = 1 };

        player.Leadership.Should().Be(1, "minimum Leadership is 1");
    }

    [Fact]
    public void Leadership_BoundaryMaximum_IsEight()
    {
        var player = new Player { Leadership = 8 };

        player.Leadership.Should().Be(8, "maximum Leadership is 8");
    }

    #endregion

    #region YellowCards (0-3)

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void YellowCards_AcceptsValidValues(int yellowCards)
    {
        var player = new Player { YellowCards = yellowCards };

        player.YellowCards.Should().Be(yellowCards);
    }

    [Fact]
    public void YellowCards_BoundaryMinimum_IsZero()
    {
        var player = new Player { YellowCards = 0 };

        player.YellowCards.Should().Be(0, "minimum YellowCards is 0");
    }

    [Fact]
    public void YellowCards_BoundaryMaximum_IsThree()
    {
        var player = new Player { YellowCards = 3 };

        player.YellowCards.Should().Be(3, "cumulative suspension at 3 yellow cards in Hattrick");
    }

    #endregion

    #region InjuryWeeks (0+)

    [Fact]
    public void InjuryWeeks_ZeroMeansHealthy()
    {
        var player = new Player { InjuryWeeks = 0 };

        player.InjuryWeeks.Should().Be(0, "0 means the player is healthy");
    }

    [Fact]
    public void InjuryWeeks_CanBeSetToPositiveValue()
    {
        var player = new Player { InjuryWeeks = 5 };

        player.InjuryWeeks.Should().Be(5);
    }

    [Fact]
    public void InjuryWeeks_CanBeSetToLargeValue()
    {
        // Long-term injuries can last many weeks
        var player = new Player { InjuryWeeks = 52 };

        player.InjuryWeeks.Should().Be(52);
    }

    #endregion

    #region RedCard

    [Fact]
    public void RedCard_CanBeSetToTrue()
    {
        var player = new Player { RedCard = true };

        player.RedCard.Should().BeTrue();
    }

    [Fact]
    public void RedCard_CanBeSetToFalse()
    {
        var player = new Player { RedCard = false };

        player.RedCard.Should().BeFalse();
    }

    #endregion

    #region IsMotherClub

    [Fact]
    public void IsMotherClub_CanBeSetToTrue()
    {
        var player = new Player { IsMotherClub = true };

        player.IsMotherClub.Should().BeTrue();
    }

    #endregion

    #region Wage

    [Fact]
    public void Wage_IsDecimalType_CanHoldPreciseValues()
    {
        var player = new Player { Wage = 12500.50m };

        player.Wage.Should().Be(12500.50m);
    }

    [Fact]
    public void Wage_CanBeSetToLargeValue()
    {
        var player = new Player { Wage = 999999.99m };

        player.Wage.Should().Be(999999.99m);
    }

    [Fact]
    public void Wage_CanBeSetToZero()
    {
        var player = new Player { Wage = 0m };

        player.Wage.Should().Be(0m);
    }

    #endregion

    #region TSI, HTMS, Potential

    [Fact]
    public void TSI_CanBeSetAndRetrieved()
    {
        var player = new Player { TSI = 5000 };

        player.TSI.Should().Be(5000);
    }

    [Fact]
    public void TSI_CanBeLargeValue()
    {
        var player = new Player { TSI = 1_000_000 };

        player.TSI.Should().Be(1_000_000,
            "top players can have very high TSI values");
    }

    [Fact]
    public void HTMS_CanBeSetAndRetrieved()
    {
        var player = new Player { HTMS = 450 };

        player.HTMS.Should().Be(450);
    }

    [Fact]
    public void Potential_CanBeSetAndRetrieved()
    {
        var player = new Player { Potential = 85 };

        player.Potential.Should().Be(85);
    }

    #endregion

    #region Full Player Construction (Integration)

    [Fact]
    public void FullPlayer_AllPropertiesCanBeSetViaObjectInitializer()
    {
        var playerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();

        var player = new Player
        {
            Id = playerId,
            TeamId = teamId,
            Name = "Anders Eriksson",
            Age = 24,
            AgeDays = 55,
            Skills = new Dictionary<SkillType, double>
            {
                { SkillType.Keeper, 2.0 },
                { SkillType.Defending, 5.5 },
                { SkillType.Playmaking, 10.73 },
                { SkillType.Winger, 4.0 },
                { SkillType.Passing, 8.25 },
                { SkillType.Scoring, 6.0 },
                { SkillType.SetPieces, 3.5 },
                { SkillType.Stamina, 7.0 }
            },
            Specialty = Specialty.Technical,
            Form = 7,
            Stamina = 8,
            Experience = 12,
            Loyalty = 0.85,
            IsMotherClub = false,
            TSI = 15000,
            InjuryWeeks = 0,
            RedCard = false,
            YellowCards = 1,
            JerseyNumber = 8,
            Wage = 7500.00m,
            Personality = PlayerPersonality.Calm,
            Leadership = 5,
            HTMS = 320,
            Potential = 70,
            BestPosition = Position.InnerMidfielder,
            NativeCountryId = 46
        };

        player.Id.Should().Be(playerId);
        player.TeamId.Should().Be(teamId);
        player.Name.Should().Be("Anders Eriksson");
        player.Age.Should().Be(24);
        player.AgeDays.Should().Be(55);
        player.Skills.Should().HaveCount(8);
        player.Skills[SkillType.Playmaking].Should().Be(10.73);
        player.Skills[SkillType.Passing].Should().Be(8.25);
        player.Specialty.Should().Be(Specialty.Technical);
        player.Form.Should().Be(7);
        player.Stamina.Should().Be(8);
        player.Experience.Should().Be(12);
        player.Loyalty.Should().Be(0.85);
        player.IsMotherClub.Should().BeFalse();
        player.TSI.Should().Be(15000);
        player.InjuryWeeks.Should().Be(0);
        player.RedCard.Should().BeFalse();
        player.YellowCards.Should().Be(1);
        player.JerseyNumber.Should().Be(8);
        player.Wage.Should().Be(7500.00m);
        player.Personality.Should().Be(PlayerPersonality.Calm);
        player.Leadership.Should().Be(5);
        player.HTMS.Should().Be(320);
        player.Potential.Should().Be(70);
        player.BestPosition.Should().Be(Position.InnerMidfielder);
        player.NativeCountryId.Should().Be(46);
    }

    [Fact]
    public void Player_IsMutable_PropertiesCanBeChangedAfterConstruction()
    {
        var player = new Player
        {
            Name = "Old Name",
            Form = 3,
            Experience = 5
        };

        player.Name = "New Name";
        player.Form = 7;
        player.Experience = 10;

        player.Name.Should().Be("New Name");
        player.Form.Should().Be(7);
        player.Experience.Should().Be(10);
    }

    #endregion
}
