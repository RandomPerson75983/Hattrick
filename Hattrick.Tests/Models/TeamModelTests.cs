using Hattrick.Core.Models;

namespace Hattrick.Tests.Models;

/// <summary>
/// Tests for the Team model class.
/// </summary>
public class TeamModelTests
{
    #region Construction & Defaults

    [Fact]
    public void Constructor_Default_CreatesTeamWithNonEmptyId()
    {
        var team = new Team();

        team.Id.Should().NotBe(Guid.Empty,
            "a new Team should have a non-empty Guid by default");
    }

    [Fact]
    public void Constructor_Default_NameIsEmpty()
    {
        var team = new Team();

        team.Name.Should().BeEmpty(
            "Name should default to empty string, not null");
    }

    [Fact]
    public void Constructor_Default_IsHumanControlledIsFalse()
    {
        var team = new Team();

        team.IsHumanControlled.Should().BeFalse(
            "a team should default to AI-controlled");
    }

    [Fact]
    public void Constructor_Default_BudgetIsZero()
    {
        var team = new Team();

        team.Budget.Should().Be(0m,
            "Budget should default to zero decimal");
    }

    [Fact]
    public void Constructor_Default_FansIsZero()
    {
        var team = new Team();

        team.Fans.Should().Be(0,
            "Fans should default to zero");
    }

    [Fact]
    public void Constructor_Default_FanClubSizeIsZero()
    {
        var team = new Team();

        team.FanClubSize.Should().Be(0,
            "FanClubSize should default to zero");
    }

    [Fact]
    public void Constructor_Default_TeamSpiritIsZero()
    {
        var team = new Team();

        team.TeamSpirit.Should().Be(0.0,
            "TeamSpirit should default to 0.0");
    }

    [Fact]
    public void Constructor_Default_ConfidenceIsZero()
    {
        var team = new Team();

        team.Confidence.Should().Be(0.0,
            "Confidence should default to 0.0");
    }

    [Fact]
    public void Constructor_Default_CoachTypeIsOffensive()
    {
        var team = new Team();

        team.CoachType.Should().Be(CoachType.Offensive,
            "CoachType.Offensive is the zero-value (0) enum member and expected default");
    }

    [Fact]
    public void Constructor_Default_CoachLevelIsOne()
    {
        var team = new Team();

        team.CoachLevel.Should().Be(Team.MinCoachLevel,
            "CoachLevel minimum is 1; every team starts with a coach");
    }

    [Fact]
    public void Constructor_Default_StaffLevelsAreZero()
    {
        var team = new Team();

        team.AssistantCoachLevel.Should().Be(0, "AssistantCoachLevel default is 0 (none hired)");
        team.DoctorLevel.Should().Be(0, "DoctorLevel default is 0 (none hired)");
        team.SpokespersonLevel.Should().Be(0, "SpokespersonLevel default is 0 (none hired)");
        team.FinancialDirectorLevel.Should().Be(0, "FinancialDirectorLevel default is 0 (none hired)");
    }

    #endregion

    #region Id Property

    [Fact]
    public void Id_CanBeSetToSpecificGuid()
    {
        var specificId = Guid.Parse("aaaabbbb-cccc-dddd-eeee-ffffffffffff");
        var team = new Team { Id = specificId };

        team.Id.Should().Be(specificId);
    }

    [Fact]
    public void Id_TwoNewTeams_HaveDifferentIds()
    {
        var team1 = new Team();
        var team2 = new Team();

        team1.Id.Should().NotBe(team2.Id,
            "each new Team should get a unique Guid");
    }

    #endregion

    #region Name Property

    [Fact]
    public void Name_CanBeSetAndRetrieved()
    {
        var team = new Team { Name = "Ironvale United" };

        team.Name.Should().Be("Ironvale United");
    }

    [Fact]
    public void Name_CanBeUpdatedAfterConstruction()
    {
        var team = new Team { Name = "Old Name" };
        team.Name = "New Name";

        team.Name.Should().Be("New Name");
    }

    #endregion

    #region IsHumanControlled

    [Fact]
    public void IsHumanControlled_CanBeSetToTrue()
    {
        var team = new Team { IsHumanControlled = true };

        team.IsHumanControlled.Should().BeTrue();
    }

