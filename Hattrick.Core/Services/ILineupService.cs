using Hattrick.Core.Models;

namespace Hattrick.Core.Services;

/// <summary>
/// Service for managing team lineups including validation.
/// Phase 2 Sprint 2, Quartet 3.
/// </summary>
public interface ILineupService
{
    /// <summary>
    /// Validates a team lineup against the squad.
    /// </summary>
    /// <param name="lineup">The lineup to validate.</param>
    /// <param name="squad">The squad of available players.</param>
    /// <returns>Validation result with IsValid flag and list of errors.</returns>
    LineupValidationResult ValidateLineup(TeamLineup lineup, IReadOnlyList<Player> squad);

    /// <summary>
    /// Suggests an optimal lineup for a team using a 4-4-2 formation.
    /// </summary>
    /// <param name="teamId">The team ID for the lineup.</param>
    /// <param name="squad">The squad of available players.</param>
    /// <returns>A suggested TeamLineup with optimal player placements.</returns>
    /// <exception cref="ArgumentNullException">Thrown when squad is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when fewer than 11 available players.</exception>
    TeamLineup SuggestLineup(Guid teamId, IReadOnlyList<Player> squad);
}
