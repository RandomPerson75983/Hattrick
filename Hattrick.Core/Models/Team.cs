namespace Hattrick.Core.Models;

/// <summary>
/// Represents a football team in the game.
/// Mutable model class since team state changes during gameplay
/// (budget, fans, team spirit, confidence, staff levels, etc.).
/// </summary>
public class Team
{
    public const int MinCoachLevel = 1;
    public const int MaxCoachLevel = 8;
    public const int MaxAssistantCoachLevel = 10;
    public const int MaxStaffLevel = 5;

    /// <summary>
    /// Unique identifier for the team. Auto-generated on construction.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The team's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Whether this team is controlled by a human player (true) or AI (false).
    /// </summary>
    public bool IsHumanControlled { get; set; }

    /// <summary>
    /// Current team budget in currency units. Decimal for precise financial tracking.
    /// </summary>
    public decimal Budget { get; set; }

    /// <summary>
    /// Total number of fans supporting the team.
    /// </summary>
    public int Fans { get; set; }

    /// <summary>
    /// Size of the fan club membership.
    /// </summary>
    public int FanClubSize { get; set; }

    /// <summary>
    /// Team spirit level (0.0-10.0). Affects match performance.
    /// </summary>
    public double TeamSpirit { get; set; }

    /// <summary>
    /// Team confidence level (0.0-10.0). Affects match performance.
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// The coaching style used by the team's coach.
    /// Defaults to Offensive (the zero-value enum member).
    /// </summary>
    public CoachType CoachType { get; set; }

    /// <summary>
    /// Coach skill level (1-8). Every team must have a coach, minimum level 1.
    /// </summary>
    public int CoachLevel { get; set; } = MinCoachLevel;

    /// <summary>
    /// Assistant coach skill level (0-10). Zero means none hired.
    /// </summary>
    public int AssistantCoachLevel { get; set; }

    /// <summary>
    /// Team doctor skill level (0-5). Zero means none hired.
    /// </summary>
    public int DoctorLevel { get; set; }

    /// <summary>
    /// Spokesperson skill level (0-5). Zero means none hired.
    /// </summary>
    public int SpokespersonLevel { get; set; }

    /// <summary>
    /// Financial director skill level (0-5). Zero means none hired.
    /// </summary>
    public int FinancialDirectorLevel { get; set; }
}