    [Fact]
    public void IsHumanControlled_CanBeSetToFalse()
    {
        var team = new Team { IsHumanControlled = false };

        team.IsHumanControlled.Should().BeFalse();
    }

    #endregion

    #region Budget (decimal)

    [Fact]
    public void Budget_CanBeSetAndRetrieved()
    {
        var team = new Team { Budget = 500_000.00m };

        team.Budget.Should().Be(500_000.00m);
    }

    [Fact]
    public void Budget_SupportsDecimalPrecision()
    {
        var team = new Team { Budget = 123456.78m };

        team.Budget.Should().Be(123456.78m,
            "Budget is decimal and must retain fractional currency values");
    }

    [Fact]
    public void Budget_CanBeSetToLargeValue()
    {
        var team = new Team { Budget = 10_000_000.00m };

        team.Budget.Should().Be(10_000_000.00m,
            "a successful team can accumulate large budgets");
    }

    [Fact]
    public void Budget_CanBeSetToZero()
    {
        var team = new Team { Budget = 0m };

        team.Budget.Should().Be(0m);
    }

    #endregion

    #region Fans & FanClubSize

    [Fact]
    public void Fans_CanBeSetAndRetrieved()
    {
        var team = new Team { Fans = 25_000 };

        team.Fans.Should().Be(25_000);
    }

    [Fact]
    public void Fans_CanBeSetToLargeValue()
    {
        var team = new Team { Fans = 500_000 };

        team.Fans.Should().Be(500_000,
            "top clubs can have very large fan bases");
    }

    [Fact]
    public void FanClubSize_CanBeSetAndRetrieved()
    {
        var team = new Team { FanClubSize = 1_200 };

        team.FanClubSize.Should().Be(1_200);
    }

    [Fact]
    public void FanClubSize_CanBeSetToLargeValue()
    {
        var team = new Team { FanClubSize = 100_000 };

        team.FanClubSize.Should().Be(100_000);
    }

    [Fact]
    public void Fans_CanBeSetToZero()
    {
        var team = new Team { Fans = 25_000 };
        team.Fans = 0;

        team.Fans.Should().Be(0, "Fans can drop to zero (e.g., disbanded or reset scenario)");
    }

    [Fact]
    public void FanClubSize_CanBeSetToZero()
    {
        var team = new Team { FanClubSize = 1_200 };
        team.FanClubSize = 0;

        team.FanClubSize.Should().Be(0, "FanClubSize can drop to zero (e.g., disbanded or reset scenario)");
    }

    #endregion

    #region TeamSpirit (0.0-10.0)

    [Theory]
    [InlineData(0.0)]
    [InlineData(5.0)]
    [InlineData(10.0)]
    public void TeamSpirit_AcceptsValidValues(double spirit)
    {
        var team = new Team { TeamSpirit = spirit };

        team.TeamSpirit.Should().Be(spirit);
    }

    [Fact]
    public void TeamSpirit_BoundaryMinimum_IsZero()
    {
        var team = new Team { TeamSpirit = 0.0 };

        team.TeamSpirit.Should().Be(0.0, "minimum TeamSpirit is 0.0");
    }

    [Fact]
    public void TeamSpirit_BoundaryMaximum_IsTen()
    {
        var team = new Team { TeamSpirit = 10.0 };

        team.TeamSpirit.Should().Be(10.0, "maximum TeamSpirit is 10.0");
    }

    [Fact]
    public void TeamSpirit_SupportsFractionalValues()
    {
        var team = new Team { TeamSpirit = 7.35 };

        team.TeamSpirit.Should().Be(7.35,
            "TeamSpirit is a double and must retain sub-integer precision");
    }

    #endregion

    #region Confidence (0.0-10.0)

    [Theory]
    [InlineData(0.0)]
    [InlineData(5.0)]
    [InlineData(10.0)]
    public void Confidence_AcceptsValidValues(double confidence)
    {
        var team = new Team { Confidence = confidence };

        team.Confidence.Should().Be(confidence);
    }

    [Fact]
    public void Confidence_BoundaryMinimum_IsZero()
    {
        var team = new Team { Confidence = 0.0 };

        team.Confidence.Should().Be(0.0, "minimum Confidence is 0.0");
    }

