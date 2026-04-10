using Hattrick.Core.Models;
using Hattrick.Core.Services;
using System.Reflection;

namespace Hattrick.Tests.Services;

/// <summary>
/// Unit tests for IPlayerStatsService.
///
/// PlayerStatsService computes team-level aggregate stats (totals and averages)
/// from a list of players. All methods are pure functions with no side effects.
///
/// Formulas:
///   EstimatedValue = TSI * 25  (integer multiplication → decimal result)
///   NationalityCount = count of distinct NativeCountryId values
///   InjuredCount    = count of players where InjuryWeeks > 0
///   RedCardCount    = count of players where RedCard == true
///   YellowCardCount = sum of all YellowCards across the squad
///   Averages        = arithmetic mean; return 0.0 for empty squad (no divide-by-zero)
/// </summary>
public class PlayerStatsServiceTests
{
    private readonly IPlayerStatsService _sut = new PlayerStatsService();

    // ─────────────────────────────────────────────────────────────────────────
    // Helper
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Builds a minimal Player with all fields required by IPlayerStatsService.
    /// Every parameter is positional so tests stay concise.
    /// </summary>
    private static Player CreatePlayer(
        int tsi,
        decimal wage,
        int nationalityId,
        int injuryWeeks,
        bool redCard,
        int yellowCards,
        int form,
        int stamina,
        int experience,
        int age)
    {
        return new Player
        {
            TSI = tsi,
            Wage = wage,
            NativeCountryId = nationalityId,
            InjuryWeeks = injuryWeeks,
            RedCard = redCard,
            YellowCards = yellowCards,
            Form = form,
            Stamina = stamina,
            Experience = experience,
            Age = age,
        };
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GetTeamTotals — empty squad
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GetTeamTotals_WithEmptyList_ReturnsTotalTSIOfZero()
    {
        var result = _sut.GetTeamTotals(new List<Player>());

        result.TotalTSI.Should().Be(0);
    }

    [Fact]
    public void GetTeamTotals_WithEmptyList_ReturnsTotalWageOfZero()
    {
        var result = _sut.GetTeamTotals(new List<Player>());

        result.TotalWage.Should().Be(0m);
    }

    [Fact]
    public void GetTeamTotals_WithEmptyList_ReturnsTotalEstimatedValueOfZero()
    {
        var result = _sut.GetTeamTotals(new List<Player>());

        result.TotalEstimatedValue.Should().Be(0m);
    }

    [Fact]
    public void GetTeamTotals_WithEmptyList_ReturnsNationalityCountOfZero()
    {
        var result = _sut.GetTeamTotals(new List<Player>());

        result.NationalityCount.Should().Be(0);
    }

    [Fact]
    public void GetTeamTotals_WithEmptyList_ReturnsInjuredCountOfZero()
    {
        var result = _sut.GetTeamTotals(new List<Player>());

        result.InjuredCount.Should().Be(0);
    }

    [Fact]
    public void GetTeamTotals_WithEmptyList_ReturnsRedCardCountOfZero()
    {
        var result = _sut.GetTeamTotals(new List<Player>());

        result.RedCardCount.Should().Be(0);
    }

    [Fact]
    public void GetTeamTotals_WithEmptyList_ReturnsYellowCardCountOfZero()
    {
        var result = _sut.GetTeamTotals(new List<Player>());

        result.YellowCardCount.Should().Be(0);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GetTeamTotals — single player
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GetTeamTotals_WithSinglePlayer_ReturnsCorrectTotalTSI()
    {
        var players = new List<Player>
        {
            CreatePlayer(tsi: 4500, wage: 12000m, nationalityId: 1,
                         injuryWeeks: 0, redCard: false, yellowCards: 0,
                         form: 5, stamina: 6, experience: 7, age: 25),
        };

        var result = _sut.GetTeamTotals(players);

        result.TotalTSI.Should().Be(4500);
    }

    [Fact]
    public void GetTeamTotals_WithSinglePlayer_ReturnsCorrectTotalWage()
    {
        var players = new List<Player>
        {
            CreatePlayer(tsi: 4500, wage: 12000m, nationalityId: 1,
                         injuryWeeks: 0, redCard: false, yellowCards: 0,
                         form: 5, stamina: 6, experience: 7, age: 25),
        };

        var result = _sut.GetTeamTotals(players);

        result.TotalWage.Should().Be(12000m);
    }

    [Fact]
    public void GetTeamTotals_WithSinglePlayer_ReturnsCorrectTotalEstimatedValue()
    {
        // EstimatedValue formula: TSI * 25
        // 4500 * 25 = 112500
        var players = new List<Player>
        {
            CreatePlayer(tsi: 4500, wage: 12000m, nationalityId: 1,
                         injuryWeeks: 0, redCard: false, yellowCards: 0,
                         form: 5, stamina: 6, experience: 7, age: 25),
        };

        var result = _sut.GetTeamTotals(players);

        result.TotalEstimatedValue.Should().Be(112500m);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GetTeamTotals — multiple players (sums)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GetTeamTotals_WithMultiplePlayers_ReturnsSumOfTSI()
    {
        // 4500 + 2000 + 7000 = 13500
        var players = new List<Player>
        {
            CreatePlayer(tsi: 4500, wage: 10000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 2000, wage: 8000m,  nationalityId: 2, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 4, stamina: 4, experience: 4, age: 22),
            CreatePlayer(tsi: 7000, wage: 15000m, nationalityId: 3, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 6, stamina: 7, experience: 9, age: 30),
        };

        var result = _sut.GetTeamTotals(players);

        result.TotalTSI.Should().Be(13500);
    }

    [Fact]
    public void GetTeamTotals_WithMultiplePlayers_ReturnsSumOfWage()
    {
        // 10000 + 8000 + 15000 = 33000
        var players = new List<Player>
        {
            CreatePlayer(tsi: 4500, wage: 10000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 2000, wage: 8000m,  nationalityId: 2, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 4, stamina: 4, experience: 4, age: 22),
            CreatePlayer(tsi: 7000, wage: 15000m, nationalityId: 3, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 6, stamina: 7, experience: 9, age: 30),
        };

        var result = _sut.GetTeamTotals(players);

        result.TotalWage.Should().Be(33000m);
    }

    [Fact]
    public void GetTeamTotals_WithMultiplePlayers_ReturnsSumOfEstimatedValue()
    {
        // (4500 + 2000 + 7000) * 25 = 13500 * 25 = 337500
        var players = new List<Player>
        {
            CreatePlayer(tsi: 4500, wage: 10000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 2000, wage: 8000m,  nationalityId: 2, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 4, stamina: 4, experience: 4, age: 22),
            CreatePlayer(tsi: 7000, wage: 15000m, nationalityId: 3, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 6, stamina: 7, experience: 9, age: 30),
        };

        var result = _sut.GetTeamTotals(players);

        result.TotalEstimatedValue.Should().Be(337500m);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GetTeamTotals — NationalityCount (distinct)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GetTeamTotals_WithTwoDistinctNationalities_ReturnsNationalityCountOfTwo()
    {
        // Players: nationalityId = 1, 1, 2 → 2 distinct countries
        var players = new List<Player>
        {
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 2, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
        };

        var result = _sut.GetTeamTotals(players);

        result.NationalityCount.Should().Be(2);
    }

    [Fact]
    public void GetTeamTotals_WithAllSameNationality_ReturnsNationalityCountOfOne()
    {
        var players = new List<Player>
        {
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 42, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 42, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
        };

        var result = _sut.GetTeamTotals(players);

        result.NationalityCount.Should().Be(1);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GetTeamTotals — InjuredCount
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GetTeamTotals_WithTwoInjuredPlayers_ReturnsInjuredCountOfTwo()
    {
        // InjuryWeeks > 0 means injured
        var players = new List<Player>
        {
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 3, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 1, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
        };

        var result = _sut.GetTeamTotals(players);

        result.InjuredCount.Should().Be(2);
    }

    [Fact]
    public void GetTeamTotals_WithNoInjuredPlayers_ReturnsInjuredCountOfZero()
    {
        var players = new List<Player>
        {
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
        };

        var result = _sut.GetTeamTotals(players);

        result.InjuredCount.Should().Be(0);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GetTeamTotals — RedCardCount
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GetTeamTotals_WithOneRedCardedPlayer_ReturnsRedCardCountOfOne()
    {
        var players = new List<Player>
        {
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: true,  yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
        };

        var result = _sut.GetTeamTotals(players);

        result.RedCardCount.Should().Be(1);
    }

    [Fact]
    public void GetTeamTotals_WithNoRedCards_ReturnsRedCardCountOfZero()
    {
        var players = new List<Player>
        {
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
        };

        var result = _sut.GetTeamTotals(players);

        result.RedCardCount.Should().Be(0);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GetTeamTotals — YellowCardCount (sum, not count of players)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GetTeamTotals_WithMultipleYellowCards_ReturnsSumOfAllYellowCards()
    {
        // 2 + 0 + 3 = 5 total yellow cards
        var players = new List<Player>
        {
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 2, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 3, form: 5, stamina: 5, experience: 5, age: 25),
        };

        var result = _sut.GetTeamTotals(players);

        result.YellowCardCount.Should().Be(5);
    }

    [Fact]
    public void GetTeamTotals_WithNoYellowCards_ReturnsYellowCardCountOfZero()
    {
        var players = new List<Player>
        {
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
        };

        var result = _sut.GetTeamTotals(players);

        result.YellowCardCount.Should().Be(0);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GetTeamAverages — empty squad (no divide-by-zero)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GetTeamAverages_WithEmptyList_ReturnsAvgTSIOfZero()
    {
        var result = _sut.GetTeamAverages(new List<Player>());

        result.AvgTSI.Should().Be(0.0);
    }

    [Fact]
    public void GetTeamAverages_WithEmptyList_ReturnsAvgWageOfZero()
    {
        var result = _sut.GetTeamAverages(new List<Player>());

        result.AvgWage.Should().Be(0.0);
    }

    [Fact]
    public void GetTeamAverages_WithEmptyList_ReturnsAvgEstimatedValueOfZero()
    {
        var result = _sut.GetTeamAverages(new List<Player>());

        result.AvgEstimatedValue.Should().Be(0m);
    }

    [Fact]
    public void GetTeamAverages_WithEmptyList_ReturnsAvgAgeOfZero()
    {
        var result = _sut.GetTeamAverages(new List<Player>());

        result.AvgAge.Should().Be(0.0);
    }

    [Fact]
    public void GetTeamAverages_WithEmptyList_ReturnsAvgFormOfZero()
    {
        var result = _sut.GetTeamAverages(new List<Player>());

        result.AvgForm.Should().Be(0.0);
    }

    [Fact]
    public void GetTeamAverages_WithEmptyList_ReturnsAvgStaminaOfZero()
    {
        var result = _sut.GetTeamAverages(new List<Player>());

        result.AvgStamina.Should().Be(0.0);
    }

    [Fact]
    public void GetTeamAverages_WithEmptyList_ReturnsAvgExperienceOfZero()
    {
        var result = _sut.GetTeamAverages(new List<Player>());

        result.AvgExperience.Should().Be(0.0);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GetTeamAverages — single player
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GetTeamAverages_WithSinglePlayer_ReturnsAvgTSIEqualToPlayerTSI()
    {
        var players = new List<Player>
        {
            CreatePlayer(tsi: 4500, wage: 12000m, nationalityId: 1,
                         injuryWeeks: 0, redCard: false, yellowCards: 0,
                         form: 5, stamina: 6, experience: 7, age: 25),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgTSI.Should().BeApproximately(4500.0, precision: 0.01);
    }

    [Fact]
    public void GetTeamAverages_WithSinglePlayer_ReturnsAvgWageEqualToPlayerWage()
    {
        var players = new List<Player>
        {
            CreatePlayer(tsi: 4500, wage: 12000m, nationalityId: 1,
                         injuryWeeks: 0, redCard: false, yellowCards: 0,
                         form: 5, stamina: 6, experience: 7, age: 25),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgWage.Should().BeApproximately(12000.0, precision: 0.01);
    }

    [Fact]
    public void GetTeamAverages_WithSinglePlayer_ReturnsAvgEstimatedValueEqualToTSITimesTwenty5()
    {
        // EstimatedValue = TSI * 25 → 4500 * 25 = 112500
        var players = new List<Player>
        {
            CreatePlayer(tsi: 4500, wage: 12000m, nationalityId: 1,
                         injuryWeeks: 0, redCard: false, yellowCards: 0,
                         form: 5, stamina: 6, experience: 7, age: 25),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgEstimatedValue.Should().Be(112500m);
    }

    [Fact]
    public void GetTeamAverages_WithSinglePlayer_ReturnsAvgAgeEqualToPlayerAge()
    {
        var players = new List<Player>
        {
            CreatePlayer(tsi: 4500, wage: 12000m, nationalityId: 1,
                         injuryWeeks: 0, redCard: false, yellowCards: 0,
                         form: 5, stamina: 6, experience: 7, age: 27),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgAge.Should().BeApproximately(27.0, 0.01);
    }

    [Fact]
    public void GetTeamAverages_WithSinglePlayer_ReturnsAvgFormEqualToPlayerForm()
    {
        var players = new List<Player>
        {
            CreatePlayer(tsi: 4500, wage: 12000m, nationalityId: 1,
                         injuryWeeks: 0, redCard: false, yellowCards: 0,
                         form: 6, stamina: 6, experience: 7, age: 25),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgForm.Should().BeApproximately(6.0, 0.01);
    }

    [Fact]
    public void GetTeamAverages_WithSinglePlayer_ReturnsAvgStaminaEqualToPlayerStamina()
    {
        var players = new List<Player>
        {
            CreatePlayer(tsi: 4500, wage: 12000m, nationalityId: 1,
                         injuryWeeks: 0, redCard: false, yellowCards: 0,
                         form: 5, stamina: 7, experience: 7, age: 25),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgStamina.Should().BeApproximately(7.0, 0.01);
    }

    [Fact]
    public void GetTeamAverages_WithSinglePlayer_ReturnsAvgExperienceEqualToPlayerExperience()
    {
        var players = new List<Player>
        {
            CreatePlayer(tsi: 4500, wage: 12000m, nationalityId: 1,
                         injuryWeeks: 0, redCard: false, yellowCards: 0,
                         form: 5, stamina: 6, experience: 10, age: 25),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgExperience.Should().BeApproximately(10.0, 0.01);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GetTeamAverages — multiple players (arithmetic means)
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GetTeamAverages_WithMultiplePlayers_ReturnsArithmeticMeanOfTSI()
    {
        // (3000 + 6000) / 2 = 4500
        var players = new List<Player>
        {
            CreatePlayer(tsi: 3000, wage: 10000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 6000, wage: 14000m, nationalityId: 2, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 7, stamina: 7, experience: 9, age: 29),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgTSI.Should().BeApproximately(4500.0, precision: 0.01);
    }

    [Fact]
    public void GetTeamAverages_WithMultiplePlayers_ReturnsArithmeticMeanOfForm()
    {
        // (4 + 6 + 8) / 3 = 6.0
        var players = new List<Player>
        {
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 4, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 6, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 8, stamina: 5, experience: 5, age: 25),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgForm.Should().BeApproximately(6.0, precision: 0.01);
    }

    [Fact]
    public void GetTeamAverages_WithMultiplePlayers_ReturnsArithmeticMeanOfStamina()
    {
        // (3 + 6 + 9) / 3 = 6.0
        var players = new List<Player>
        {
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 3, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 6, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 9, experience: 5, age: 25),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgStamina.Should().BeApproximately(6.0, precision: 0.01);
    }

    [Fact]
    public void GetTeamAverages_WithMultiplePlayers_ReturnsArithmeticMeanOfExperience()
    {
        // (2 + 8 + 14) / 3 = 8.0
        var players = new List<Player>
        {
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 2,  age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 8,  age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 14, age: 25),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgExperience.Should().BeApproximately(8.0, precision: 0.01);
    }

    [Fact]
    public void GetTeamAverages_WithMultiplePlayers_ReturnsArithmeticMeanOfAge()
    {
        // (20 + 25 + 30) / 3 = 25.0
        var players = new List<Player>
        {
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 20),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 1000, wage: 5000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 30),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgAge.Should().BeApproximately(25.0, precision: 0.01);
    }

    [Fact]
    public void GetTeamAverages_WithMultiplePlayers_ReturnsArithmeticMeanOfWage()
    {
        // (10000 + 14000) / 2 = 12000
        var players = new List<Player>
        {
            CreatePlayer(tsi: 3000, wage: 10000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 6000, wage: 14000m, nationalityId: 2, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 7, stamina: 7, experience: 9, age: 29),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgWage.Should().BeApproximately(12000.0, 0.01);
    }

    [Fact]
    public void GetTeamAverages_WithMultiplePlayers_ReturnsArithmeticMeanOfEstimatedValue()
    {
        // TSI=2000 → EstimatedValue=50000; TSI=6000 → EstimatedValue=150000
        // (50000 + 150000) / 2 = 100000
        var players = new List<Player>
        {
            CreatePlayer(tsi: 2000, wage: 10000m, nationalityId: 1, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 5, stamina: 5, experience: 5, age: 25),
            CreatePlayer(tsi: 6000, wage: 14000m, nationalityId: 2, injuryWeeks: 0, redCard: false, yellowCards: 0, form: 7, stamina: 7, experience: 9, age: 29),
        };

        var result = _sut.GetTeamAverages(players);

        result.AvgEstimatedValue.Should().Be(100000m);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Null Guard Tests — RED until ArgumentNullException.ThrowIfNull is added
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GetTeamTotals_WithNullPlayers_ThrowsArgumentNullException()
    {
        // Without a null guard, LINQ throws NullReferenceException deep inside
        // the call stack rather than a clear ArgumentNullException at the entry point.
        // This test documents the required contract.
        Assert.Throws<ArgumentNullException>(() => _sut.GetTeamTotals(null!));
    }

    [Fact]
    public void GetTeamAverages_WithNullPlayers_ThrowsArgumentNullException()
    {
        // Without a null guard, players.Count on a null arg throws NullReferenceException
        // before the empty-list guard fires.
        // This test documents the required contract.
        Assert.Throws<ArgumentNullException>(() => _sut.GetTeamAverages(null!));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Overflow Tests — int*int multiplication risk for large TSI values
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void GetTeamTotals_WithHighTSIPlayer_ReturnsTotalEstimatedValueWithoutOverflow()
    {
        // TSI = 100_000 → EstimatedValue = 100_000 * 25 = 2_500_000
        // int.MaxValue = 2_147_483_647; 100_000 * 25 = 2_500_000 fits in int BUT
        // the multiplication p.TSI * EstimatedValueMultiplier is int*int before
        // decimal assignment, so larger values would silently overflow.
        // Casting to (decimal) before multiplication prevents this.
        var players = new List<Player>
        {
            CreatePlayer(tsi: 100_000, wage: 0m, nationalityId: 1,
                         injuryWeeks: 0, redCard: false, yellowCards: 0,
                         form: 5, stamina: 5, experience: 5, age: 25),
        };

        var result = _sut.GetTeamTotals(players);

        // 100_000 * 25 = 2_500_000 — must be exact decimal, not a truncated int
        result.TotalEstimatedValue.Should().Be(2_500_000m);
    }

    [Fact]
    public void GetTeamAverages_WithHighTSIPlayer_ReturnsAvgEstimatedValueWithoutOverflow()
    {
        // Same TSI=100_000 single-player squad: average equals the one player's value.
        // When AvgEstimatedValue is changed from double to decimal this test validates
        // the decimal precision is preserved end-to-end.
        var players = new List<Player>
        {
            CreatePlayer(tsi: 100_000, wage: 0m, nationalityId: 1,
                         injuryWeeks: 0, redCard: false, yellowCards: 0,
                         form: 5, stamina: 5, experience: 5, age: 25),
        };

        var result = _sut.GetTeamAverages(players);

        // 100_000 * 25 = 2_500_000 — must be exact decimal, not a double approximation.
        // Requires AvgEstimatedValue to be decimal (not double) for Be(decimal) to compile.
        result.AvgEstimatedValue.Should().Be(2_500_000m);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Type Consistency — TeamAverages.AvgEstimatedValue must be decimal
    // RED until TeamAverages.AvgEstimatedValue is changed from double to decimal
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void TeamAverages_AvgEstimatedValue_IsDecimalType()
    {
        // TotalEstimatedValue on TeamTotals is already decimal.
        // AvgEstimatedValue on TeamAverages must also be decimal to stay consistent.
        // This test fails while AvgEstimatedValue is double.
        var propertyType = typeof(TeamAverages)
            .GetProperty(nameof(TeamAverages.AvgEstimatedValue))!
            .PropertyType;

        propertyType.Should().Be(typeof(decimal));
    }
}
