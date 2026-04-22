using Hattrick.Core.Models;

namespace Hattrick.Core.Services;

/// <summary>
/// Service for generating players with randomized but position-appropriate attributes.
/// Uses IRandomProvider for all randomness to enable testability.
/// </summary>
public class PlayerGenerationService : IPlayerGenerationService
{
    private readonly IRandomProvider _random;

    // Age bounds (inclusive)
    private const int MinAge = 17;
    private const int MaxAge = 32;
    private const int AgeMaxExclusive = MaxAge + 1;

    // Form bounds (inclusive)
    private const int MinForm = 5;
    private const int MaxForm = 8;
    private const int FormMaxExclusive = MaxForm + 1;

    // Stamina bounds (inclusive)
    private const int MinStamina = 5;
    private const int MaxStamina = 8;
    private const int StaminaMaxExclusive = MaxStamina + 1;

    // Experience bounds (inclusive)
    private const int MinExperience = 1;
    private const int MaxExperience = 5;
    private const int ExperienceMaxExclusive = MaxExperience + 1;

    // Skill bounds for primary skills
    private const int PrimarySkillMin = 6;
    private const int PrimarySkillMaxExclusive = 13;

    // Keeper has higher primary skill
    private const int KeeperSkillMin = 8;
    private const int KeeperSkillMaxExclusive = 16;

    // Secondary skill bounds
    private const int SecondarySkillMin = 5;
    private const int SecondarySkillMaxExclusive = 11;

    // Low skill bounds (for non-relevant skills)
    private const int LowSkillMin = 1;
    private const int LowSkillMaxExclusive = 4;

    // SetPieces skill bounds
    private const int SetPiecesMin = 1;
    private const int SetPiecesMaxExclusive = 11;

    // Stamina skill bounds (different from player Stamina attribute)
    private const int StaminaSkillMin = 3;
    private const int StaminaSkillMaxExclusive = 8;

    /// <summary>
    /// Fictional first names for generated players.
    /// </summary>
    private static readonly string[] FirstNames =
    [
        "Viktor", "Alexei", "Nikolai", "Dmitri", "Yuri",
        "Marco", "Luca", "Giovanni", "Franco", "Pietro",
        "Hans", "Klaus", "Fritz", "Otto", "Werner",
        "Jean", "Pierre", "Louis", "Henri", "Marcel"
    ];

    /// <summary>
    /// Fictional last names for generated players.
    /// </summary>
    private static readonly string[] LastNames =
    [
        "Volkov", "Petrov", "Kozlov", "Orloff", "Ivanov",
        "Rossi", "Bianchi", "Conti", "Moretti", "Romano",
        "Muller", "Schmidt", "Fischer", "Weber", "Bauer",
        "Bernard", "Dubois", "Moreau", "Laurent", "Simon"
    ];

    /// <summary>
    /// Initializes a new instance of <see cref="PlayerGenerationService"/>.
    /// </summary>
    /// <param name="random">Random provider for generating random values.</param>
    /// <exception cref="ArgumentNullException">Thrown when random is null.</exception>
    public PlayerGenerationService(IRandomProvider random)
    {
        ArgumentNullException.ThrowIfNull(random);
        _random = random;
    }

    /// <inheritdoc />
    public Player GeneratePlayer(Position position)
    {
        var player = new Player
        {
            Name = GenerateName(),
            Age = _random.Next(MinAge, AgeMaxExclusive),
            Form = _random.Next(MinForm, FormMaxExclusive),
            Stamina = _random.Next(MinStamina, StaminaMaxExclusive),
            Experience = _random.Next(MinExperience, ExperienceMaxExclusive),
            BestPosition = position,
            Specialty = GenerateSpecialty(),
            Skills = GenerateSkills(position)
        };

        return player;
    }

    private string GenerateName()
    {
        var firstName = FirstNames[_random.Next(FirstNames.Length)];
        var lastName = LastNames[_random.Next(LastNames.Length)];
        return $"{firstName} {lastName}";
    }

    private Specialty GenerateSpecialty()
    {
        var specialties = Enum.GetValues<Specialty>();
        return specialties[_random.Next(specialties.Length)];
    }

    private Dictionary<SkillType, double> GenerateSkills(Position position)
    {
        var skills = new Dictionary<SkillType, double>();
        var skillConfig = GetSkillConfiguration(position);

        foreach (var skillType in Enum.GetValues<SkillType>())
        {
            var (min, maxExclusive) = skillConfig[skillType];
            skills[skillType] = _random.Next(min, maxExclusive);
        }

        return skills;
    }

