using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models;

public enum RoleType
{
	[Display(Name = "Member")]
	Member,

	[Display(Name = "Moderator")]
	Moderator,

	[Display(Name = "Owner")]
	Owner
}

/// <summary>
/// Represents a user's membership in a server, including their role and join time.
/// </summary>
public class ServerMember
{
	/// <summary>
	/// Primary key for the membership record.
	/// </summary>
	[Key]
	public int Id { get; set; }

	/// <summary>
	/// Foreign key to the <see cref="Server"/> this membership belongs to.
	/// </summary>
	[Required]
	public int ServerId { get; set; }

	/// <summary>
	/// Navigation property for the server.
	/// </summary>
	[ForeignKey(nameof(ServerId))]
	public Server Server { get; set; } = null!;

	/// <summary>
	/// Foreign key to the <see cref="UserAccount"/> who is the member.
	/// </summary>
	[Required]
	public required string UserId { get; set; }

	/// <summary>
	/// Navigation property for the user who is the member.
	/// </summary>
	[ForeignKey(nameof(UserId))]
	public UserAccount User { get; set; } = null!;

	/// <summary>
	/// Role of the member within the server (e.g. "member", "admin"). Defaults to "member".
	/// </summary>
	[Required]
	[MaxLength(50)]
	public RoleType Role { get; set; } = RoleType.Member;

	/// <summary>
	/// UTC timestamp when the user joined the server. Defaults to the time the instance is created.
	/// </summary>
	public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
