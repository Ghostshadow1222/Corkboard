using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Corkboard.Models;

/// <summary>
/// Defines the role a user can have within a server.
/// </summary>
public enum RoleType
{
	/// <summary>
	/// Standard member with basic permissions.
	/// </summary>
	[Display(Name = "Member")]
	Member,

	/// <summary>
	/// Moderator with elevated permissions to manage the server.
	/// </summary>
	[Display(Name = "Moderator")]
	Moderator,

	/// <summary>
	/// Server owner with full administrative control.
	/// </summary>
	[Display(Name = "Owner")]
	Owner
}

/// <summary>
/// Represents a user's membership in a server, including their role and join time.
/// </summary>
[Index(nameof(ServerId), nameof(UserId), IsUnique = true)]
public class ServerMember
{
	/// <summary>
	/// Primary key for the membership record.
	/// </summary>
	[Key]
	public int Id { get; set; }

	/// <summary>
	/// Foreign key to the <see cref="ServerInvite"/> that was used to join, if applicable.
	/// </summary>
	public int? InviteId { get; set; }

	/// <summary>
	/// Navigation property for the invite that created this membership, if any.
	/// </summary>
	[ForeignKey(nameof(InviteId))]
	public ServerInvite? Invite { get; set; }

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
	/// Role of the member within the server. Defaults to standard member role.
	/// </summary>
	[Required]
	[MaxLength(50)]
	public RoleType Role { get; set; } = RoleType.Member;

	/// <summary>
	/// UTC timestamp when the user joined the server. Defaults to the time the instance is created.
	/// </summary>
	public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
