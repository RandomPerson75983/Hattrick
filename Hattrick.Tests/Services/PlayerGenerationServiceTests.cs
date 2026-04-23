using Hattrick.Core.Models;
using Hattrick.Core.Services;
using NSubstitute;

namespace Hattrick.Tests.Services;

/// <summary>
/// Unit tests for IPlayerGenerationService.
///
/// PlayerGenerationService generates players with randomized but position-appropriate attributes.
/// All randomness flows through IRandomProvider for testability.
///
/// Requirements:
/// - GeneratePlayer(Position position) returns a Player
/// - Age: 17-32 (inclusive)
/// - Form: 5-8 (inclusive)
/// - Stamina: 5-8 (inclusive)
/// - Experience: 1-5 (inclusive)
/// - Skills: 8 skill types, levels position-appropriate
/// - Specialty: random from enum or None
/// - Name: random from list (not empty)
///
/// Position-appropriate skill rules:
/// - Keeper: high Keeper skill, low outfield skills
/// - CentralDefender: high Defending skill
/// - WingBack: high Defending and Winger skills
/// - InnerMidfielder: high Playmaking and Passing skills
/// - Winger: high Winger and Passing skills
/// - Forward: high Scoring skill
/// </summary>
public class PlayerGenerationServiceTests
{
    private readonly IRandomProvider _randomMock;
    private readonly IPlayerGenerationService _sut;

    // Constants matching PlayerGenerationService bounds
    private const int MinAge = 17;
    private const int MaxAge = 32;
    private const int AgeMaxExclusive = MaxAge + 1;
    private const int MinForm = 5;
    private const int MaxForm = 8;
    private const int MinStamina = 5;
    private const int MaxStamina = 8;
    private const int MinExperience = 1;
    private const int MaxExperience = 5;
    private const int MinSkillValue = 1;
    private const int StatisticalSampleSize = 1000;
    private const double LowSkillThreshold = 3.0;

    public PlayerGenerationServiceTests()
    {
        _randomMock = Substitute.For<IRandomProvider>();
        _sut = new PlayerGenerationService(_randomMock);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Constructor / DI Tests
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithNullRandomProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PlayerGenerationService(null!));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Basic Contract
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_WhenCalled_ReturnsNonNullPlayer()
    {
        // Arrange: Set up mock to return deterministic values
        SetupDeterministicRandom();

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert
        player.Should().NotBeNull();
    }

    [Fact]
    public void GeneratePlayer_WhenCalled_ReturnsPlayerWithUniqueId()
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert
        player.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void GeneratePlayer_WhenCalledTwice_ReturnsDifferentIds()
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        var player1 = _sut.GeneratePlayer(Position.Forward);
        var player2 = _sut.GeneratePlayer(Position.Forward);

