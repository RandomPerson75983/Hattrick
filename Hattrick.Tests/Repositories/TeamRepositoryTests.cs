using Hattrick.Core.Models;
using Hattrick.Core.Repositories;

namespace Hattrick.Tests.Repositories;

public class TeamRepositoryTests
{
    private readonly ITeamRepository _sut = new TeamRepository();

    private static Team CreateTeam(string name = "Test United", bool isHumanControlled = false)
    {
        return new Team
        {
            Name = name,
            IsHumanControlled = isHumanControlled,
            Budget = 500_000m,
            Fans = 1000,
            TeamSpirit = 5.0,
            Confidence = 5.0,
            CoachLevel = 3,
        };
    }

    #region Add

    [Fact]
    public void Add_SingleTeam_IsRetrievableById()
    {
        // Arrange
        var team = CreateTeam("Northford Athletic");

        // Act
        _sut.Add(team);
        var result = _sut.GetById(team.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(team.Id);
        result.Name.Should().Be("Northford Athletic");
        result.IsHumanControlled.Should().BeFalse();
    }

    [Fact]
    public void Add_MultipleTeams_AllAreReturnedByGetAll()
    {
        // Arrange
        var team1 = CreateTeam("Northford Athletic");
        var team2 = CreateTeam("Southwick City");
        var team3 = CreateTeam("Eastbridge United");

        // Act
        _sut.Add(team1);
        _sut.Add(team2);
        _sut.Add(team3);

        // Assert
        var result1 = _sut.GetById(team1.Id);
        result1.Should().NotBeNull();
        result1!.Name.Should().Be("Northford Athletic");

        var result2 = _sut.GetById(team2.Id);
        result2.Should().NotBeNull();
        result2!.Name.Should().Be("Southwick City");

        var result3 = _sut.GetById(team3.Id);
        result3.Should().NotBeNull();
        result3!.Name.Should().Be("Eastbridge United");

        _sut.GetAll().Should().HaveCount(3);
    }

    [Fact]
    public void Add_NullTeam_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.Add(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("team");
    }

    [Fact]
    public void Add_DuplicateId_ThrowsArgumentException()
    {
        // Arrange
        var team = CreateTeam("Original Town");
        _sut.Add(team);

        var duplicate = new Team
        {
            Id = team.Id,
            Name = "Duplicate City",
            Budget = 100_000m,
        };

        // Act
        var act = () => _sut.Add(duplicate);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region GetById

    [Fact]
    public void GetById_ReturnsTeamWhenExists()
    {
        // Arrange
        var team = CreateTeam("Findable FC");
        _sut.Add(team);

        // Act
        var result = _sut.GetById(team.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(team.Id);
        result.Name.Should().Be("Findable FC");
        result.Budget.Should().Be(500_000m);
    }

    [Fact]
    public void GetById_ReturnsNullWhenNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = _sut.GetById(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetById_EmptyRepository_ReturnsNull()
    {
        // Act
        var result = _sut.GetById(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetById_DoesNotReturnTeamWithDifferentId()
    {
        // Arrange
        var team = CreateTeam("Wrong Id FC");
        _sut.Add(team);

        // Act
        var result = _sut.GetById(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAll

    [Fact]
    public void GetAll_EmptyRepository_ReturnsEmptyList()
    {
        // Act
        var result = _sut.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAll_ReturnsAllAddedTeams()
    {
        // Arrange
        var team1 = CreateTeam("Northford Athletic");
        var team2 = CreateTeam("Southwick City");
        var team3 = CreateTeam("Eastbridge United");
        _sut.Add(team1);
        _sut.Add(team2);
        _sut.Add(team3);

        // Act
        var result = _sut.GetAll();

        // Assert
        result.Should().HaveCount(3);
        result.Select(t => t.Name).Should().BeEquivalentTo(
            new[] { "Northford Athletic", "Southwick City", "Eastbridge United" });
    }

    [Fact]
    public void GetAll_ReturnsIReadOnlyList()
    {
        // Arrange
        var team = CreateTeam("Type Check FC");
        _sut.Add(team);

        // Act
        var result = _sut.GetAll();

        // Assert
        result.Should().BeAssignableTo<IReadOnlyList<Team>>();
        result.Should().ContainSingle()
            .Which.Name.Should().Be("Type Check FC");
    }

    [Fact]
    public void GetAll_ReturnedListIsSnapshot_NotLiveReference()
    {
        // Arrange
        var team1 = CreateTeam("First Team");
        _sut.Add(team1);

        // Act — take snapshot, then add more teams
        var snapshot = _sut.GetAll();
        var team2 = CreateTeam("Second Team");
        _sut.Add(team2);

        // Assert — snapshot still has only 1 team; fresh query returns 2
        snapshot.Should().HaveCount(1);
        _sut.GetAll().Should().HaveCount(2);
    }

    [Fact]
    public void GetAll_ReturnsTeamsWithCorrectFieldValues()
    {
        // Arrange
        var team = new Team
        {
            Name = "Full Fields FC",
            IsHumanControlled = true,
            Budget = 750_000m,
            Fans = 5000,
            FanClubSize = 200,
            TeamSpirit = 7.5,
            Confidence = 6.0,
            CoachLevel = 5,
            AssistantCoachLevel = 3,
            DoctorLevel = 2,
        };
        _sut.Add(team);

        // Act
        var result = _sut.GetAll();

        // Assert
        result.Should().ContainSingle();
        var stored = result[0];
        stored.Id.Should().Be(team.Id);
        stored.Name.Should().Be("Full Fields FC");
        stored.IsHumanControlled.Should().BeTrue();
        stored.Budget.Should().Be(750_000m);
        stored.Fans.Should().Be(5000);
        stored.FanClubSize.Should().Be(200);
        stored.TeamSpirit.Should().Be(7.5);
        stored.Confidence.Should().Be(6.0);
        stored.CoachLevel.Should().Be(5);
        stored.AssistantCoachLevel.Should().Be(3);
        stored.DoctorLevel.Should().Be(2);
    }

    #endregion

    #region Update

    [Fact]
    public void Update_ReplacesTeamData()
    {
        // Arrange
        var team = CreateTeam("Original Town");
        _sut.Add(team);

        var updatedTeam = new Team
        {
            Id = team.Id,
            Name = "Updated Town",
            Budget = 999_000m,
            Fans = 8000,
            TeamSpirit = 9.0,
        };

        // Act
        _sut.Update(updatedTeam);
        var result = _sut.GetById(team.Id);

        // Assert — fields explicitly set on updatedTeam
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Town");
        result.Budget.Should().Be(999_000m);
        result.Fans.Should().Be(8000);
        result.TeamSpirit.Should().Be(9.0);

        // Assert — fields NOT set on updatedTeam default to new Team() defaults,
        // confirming full-object replacement (not a partial patch)
        result.IsHumanControlled.Should().BeFalse("updatedTeam did not set IsHumanControlled");
        result.FanClubSize.Should().Be(0, "updatedTeam did not set FanClubSize");
        result.Confidence.Should().Be(0.0, "updatedTeam did not set Confidence");
        result.CoachLevel.Should().Be(Team.MinCoachLevel, "updatedTeam did not set CoachLevel");
        result.CoachType.Should().Be(CoachType.Offensive, "updatedTeam did not set CoachType");
        result.AssistantCoachLevel.Should().Be(0, "updatedTeam did not set AssistantCoachLevel");
        result.DoctorLevel.Should().Be(0, "updatedTeam did not set DoctorLevel");
        result.SpokespersonLevel.Should().Be(0, "updatedTeam did not set SpokespersonLevel");
        result.FinancialDirectorLevel.Should().Be(0, "updatedTeam did not set FinancialDirectorLevel");
    }

    [Fact]
    public void Update_PreservesTeamId()
    {
        // Arrange
        var team = CreateTeam("Original");
        var originalId = team.Id;
        _sut.Add(team);

        var updatedTeam = new Team
        {
            Id = originalId,
            Name = "Changed",
        };

        // Act
        _sut.Update(updatedTeam);
        var result = _sut.GetById(originalId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(originalId);
        result.Name.Should().Be("Changed");
    }

    [Fact]
    public void Update_NullTeam_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.Update(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("team");
    }

    [Fact]
    public void Update_NonExistentTeam_ThrowsKeyNotFoundException()
    {
        // Arrange
        var team = CreateTeam("Ghost FC");

        // Act — team was never added
        var act = () => _sut.Update(team);

        // Assert
        act.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void Update_DoesNotAffectOtherTeams()
    {
        // Arrange
        var team1 = CreateTeam("Team One");
        var team2 = CreateTeam("Team Two");
        _sut.Add(team1);
        _sut.Add(team2);

        var updatedTeam1 = new Team
        {
            Id = team1.Id,
            Name = "Updated Team One",
            Budget = 1_000_000m,
        };

        // Act
        _sut.Update(updatedTeam1);

        // Assert — team2 is unchanged
        var unchanged = _sut.GetById(team2.Id);
        unchanged.Should().NotBeNull();
        unchanged!.Name.Should().Be("Team Two");
        unchanged.Budget.Should().Be(500_000m);
    }

    [Fact]
    public void Update_AllTeamFields_ArePersisted()
    {
        // Arrange
        var team = CreateTeam("Partial FC");
        _sut.Add(team);

        var updatedTeam = new Team
        {
            Id = team.Id,
            Name = "Complete United",
            IsHumanControlled = true,
            Budget = 1_200_000m,
            Fans = 12_000,
            FanClubSize = 500,
            TeamSpirit = 8.5,
            Confidence = 7.0,
            CoachType = CoachType.Defensive,
            CoachLevel = 6,
            AssistantCoachLevel = 4,
            DoctorLevel = 3,
            SpokespersonLevel = 2,
            FinancialDirectorLevel = 1,
        };

        // Act
        _sut.Update(updatedTeam);
        var result = _sut.GetById(team.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Complete United");
        result.IsHumanControlled.Should().BeTrue();
        result.Budget.Should().Be(1_200_000m);
        result.Fans.Should().Be(12_000);
        result.FanClubSize.Should().Be(500);
        result.TeamSpirit.Should().Be(8.5);
        result.Confidence.Should().Be(7.0);
        result.CoachType.Should().Be(CoachType.Defensive);
        result.CoachLevel.Should().Be(6);
        result.AssistantCoachLevel.Should().Be(4);
        result.DoctorLevel.Should().Be(3);
        result.SpokespersonLevel.Should().Be(2);
        result.FinancialDirectorLevel.Should().Be(1);
    }

    #endregion

    #region No Remove method (teams are permanent)

    [Fact]
    public void Interface_WhenInspected_HasNoRemoveMethod()
    {
        // Teams are permanent in Phase 1 — verify ITeamRepository has no Remove method
        var interfaceType = typeof(ITeamRepository);
        var removeMethods = interfaceType.GetMethods()
            .Where(m => m.Name.Equals("Remove", StringComparison.Ordinal))
            .ToList();

        removeMethods.Should().BeEmpty("teams are permanent entities and ITeamRepository must not expose Remove");
    }

    #endregion

    #region Interface compliance

    [Fact]
    public void Class_WhenInstantiated_ImplementsITeamRepository()
    {
        // Assert
        _sut.Should().BeAssignableTo<ITeamRepository>();
    }

    #endregion

    #region Thread Safety

    [Fact]
    public void ConcurrentAdds_DoNotCorruptState()
    {
        // Arrange
        const int teamCount = 200;
        var teams = Enumerable.Range(0, teamCount)
            .Select(i => CreateTeam($"Concurrent Team {i}"))
            .ToList();
        var teamIds = teams.Select(t => t.Id).ToList();

        // Act — intentional parallel adds to exercise thread safety
        Parallel.For(0, teamCount, i => _sut.Add(teams[i]));

        // Assert
        var results = _sut.GetAll();
        results.Should().HaveCount(teamCount);
        results.Select(t => t.Id).Should().BeEquivalentTo(teamIds, "all added team IDs must be present");
    }

    [Fact]
    public void ConcurrentReadsAndWrites_DoNotThrow()
    {
        // Arrange
        const int iterationCount = 100;
        var preloadedTeams = new List<Team>();

        // Pre-load some teams
        const int preloadCount = 50;
        for (var i = 0; i < preloadCount; i++)
        {
            var team = CreateTeam($"Preloaded Team {i}");
            _sut.Add(team);
            preloadedTeams.Add(team);
        }

        // Act — concurrent reads (GetAll, GetById) and writes (Add)
        var act = () => Parallel.For(0, iterationCount, i =>
        {
            if (i % 3 == 0)
            {
                // Add
                var team = CreateTeam($"New Team {i}");
                _sut.Add(team);
            }
            else if (i % 3 == 1)
            {
                // Read all
                var _ = _sut.GetAll();
            }
            else
            {
                // Read by id
                var idx = i % preloadedTeams.Count;
                var _ = _sut.GetById(preloadedTeams[idx].Id);
            }
        });

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ConcurrentUpdates_DoNotCorruptState()
    {
        // Arrange
        var team = CreateTeam("Shared Team");
        _sut.Add(team);
        const int iterationCount = 200;

        // Act — multiple threads updating the same team
        var act = () => Parallel.For(0, iterationCount, i =>
        {
            var updatedTeam = new Team
            {
                Id = team.Id,
                Name = $"Updated by thread {i}",
                Budget = 100_000m + i,
                Fans = 1000 + i,
            };
            _sut.Update(updatedTeam);
        });

        // Assert
        act.Should().NotThrow();
        var result = _sut.GetById(team.Id);
        result.Should().NotBeNull();
        result!.Name.Should().StartWith("Updated by thread ");
        result.Budget.Should().BeGreaterThan(0, "some thread must have set Budget");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void GetAll_AfterUpdate_ReflectsNewData()
    {
        // Arrange
        var team = CreateTeam("Before Update FC");
        _sut.Add(team);

        var updatedTeam = new Team
        {
            Id = team.Id,
            Name = "After Update FC",
            Budget = 250_000m,
        };

        // Act
        _sut.Update(updatedTeam);
        var result = _sut.GetAll();

        // Assert
        result.Should().ContainSingle()
            .Which.Name.Should().Be("After Update FC");
    }

    [Fact]
    public void GetAll_HumanAndAiTeams_AreAllReturned()
    {
        // Arrange
        var humanTeam = CreateTeam("Human FC", isHumanControlled: true);
        var aiTeam1 = CreateTeam("AI Rovers");
        var aiTeam2 = CreateTeam("AI City");
        _sut.Add(humanTeam);
        _sut.Add(aiTeam1);
        _sut.Add(aiTeam2);

        // Act
        var result = _sut.GetAll();

        // Assert
        result.Should().HaveCount(3);
        result.Count(t => t.IsHumanControlled).Should().Be(1);
        result.Count(t => !t.IsHumanControlled).Should().Be(2);
        result.Single(t => t.IsHumanControlled).Name.Should().Be("Human FC");
    }

    #endregion
}
