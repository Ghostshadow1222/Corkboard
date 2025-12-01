using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models.ViewModels.InvitesController;

/// <summary>
/// View model for creating a new server invite with validation logic.
/// </summary>
[NotMapped]
public class ServerInviteViewModel : IValidatableObject
{
	/// <summary>
	/// Foreign key to the <see cref="Server"/> this invite is for.
	/// </summary>
	[Required]
	[Display(Name = "Server")]
	public int ServerId { get; set; }

	/// <summary>
	/// Username of the specific user to invite (optional). If not provided, the invite can be used by anyone.
	/// </summary>
	[Display(Name = "Username")]
	public string? Username { get; set; }

	/// <summary>
	/// The date and time when this invite should expire, if applicable.
	/// </summary>
	[Display(Name = "Expires At")]
	public DateTime? ExpiresAt { get; set; }

	/// <summary>
	/// Whether this invite should expire. If false, no expiration is set.
	/// </summary>
	[Display(Name = "Expires")]
	public bool Expires { get; set; } = false;

	/// <summary>
	/// Whether this invite can only be used once. Defaults to true.
	/// </summary>
	[Display(Name = "One Time Use")]
	public bool OneTimeUse { get; set; } = true;

	/// <summary>
	/// Validates that expiration settings are consistent and correct.
	/// </summary>
	/// <param name="validationContext">The validation context.</param>
	/// <returns>Collection of validation results, or empty if valid.</returns>
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		// If Expires is true, ExpiresAt must be provided and in the future
		if (Expires)
		{
			if (!ExpiresAt.HasValue)
			{
				yield return new ValidationResult("Expiration date/time must be provided when expiry is enabled.", new[] { nameof(ExpiresAt) });
			}
			else if (ExpiresAt.Value <= DateTime.UtcNow)
			{
				yield return new ValidationResult("Expiration must be a future date/time.", new[] { nameof(ExpiresAt) });
			}
		}
		else
		{
			// If Expires is false, ensure ExpiresAt is not set to avoid confusion
			if (ExpiresAt.HasValue)
			{
				yield return new ValidationResult("Clear the expiration date/time or enable expiry.", new[] { nameof(ExpiresAt) });
			}
		}

		// Invites for specific users cannot be multi-use
		if (!string.IsNullOrEmpty(Username) && !OneTimeUse)
		{
			yield return new ValidationResult("Invites for specific users cannot be multi-use.", new[] { nameof(Username), nameof(OneTimeUse) });
		}
	}
}