        // Assert
        player1.Id.Should().NotBe(player2.Id);
    }

    [Fact]
    public void GeneratePlayer_WhenCalled_ReturnsBestPositionMatchingInput()
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        var player = _sut.GeneratePlayer(Position.InnerMidfielder);

        // Assert
        player.BestPosition.Should().Be(Position.InnerMidfielder);
    }

    [Theory]
    [InlineData(Position.Keeper)]
    [InlineData(Position.CentralDefender)]
    [InlineData(Position.WingBack)]
    [InlineData(Position.InnerMidfielder)]
    [InlineData(Position.Winger)]
    [InlineData(Position.Forward)]
    public void GeneratePlayer_ForAllPositions_SetsBestPositionCorrectly(Position position)
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        var player = _sut.GeneratePlayer(position);

        // Assert
        player.BestPosition.Should().Be(position);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Name
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_WhenCalled_ReturnsPlayerWithNonEmptyName()
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert
        player.Name.Should().NotBeNullOrWhiteSpace();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Age Bounds (17-32)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_WhenRandomReturnsMinAge_ReturnsPlayerWithAge17()
    {
        // Arrange: Use argument-specific returns to isolate age call
        _randomMock.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(callInfo =>
        {
            var min = callInfo.ArgAt<int>(0);
            var max = callInfo.ArgAt<int>(1);
            // Age range: MinAge to AgeMaxExclusive -> return MinAge
            if (min == MinAge && max == AgeMaxExclusive)
                return MinAge;
            // For other ranges, return middle value
            return (min + max) / 2;
        });
        _randomMock.NextDouble().Returns(0.5);
        _randomMock.Next(Arg.Any<int>()).Returns(callInfo => callInfo.ArgAt<int>(0) / 2);

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert: Age should be MinAge (minimum)
        player.Age.Should().Be(MinAge);
    }

    [Fact]
    public void GeneratePlayer_WhenRandomReturnsMaxAge_ReturnsPlayerWithAge32()
    {
        // Arrange: Mock returns 32 for age (max value)
        SetupDeterministicRandomWithAge(MaxAge);

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert
        player.Age.Should().Be(MaxAge);
    }

    [Fact]
    public void GeneratePlayer_Statistical_AgeIsAlwaysWithinBounds()
    {
        // Arrange: Use a real random provider for statistical test
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var ages = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward).Age)
            .ToList();

        // Assert: All ages must be in [17, 32]
        ages.Should().OnlyContain(age => age >= MinAge && age <= MaxAge);
    }

    [Fact]
    public void GeneratePlayer_Statistical_AgeDistributionCoversRange()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var ages = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward).Age)
            .ToList();

        // Assert: Should see ages at or near both ends
        ages.Should().Contain(age => age <= 20, "should include young players");
        ages.Should().Contain(age => age >= 29, "should include experienced players");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Form Bounds (5-8)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_Statistical_FormIsAlwaysWithinBounds()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var forms = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward).Form)
            .ToList();

        // Assert: All forms must be in [5, 8]
        forms.Should().OnlyContain(form => form >= 5 && form <= 8);
    }

    [Fact]
    public void GeneratePlayer_WhenRandomReturnsMinForm_ReturnsPlayerWithForm5()
    {
        // Arrange
        SetupDeterministicRandomWithForm(5);

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert
        player.Form.Should().Be(5);
    }

    [Fact]
    public void GeneratePlayer_WhenRandomReturnsMaxForm_ReturnsPlayerWithForm8()
    {
        // Arrange
        SetupDeterministicRandomWithForm(8);

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert
        player.Form.Should().Be(8);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Stamina Bounds (5-8)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_Statistical_StaminaIsAlwaysWithinBounds()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var staminas = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward).Stamina)
            .ToList();

        // Assert: All staminas must be in [5, 8]
        staminas.Should().OnlyContain(stamina => stamina >= 5 && stamina <= 8);
    }

    [Fact]
    public void GeneratePlayer_WhenRandomReturnsMinStamina_ReturnsPlayerWithStamina5()
    {
        // Arrange
        SetupDeterministicRandomWithStamina(5);

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert
        player.Stamina.Should().Be(5);
    }

    [Fact]
    public void GeneratePlayer_WhenRandomReturnsMaxStamina_ReturnsPlayerWithStamina8()
    {
        // Arrange
        SetupDeterministicRandomWithStamina(8);

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert
        player.Stamina.Should().Be(8);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Experience Bounds (1-5)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_Statistical_ExperienceIsAlwaysWithinBounds()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var experiences = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward).Experience)
            .ToList();

        // Assert: All experiences must be in [1, 5]
        experiences.Should().OnlyContain(exp => exp >= 1 && exp <= 5);
    }

    [Fact]
    public void GeneratePlayer_WhenRandomReturnsMinExperience_ReturnsPlayerWithExperience1()
    {
        // Arrange
        SetupDeterministicRandomWithExperience(1);

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert
        player.Experience.Should().Be(1);
    }

    [Fact]
    public void GeneratePlayer_WhenRandomReturnsMaxExperience_ReturnsPlayerWithExperience5()
    {
        // Arrange
        SetupDeterministicRandomWithExperience(5);

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert
        player.Experience.Should().Be(5);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Skills Dictionary
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_WhenCalled_ReturnsPlayerWithAllEightSkillTypes()
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert: All 8 skill types should be present
        var allSkillTypes = Enum.GetValues<SkillType>();
        player.Skills.Should().HaveCount(8);
        foreach (var skillType in allSkillTypes)
        {
            player.Skills.Should().ContainKey(skillType);
        }
    }

    [Fact]
    public void GeneratePlayer_WhenCalled_ReturnsPlayerWithNonNegativeSkillValues()
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert: Minimum skill value is 1 (LowSkillMin)
        player.Skills.Values.Should().OnlyContain(value => value >= 1.0);
    }

    [Fact]
    public void GeneratePlayer_Statistical_SkillValuesAreWithinReasonableBounds()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var allSkills = Enumerable.Range(0, sampleSize)
            .SelectMany(_ => service.GeneratePlayer(Position.Forward).Skills.Values)
            .ToList();

        // Assert: Skills should be reasonable (e.g., 0-20 for Hattrick scale)
        allSkills.Should().OnlyContain(skill => skill >= 0.0 && skill <= 20.0);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Position-Appropriate Skills: Keeper
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_ForKeeper_HasHighKeeperSkill()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var keepers = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Keeper))
            .ToList();
        var forwards = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward))
            .ToList();

        var keeperAvgKeeperSkill = keepers.Average(p => p.Skills[SkillType.Keeper]);
        var forwardAvgKeeperSkill = forwards.Average(p => p.Skills[SkillType.Keeper]);

        // Assert: Keepers should have significantly higher Keeper skill than Forwards (at least 2x)
        keeperAvgKeeperSkill.Should().BeGreaterThan(forwardAvgKeeperSkill * 2.0,
            "Keepers should have at least double the Keeper skill of Forwards");
    }

    [Fact]
    public void GeneratePlayer_ForKeeper_HasLowOutfieldSkills()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var keepers = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Keeper))
            .ToList();

        var avgKeeperSkill = keepers.Average(p => p.Skills[SkillType.Keeper]);
        var avgScoringSkill = keepers.Average(p => p.Skills[SkillType.Scoring]);
        var avgWingerSkill = keepers.Average(p => p.Skills[SkillType.Winger]);

        // Assert: Keeper skill should be much higher than outfield skills
        avgKeeperSkill.Should().BeGreaterThan(avgScoringSkill, "Keeper should be better at keeping than scoring");
        avgKeeperSkill.Should().BeGreaterThan(avgWingerSkill, "Keeper should be better at keeping than winger play");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Position-Appropriate Skills: CentralDefender
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_ForCentralDefender_HasHighDefendingSkill()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var defenders = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.CentralDefender))
            .ToList();
        var forwards = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward))
            .ToList();

        var defenderAvgDefending = defenders.Average(p => p.Skills[SkillType.Defending]);
        var forwardAvgDefending = forwards.Average(p => p.Skills[SkillType.Defending]);

        // Assert: Defenders should have significantly higher Defending skill than Forwards
        defenderAvgDefending.Should().BeGreaterThan(forwardAvgDefending * 1.5,
            "Defenders should have at least 1.5x the Defending skill of Forwards");
    }

    [Fact]
    public void GeneratePlayer_ForCentralDefender_HasLowKeeperSkill()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var defenders = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.CentralDefender))
            .ToList();

        var avgKeeperSkill = defenders.Average(p => p.Skills[SkillType.Keeper]);
        var avgDefendingSkill = defenders.Average(p => p.Skills[SkillType.Defending]);

        // Assert: Outfield players should have low keeper skill
        avgKeeperSkill.Should().BeLessThan(avgDefendingSkill, "Defenders should not have high keeper skill");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Position-Appropriate Skills: WingBack
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_ForWingBack_HasHighDefendingAndWingerSkills()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var wingbacks = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.WingBack))
            .ToList();
        var forwards = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward))
            .ToList();

        var wingbackAvgDefending = wingbacks.Average(p => p.Skills[SkillType.Defending]);
        var wingbackAvgWinger = wingbacks.Average(p => p.Skills[SkillType.Winger]);
        var forwardAvgDefending = forwards.Average(p => p.Skills[SkillType.Defending]);
        var forwardAvgWinger = forwards.Average(p => p.Skills[SkillType.Winger]);

        // Assert: Wingbacks should have both defending and winger skills higher than forwards
        wingbackAvgDefending.Should().BeGreaterThan(forwardAvgDefending,
            "Wingbacks should defend better than Forwards");
        wingbackAvgWinger.Should().BeGreaterThan(forwardAvgWinger,
            "Wingbacks should have better winger skill than Forwards");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Position-Appropriate Skills: InnerMidfielder
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_ForInnerMidfielder_HasHighPlaymakingAndPassingSkills()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var midfielders = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.InnerMidfielder))
            .ToList();
        var defenders = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.CentralDefender))
            .ToList();

        var midfielderAvgPlaymaking = midfielders.Average(p => p.Skills[SkillType.Playmaking]);
        var midfielderAvgPassing = midfielders.Average(p => p.Skills[SkillType.Passing]);
        var defenderAvgPlaymaking = defenders.Average(p => p.Skills[SkillType.Playmaking]);
        var defenderAvgPassing = defenders.Average(p => p.Skills[SkillType.Passing]);

        // Assert: Midfielders should have higher playmaking/passing than Defenders
        midfielderAvgPlaymaking.Should().BeGreaterThan(defenderAvgPlaymaking,
            "Midfielders should have better playmaking than Defenders");
        midfielderAvgPassing.Should().BeGreaterThan(defenderAvgPassing,
            "Midfielders should have better passing than Defenders");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Position-Appropriate Skills: Winger
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_ForWinger_HasHighWingerAndPassingSkills()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var wingers = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Winger))
            .ToList();
        var defenders = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.CentralDefender))
            .ToList();

        var wingerAvgWinger = wingers.Average(p => p.Skills[SkillType.Winger]);
        var wingerAvgPassing = wingers.Average(p => p.Skills[SkillType.Passing]);
        var defenderAvgWinger = defenders.Average(p => p.Skills[SkillType.Winger]);
        var defenderAvgPassing = defenders.Average(p => p.Skills[SkillType.Passing]);

        // Assert: Wingers should have higher winger/passing skills than Defenders
        wingerAvgWinger.Should().BeGreaterThan(defenderAvgWinger,
            "Wingers should have better winger skill than Defenders");
        wingerAvgPassing.Should().BeGreaterThan(defenderAvgPassing,
            "Wingers should have better passing than Defenders");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Position-Appropriate Skills: Forward
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_ForForward_HasHighScoringSkill()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var forwards = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward))
            .ToList();
        var defenders = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.CentralDefender))
            .ToList();

        var forwardAvgScoring = forwards.Average(p => p.Skills[SkillType.Scoring]);
        var defenderAvgScoring = defenders.Average(p => p.Skills[SkillType.Scoring]);

        // Assert: Forwards should have significantly higher Scoring skill than Defenders
        forwardAvgScoring.Should().BeGreaterThan(defenderAvgScoring * 1.5,
            "Forwards should have at least 1.5x the Scoring skill of Defenders");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Cross-Position Skill Differentiation
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_KeeperVsForward_HaveDifferentPrimarySkills()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var keepers = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Keeper))
            .ToList();
        var forwards = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward))
            .ToList();

        var keeperAvgKeepingSkill = keepers.Average(p => p.Skills[SkillType.Keeper]);
        var forwardAvgKeepingSkill = forwards.Average(p => p.Skills[SkillType.Keeper]);
        var keeperAvgScoringSkill = keepers.Average(p => p.Skills[SkillType.Scoring]);
        var forwardAvgScoringSkill = forwards.Average(p => p.Skills[SkillType.Scoring]);

        // Assert: Position specialization should be evident with significant differences
        keeperAvgKeepingSkill.Should().BeGreaterThan(forwardAvgKeepingSkill * 2.0,
            "Keepers should have at least double the Keeper skill of Forwards");
        forwardAvgScoringSkill.Should().BeGreaterThan(keeperAvgScoringSkill * 2.0,
            "Forwards should have at least double the Scoring skill of Keepers");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Specialty
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_WhenCalled_ReturnsPlayerWithValidSpecialty()
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert: Specialty should be a valid enum value
        Enum.IsDefined(typeof(Specialty), player.Specialty).Should().BeTrue();
    }

    [Fact]
    public void GeneratePlayer_Statistical_SpecialtyDistributionIncludesNone()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var specialties = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward).Specialty)
            .ToList();

        // Assert: Should have some players with no specialty
        specialties.Should().Contain(Specialty.None, "some players should have no specialty");
    }

    [Fact]
    public void GeneratePlayer_Statistical_SpecialtyDistributionIncludesNonNone()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var specialties = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward).Specialty)
            .ToList();

        // Assert: Should have some players with specialties
        specialties.Should().Contain(s => s != Specialty.None, "some players should have specialties");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Default Values for Other Properties
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_WhenCalled_ReturnsPlayerWithEmptyTeamId()
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert: Generated players are not assigned to a team yet
        player.TeamId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void GeneratePlayer_WhenCalled_ReturnsPlayerWithZeroInjuryWeeks()
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert: Newly generated players should be healthy
        player.InjuryWeeks.Should().Be(0);
    }

    [Fact]
    public void GeneratePlayer_WhenCalled_ReturnsPlayerWithNoRedCard()
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert
        player.RedCard.Should().BeFalse();
    }

    [Fact]
    public void GeneratePlayer_WhenCalled_ReturnsPlayerWithZeroYellowCards()
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        var player = _sut.GeneratePlayer(Position.Forward);

        // Assert
        player.YellowCards.Should().Be(0);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — SetPieces Skill Bounds
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_Statistical_SetPiecesSkillIsWithinBounds()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var setPiecesSkills = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward).Skills[SkillType.SetPieces])
            .ToList();

        // Assert: SetPieces should be within valid skill bounds (1-20)
        setPiecesSkills.Should().OnlyContain(skill => skill >= 1.0 && skill <= 20.0,
            "SetPieces skill should be within valid bounds [1, 20]");
    }

    [Fact]
    public void GeneratePlayer_ForAllPositions_HasSetPiecesSkillGenerated()
    {
        // Arrange
        SetupDeterministicRandom();

        // Act & Assert: All positions should have SetPieces skill
        foreach (var position in Enum.GetValues<Position>())
        {
            var player = _sut.GeneratePlayer(position);
            player.Skills.Should().ContainKey(SkillType.SetPieces,
                $"Position {position} should have SetPieces skill");
            player.Skills[SkillType.SetPieces].Should().BeGreaterThanOrEqualTo(0.0,
                $"Position {position} should have non-negative SetPieces skill");
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Non-Primary Skills Stay Low (Over-Correction Tests)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_ForForward_HasLowKeeperSkill()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var forwards = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward))
            .ToList();

        var avgKeeperSkill = forwards.Average(p => p.Skills[SkillType.Keeper]);

        // Assert: Forwards should have low Keeper skill (below 3.0 average)
        avgKeeperSkill.Should().BeLessThan(LowSkillThreshold,
            "Forwards should have low Keeper skill (non-primary)");
    }

    [Fact]
    public void GeneratePlayer_ForForward_HasLowDefendingSkill()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var forwards = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Forward))
            .ToList();

        var avgDefendingSkill = forwards.Average(p => p.Skills[SkillType.Defending]);

        // Assert: Forwards should have low Defending skill (below 3.0 average)
        avgDefendingSkill.Should().BeLessThan(LowSkillThreshold,
            "Forwards should have low Defending skill (non-primary)");
    }

    [Fact]
    public void GeneratePlayer_ForKeeper_HasLowScoringSkill()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var keepers = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Keeper))
            .ToList();

        var avgScoringSkill = keepers.Average(p => p.Skills[SkillType.Scoring]);

        // Assert: Keepers should have low Scoring skill (below 3.0 average)
        avgScoringSkill.Should().BeLessThan(LowSkillThreshold,
            "Keepers should have low Scoring skill (non-primary)");
    }

    [Fact]
    public void GeneratePlayer_ForKeeper_HasLowWingerSkill()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var keepers = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.Keeper))
            .ToList();

        var avgWingerSkill = keepers.Average(p => p.Skills[SkillType.Winger]);

        // Assert: Keepers should have low Winger skill (below 3.0 average)
        avgWingerSkill.Should().BeLessThan(LowSkillThreshold,
            "Keepers should have low Winger skill (non-primary)");
    }

    [Fact]
    public void GeneratePlayer_ForCentralDefender_HasLowScoringSkill()
    {
        // Arrange
        var realRandom = new RandomProvider();
        var service = new PlayerGenerationService(realRandom);
        var sampleSize = StatisticalSampleSize;

        // Act
        var defenders = Enumerable.Range(0, sampleSize)
            .Select(_ => service.GeneratePlayer(Position.CentralDefender))
            .ToList();

        var avgScoringSkill = defenders.Average(p => p.Skills[SkillType.Scoring]);

        // Assert: Central Defenders should have low Scoring skill (below 3.0 average)
        avgScoringSkill.Should().BeLessThan(LowSkillThreshold,
            "Central Defenders should have low Scoring skill (non-primary)");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GeneratePlayer — Uses IRandomProvider (No Direct Random Access)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GeneratePlayer_WhenCalled_UsesIRandomProviderForRandomness()
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        _sut.GeneratePlayer(Position.Forward);

        // Assert: Verify IRandomProvider was called (at least once for any random value)
        _randomMock.ReceivedWithAnyArgs().Next(0, 0);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Edge Cases
    // ─────────────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(Position.Keeper)]
    [InlineData(Position.CentralDefender)]
    [InlineData(Position.WingBack)]
    [InlineData(Position.InnerMidfielder)]
    [InlineData(Position.Winger)]
    [InlineData(Position.Forward)]
    public void GeneratePlayer_ForAllPositions_ReturnsValidPlayer(Position position)
    {
        // Arrange
        SetupDeterministicRandom();

        // Act
        var player = _sut.GeneratePlayer(position);

        // Assert: Basic validity checks
        player.Should().NotBeNull();
        player.Name.Should().NotBeNullOrWhiteSpace();
        player.Age.Should().BeInRange(17, 32);
        player.Form.Should().BeInRange(5, 8);
        player.Stamina.Should().BeInRange(5, 8);
        player.Experience.Should().BeInRange(1, 5);
        player.Skills.Should().HaveCount(8);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helper Methods
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Sets up the mock to return deterministic middle-of-range values.
    /// This ensures tests don't fail due to boundary conditions unless testing boundaries.
    /// </summary>
    private void SetupDeterministicRandom()
    {
        // Return middle values for int ranges
        _randomMock.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(callInfo =>
        {
            var min = callInfo.ArgAt<int>(0);
            var max = callInfo.ArgAt<int>(1);
            return (min + max) / 2;
        });

        // Return 0.5 for doubles
        _randomMock.NextDouble().Returns(0.5);

        // Return middle value for single-arg Next
        _randomMock.Next(Arg.Any<int>()).Returns(callInfo =>
        {
            var max = callInfo.ArgAt<int>(0);
            return max / 2;
        });
    }

    /// <summary>
    /// Sets up the mock to return a specific age value.
    /// </summary>
    private void SetupDeterministicRandomWithAge(int age)
    {
        _randomMock.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(callInfo =>
        {
            var min = callInfo.ArgAt<int>(0);
            var max = callInfo.ArgAt<int>(1);
            // If this is the age range (17-33 exclusive), return the specified age
            if (min == 17 && max == 33)
                return age;
            return (min + max) / 2;
        });
        _randomMock.NextDouble().Returns(0.5);
        _randomMock.Next(Arg.Any<int>()).Returns(callInfo => callInfo.ArgAt<int>(0) / 2);
    }

    /// <summary>
    /// Sets up the mock to return a specific form value.
    /// </summary>
    private void SetupDeterministicRandomWithForm(int form)
    {
        _randomMock.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(callInfo =>
        {
            var min = callInfo.ArgAt<int>(0);
            var max = callInfo.ArgAt<int>(1);
            // If this is the form range (5-9 exclusive), return the specified form
            if (min == 5 && max == 9)
                return form;
            return (min + max) / 2;
        });
        _randomMock.NextDouble().Returns(0.5);
        _randomMock.Next(Arg.Any<int>()).Returns(callInfo => callInfo.ArgAt<int>(0) / 2);
    }

    /// <summary>
    /// Sets up the mock to return a specific stamina value.
    /// </summary>
    private void SetupDeterministicRandomWithStamina(int stamina)
    {
        _randomMock.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(callInfo =>
        {
            var min = callInfo.ArgAt<int>(0);
            var max = callInfo.ArgAt<int>(1);
            // Note: If form and stamina have same range (5-9), we return the value for both
            // The implementation might need to call them in a specific order
            if (min == 5 && max == 9)
                return stamina;
            return (min + max) / 2;
        });
        _randomMock.NextDouble().Returns(0.5);
        _randomMock.Next(Arg.Any<int>()).Returns(callInfo => callInfo.ArgAt<int>(0) / 2);
    }

    /// <summary>
    /// Sets up the mock to return a specific experience value.
    /// </summary>
    private void SetupDeterministicRandomWithExperience(int experience)
    {
        _randomMock.Next(Arg.Any<int>(), Arg.Any<int>()).Returns(callInfo =>
        {
            var min = callInfo.ArgAt<int>(0);
            var max = callInfo.ArgAt<int>(1);
            // If this is the experience range (1-6 exclusive), return the specified experience
            if (min == 1 && max == 6)
                return experience;
            return (min + max) / 2;
        });
        _randomMock.NextDouble().Returns(0.5);
        _randomMock.Next(Arg.Any<int>()).Returns(callInfo => callInfo.ArgAt<int>(0) / 2);
    }
}
