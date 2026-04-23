namespace Hattrick.Core.Services;

/// <summary>
/// Result of validating a team lineup.
/// Phase 2 Sprint 2, Quartet 3.
/// </summary>
/// <param name="IsValid">True if the lineup passes all validation rules.</param>
/// <param name="Errors">List of validation error messages. Empty if valid.</param>
public record LineupValidationResult(bool IsValid, IReadOnlyList<string> Errors);