    private static Dictionary<SkillType, (int Min, int MaxExclusive)> GetSkillConfiguration(
        Position position)
    {
        return position switch
        {
            Position.Keeper => GetKeeperSkillConfig(),
            Position.CentralDefender => GetCentralDefenderSkillConfig(),
            Position.WingBack => GetWingBackSkillConfig(),
            Position.InnerMidfielder => GetInnerMidfielderSkillConfig(),
            Position.Winger => GetWingerSkillConfig(),
            Position.Forward => GetForwardSkillConfig(),
            _ => GetDefaultSkillConfig()
        };
    }

    private static Dictionary<SkillType, (int Min, int MaxExclusive)> GetKeeperSkillConfig()
    {
        return new Dictionary<SkillType, (int, int)>
        {
            [SkillType.Keeper] = (KeeperSkillMin, KeeperSkillMaxExclusive),
            [SkillType.Defending] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Playmaking] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Winger] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Passing] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Scoring] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.SetPieces] = (SetPiecesMin, SetPiecesMaxExclusive),
            [SkillType.Stamina] = (StaminaSkillMin, StaminaSkillMaxExclusive)
        };
    }

    private static Dictionary<SkillType, (int Min, int MaxExclusive)> GetCentralDefenderSkillConfig()
    {
        return new Dictionary<SkillType, (int, int)>
        {
            [SkillType.Keeper] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Defending] = (PrimarySkillMin, PrimarySkillMaxExclusive),
            [SkillType.Playmaking] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Winger] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Passing] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Scoring] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.SetPieces] = (SetPiecesMin, SetPiecesMaxExclusive),
            [SkillType.Stamina] = (StaminaSkillMin, StaminaSkillMaxExclusive)
        };
    }

    private static Dictionary<SkillType, (int Min, int MaxExclusive)> GetWingBackSkillConfig()
    {
        return new Dictionary<SkillType, (int, int)>
        {
            [SkillType.Keeper] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Defending] = (SecondarySkillMin, SecondarySkillMaxExclusive),
            [SkillType.Playmaking] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Winger] = (SecondarySkillMin, SecondarySkillMaxExclusive),
            [SkillType.Passing] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Scoring] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.SetPieces] = (SetPiecesMin, SetPiecesMaxExclusive),
            [SkillType.Stamina] = (StaminaSkillMin, StaminaSkillMaxExclusive)
        };
    }

    private static Dictionary<SkillType, (int Min, int MaxExclusive)> GetInnerMidfielderSkillConfig()
    {
        return new Dictionary<SkillType, (int, int)>
        {
            [SkillType.Keeper] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Defending] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Playmaking] = (PrimarySkillMin, PrimarySkillMaxExclusive),
            [SkillType.Winger] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Passing] = (PrimarySkillMin, PrimarySkillMaxExclusive),
            [SkillType.Scoring] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.SetPieces] = (SetPiecesMin, SetPiecesMaxExclusive),
            [SkillType.Stamina] = (StaminaSkillMin, StaminaSkillMaxExclusive)
        };
    }

    private static Dictionary<SkillType, (int Min, int MaxExclusive)> GetWingerSkillConfig()
    {
        return new Dictionary<SkillType, (int, int)>
        {
            [SkillType.Keeper] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Defending] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Playmaking] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Winger] = (PrimarySkillMin, PrimarySkillMaxExclusive),
            [SkillType.Passing] = (SecondarySkillMin, SecondarySkillMaxExclusive),
            [SkillType.Scoring] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.SetPieces] = (SetPiecesMin, SetPiecesMaxExclusive),
            [SkillType.Stamina] = (StaminaSkillMin, StaminaSkillMaxExclusive)
        };
    }

    private static Dictionary<SkillType, (int Min, int MaxExclusive)> GetForwardSkillConfig()
    {
        return new Dictionary<SkillType, (int, int)>
        {
            [SkillType.Keeper] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Defending] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Playmaking] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Winger] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Passing] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Scoring] = (PrimarySkillMin, PrimarySkillMaxExclusive),
            [SkillType.SetPieces] = (SetPiecesMin, SetPiecesMaxExclusive),
            [SkillType.Stamina] = (StaminaSkillMin, StaminaSkillMaxExclusive)
        };
    }

    private static Dictionary<SkillType, (int Min, int MaxExclusive)> GetDefaultSkillConfig()
    {
        return new Dictionary<SkillType, (int, int)>
        {
            [SkillType.Keeper] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Defending] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Playmaking] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Winger] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Passing] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.Scoring] = (LowSkillMin, LowSkillMaxExclusive),
            [SkillType.SetPieces] = (SetPiecesMin, SetPiecesMaxExclusive),
            [SkillType.Stamina] = (StaminaSkillMin, StaminaSkillMaxExclusive)
        };
    }
}
