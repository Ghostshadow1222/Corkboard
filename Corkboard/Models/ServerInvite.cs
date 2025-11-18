using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models;

public class ServerInvite
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

	public string CreatorId { get; set; } = null!;

	[ForeignKey(nameof(CreatorId))]
	public UserAccount Creator { get; set; } = null!;

	/// <summary>
	/// Foreign key to the <see cref="UserAccount"/> who is invited to the server (optional).
	/// </summary>
	public string? InvitedUserId { get; set; }

	[ForeignKey(nameof(InvitedUserId))]
	public UserAccount? InvitedUser { get; set; }

	/// <summary>
	/// A randomly generated code used to join the server via this invite.
	/// The code is a string of Capital letters and digits.
	/// </summary>
	public string Code { get; set; } = null!;

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);

	public bool OneTimeUse { get; set; } = true;

	public bool IsUsed { get; set; } = false;

	public int TimesUsed { get; set; } = 0;
}
