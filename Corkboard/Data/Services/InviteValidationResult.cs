using System.Collections.Generic;

namespace Corkboard.Data.Services;

/// <summary>
/// Encapsulates the result of validating invite creation authorization and business rules.
/// </summary>
public class InviteValidationResult
{
	/// <summary>
	/// Indicates whether all validation checks passed.
	/// </summary>
	public bool IsValid { get; set; } = true;

	/// <summary>
	/// Field-level validation errors (maps to ModelState keys and error messages).
	/// </summary>
	public Dictionary<string, string> FieldErrors { get; } = new Dictionary<string, string>();

	/// <summary>
	/// Indicates the requested server or resource was not found.
	/// </summary>
	public bool NotFound { get; set; } = false;

	/// <summary>
	/// Indicates the current user is not authorized to create invites for this server.
	/// </summary>
	public bool Unauthorized { get; set; } = false;

	/// <summary>
	/// If a username was provided and validated, contains the invited user's ID.
	/// </summary>
	public string? InvitedUserId { get; set; }

	/// <summary>
	/// The validated expiration date/time, or null if no expiration was requested.
	/// </summary>
	public DateTime? ValidatedExpiresAt { get; set; }
}