    [Fact]
    public void Confidence_BoundaryMaximum_IsTen()
    {
        var team = new Team { Confidence = 10.0 };

        team.Confidence.Should().Be(10.0, "maximum Confidence is 10.0");
    }

    [Fact]
    public void Confidence_SupportsFractionalValues()
    {
        var team = new Team { Confidence = 4.82 };

        team.Confidence.Should().Be(4.82,
            "Confidence is a double and must retain sub-integer precision");
    }

    #endregion

    #region CoachType Enum

    [Theory]
    [InlineData(CoachType.Offensive)]
    [InlineData(CoachType.Defensive)]
    [InlineData(CoachType.Balanced)]
    public void CoachType_AcceptsAllValidValues(CoachType coachType)
    {
        var team = new Team { CoachType = coachType };

        team.CoachType.Should().Be(coachType);
    }

    #endregion

    #region CoachLevel (1-8)

    [Theory]
    [InlineData(Team.MinCoachLevel)]
    [InlineData(4)]
    [InlineData(Team.MaxCoachLevel)]
    public void CoachLevel_AcceptsValidValues(int level)
    {
        var team = new Team { CoachLevel = level };

        team.CoachLevel.Should().Be(level);
    }

    [Fact]
    public void CoachLevel_BoundaryMinimum_IsOne()
    {
        var team = new Team { CoachLevel = Team.MinCoachLevel };

        team.CoachLevel.Should().Be(Team.MinCoachLevel, "minimum CoachLevel is 1");
    }

    [Fact]
    public void CoachLevel_BoundaryMaximum_IsEight()
    {
        var team = new Team { CoachLevel = Team.MaxCoachLevel };

        team.CoachLevel.Should().Be(Team.MaxCoachLevel, "maximum CoachLevel is 8");
    }

    #endregion

