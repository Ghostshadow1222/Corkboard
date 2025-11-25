using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Corkboard.Models;

/// <summary>
/// Represents an invitation to join a server, with optional expiration and usage limits.
/// </summary>
[Index(nameof(InviteCode), IsUnique = true)]
public class ServerInvite : IValidatableObject
{
	/// <summary>
	/// Primary key for the server invite record.
	/// </summary>
	[Key]
	public int Id { get; set; }

	/// <summary>
	/// Foreign key to the <see cref="Server"/> this invite is for.
	/// </summary>
	[Required]
	public int ServerId { get; set; }

	/// <summary>
	/// Navigation property for the server.
	/// </summary>
	[ForeignKey(nameof(ServerId))]
	public Server Server { get; set; } = null!;

	/// <summary>
	/// Foreign key to the <see cref="UserAccount"/> who created this invite.
	/// </summary>
	public string CreatedById { get; set; } = null!;

	/// <summary>
	/// Navigation property for the user who created the invite.
	/// </summary>
	[ForeignKey(nameof(CreatedById))]
	public UserAccount CreatedBy { get; set; } = null!;

	/// <summary>
	/// Foreign key to the <see cref="UserAccount"/> who is invited to the server (optional).
	/// </summary>
	public string? InvitedUserId { get; set; }

	/// <summary>
	/// Navigation property for the specific user this invite is intended for, if any.
	/// </summary>
	[ForeignKey(nameof(InvitedUserId))]
	public UserAccount? InvitedUser { get; set; }

	/// <summary>
	/// A randomly generated code used to join the server via this invite.
	/// The code is a string of capital letters and digits.
	/// </summary>
	public string InviteCode { get; set; } = null!;

	/// <summary>
	/// UTC timestamp when the invite was created. Defaults to the time the instance is created.
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	/// Optional UTC timestamp when this invite expires and can no longer be used.
	/// </summary>
	public DateTime? ExpiresAt { get; set; }

	/// <summary>
	/// Whether this invite can only be used once. Defaults to true.
	/// </summary>
	public bool OneTimeUse { get; set; } = true;

	/// <summary>
	/// Whether this invite has been marked as used (typically for one-time use invites).
	/// </summary>
	public bool IsUsed { get; set; } = false;

	/// <summary>
	/// The number of times this invite has been successfully redeemed.
	/// </summary>
	public int TimesUsed { get; set; } = 0;

	/// <summary>
	/// Collection of server memberships that were created using this invite.
	/// </summary>
	public List<ServerMember> CreatedMemberships { get; set; } = new List<ServerMember>();

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (InvitedUserId != null && OneTimeUse)
		{
			yield return new ValidationResult("Invites for specific users cannot be one-time use.", new[] { nameof(InvitedUserId), nameof(OneTimeUse) });
		}

		if (InvitedUserId != null && InvitedUserId == CreatedById)
		{
			yield return new ValidationResult("Users cannot invite themselves.", new[] { nameof(InvitedUserId), nameof(CreatedById) });
		}

		if (OneTimeUse && TimesUsed > 1)
		{
			yield return new ValidationResult("One-time use invites cannot have been used more than once.", new[] { nameof(TimesUsed) });
		}

		if (ExpiresAt != null && ExpiresAt <= CreatedAt)
		{
			yield return new ValidationResult("Expiration date must be after the creation date.", new[] { nameof(ExpiresAt) });
		}
	}
}