    #region AssistantCoachLevel (0-10)

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(Team.MaxAssistantCoachLevel)]
    public void AssistantCoachLevel_AcceptsValidValues(int level)
    {
        var team = new Team { AssistantCoachLevel = level };

        team.AssistantCoachLevel.Should().Be(level);
    }

    [Fact]
    public void AssistantCoachLevel_BoundaryMinimum_IsZero()
    {
        var team = new Team { AssistantCoachLevel = 0 };

        team.AssistantCoachLevel.Should().Be(0, "minimum AssistantCoachLevel is 0 (none hired)");
    }

    [Fact]
    public void AssistantCoachLevel_BoundaryMaximum_IsTen()
    {
        var team = new Team { AssistantCoachLevel = Team.MaxAssistantCoachLevel };

        team.AssistantCoachLevel.Should().Be(Team.MaxAssistantCoachLevel, "maximum AssistantCoachLevel is 10");
    }

    #endregion

    #region DoctorLevel (0-5)

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(Team.MaxStaffLevel)]
    public void DoctorLevel_AcceptsValidValues(int level)
    {
        var team = new Team { DoctorLevel = level };

        team.DoctorLevel.Should().Be(level);
    }

    [Fact]
    public void DoctorLevel_BoundaryMinimum_IsZero()
    {
        var team = new Team { DoctorLevel = 0 };

        team.DoctorLevel.Should().Be(0, "minimum DoctorLevel is 0 (none hired)");
    }

    [Fact]
    public void DoctorLevel_BoundaryMaximum_IsFive()
    {
        var team = new Team { DoctorLevel = Team.MaxStaffLevel };

        team.DoctorLevel.Should().Be(Team.MaxStaffLevel, "maximum DoctorLevel is 5");
    }

    #endregion

    #region SpokespersonLevel (0-5)

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(Team.MaxStaffLevel)]
    public void SpokespersonLevel_AcceptsValidValues(int level)
    {
        var team = new Team { SpokespersonLevel = level };

        team.SpokespersonLevel.Should().Be(level);
    }

    [Fact]
    public void SpokespersonLevel_BoundaryMinimum_IsZero()
    {
        var team = new Team { SpokespersonLevel = 0 };

        team.SpokespersonLevel.Should().Be(0, "minimum SpokespersonLevel is 0 (none hired)");
    }

    [Fact]
    public void SpokespersonLevel_BoundaryMaximum_IsFive()
    {
        var team = new Team { SpokespersonLevel = Team.MaxStaffLevel };

        team.SpokespersonLevel.Should().Be(Team.MaxStaffLevel, "maximum SpokespersonLevel is 5");
    }

    #endregion

    #region FinancialDirectorLevel (0-5)

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(Team.MaxStaffLevel)]
    public void FinancialDirectorLevel_AcceptsValidValues(int level)
    {
        var team = new Team { FinancialDirectorLevel = level };

        team.FinancialDirectorLevel.Should().Be(level);
    }

    [Fact]
    public void FinancialDirectorLevel_BoundaryMinimum_IsZero()
    {
        var team = new Team { FinancialDirectorLevel = 0 };

        team.FinancialDirectorLevel.Should().Be(0, "minimum FinancialDirectorLevel is 0 (none hired)");
    }

    [Fact]
    public void FinancialDirectorLevel_BoundaryMaximum_IsFive()
    {
        var team = new Team { FinancialDirectorLevel = Team.MaxStaffLevel };

        team.FinancialDirectorLevel.Should().Be(Team.MaxStaffLevel, "maximum FinancialDirectorLevel is 5");
    }

    #endregion

    #region Mutability

    [Fact]
    public void Properties_WhenMutatedAfterConstruction_ReflectNewValues()
    {
        var team = new Team
        {
            Name = "Ironvale United",
            Budget = 100_000m,
            TeamSpirit = 5.0,
            Confidence = 5.0,
            CoachType = CoachType.Balanced,
            CoachLevel = 4
        };

        team.Name = "Ironvale City";
        team.Budget = 250_000m;
        team.TeamSpirit = 8.5;
        team.Confidence = 7.2;
        team.CoachType = CoachType.Defensive;
        team.CoachLevel = 6;

        team.Name.Should().Be("Ironvale City");
        team.Budget.Should().Be(250_000m);
        team.TeamSpirit.Should().Be(8.5);
        team.Confidence.Should().Be(7.2);
        team.CoachType.Should().Be(CoachType.Defensive);
        team.CoachLevel.Should().Be(6);
    }

    #endregion

    #region Full Team Construction (Integration)

    [Fact]
    public void ObjectInitializer_WhenAllPropertiesProvided_AllAreStored()
    {
        var teamId = Guid.NewGuid();

        var team = new Team
        {
            Id = teamId,
            Name = "Ravenbrook Athletic",
            IsHumanControlled = true,
            Budget = 750_000.00m,
            Fans = 18_500,
            FanClubSize = 2_300,
            TeamSpirit = 6.5,
            Confidence = 7.0,
            CoachType = CoachType.Balanced,
            CoachLevel = 5,
            AssistantCoachLevel = 3,
            DoctorLevel = 2,
            SpokespersonLevel = 1,
            FinancialDirectorLevel = 4
        };

        team.Id.Should().Be(teamId);
        team.Name.Should().Be("Ravenbrook Athletic");
        team.IsHumanControlled.Should().BeTrue();
        team.Budget.Should().Be(750_000.00m);
        team.Fans.Should().Be(18_500);
        team.FanClubSize.Should().Be(2_300);
        team.TeamSpirit.Should().Be(6.5);
        team.Confidence.Should().Be(7.0);
        team.CoachType.Should().Be(CoachType.Balanced);
        team.CoachLevel.Should().Be(5);
        team.AssistantCoachLevel.Should().Be(3);
        team.DoctorLevel.Should().Be(2);
        team.SpokespersonLevel.Should().Be(1);
        team.FinancialDirectorLevel.Should().Be(4);
    }

    [Fact]
    public void Constructor_WhenMinimalAiTeam_HasCorrectDefaults()
    {
        // An AI-controlled team with no staff hired other than mandatory coach
        var team = new Team
        {
            Name = "Coldwater FC",
            IsHumanControlled = false,
            CoachLevel = Team.MinCoachLevel
        };

        team.IsHumanControlled.Should().BeFalse();
        team.CoachLevel.Should().Be(Team.MinCoachLevel);
        team.AssistantCoachLevel.Should().Be(0, "AI team starts with no assistant coach");
        team.DoctorLevel.Should().Be(0, "AI team starts with no doctor");
        team.SpokespersonLevel.Should().Be(0, "AI team starts with no spokesperson");
        team.FinancialDirectorLevel.Should().Be(0, "AI team starts with no financial director");
    }

    #endregion
}
