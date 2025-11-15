using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models;

/// <summary>
/// Represents a server (or guild) which contains channels and members.
/// </summary>
public class Server
{
	/// <summary>
	/// Primary key for the server.
	/// </summary>
	[Key]
	public int Id { get; set; }

	/// <summary>
	/// Display name of the server. Defaults to "My Server" when a new instance is created.
	/// </summary>
	[Required]
	[MaxLength(100)]
	public string Name { get; set; } = "My Server";

	/// <summary>
	/// Optional URL to an icon image for the server.
	/// </summary>
	[Url]
	[MaxLength(2048)]
	public string? IconUrl { get; set; }

	/// <summary>
	/// Optional textual description of the server.
	/// </summary>
	[MaxLength(1000)]
	public string? Description { get; set; }

	/// <summary>
	/// Foreign key referencing the <see cref="UserAccount"/> that owns the server.
	/// </summary>
	[Required]
	public required string OwnerId { get; set; }

	/// <summary>
	/// Navigation property for the server owner.
	/// </summary>
	[ForeignKey(nameof(OwnerId))]
	public UserAccount Owner { get; set; } = null!;

	/// <summary>
	/// UTC timestamp when the server was created. Set at construction time by default.
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	/// Collection of membership records for users that belong to this server.
	/// </summary>
	public ICollection<ServerMember> Members { get; set; } = new List<ServerMember>();

	public int MemberCount => Members.Count;

	/// <summary>
	/// Collection of channels that belong to this server.
	/// </summary>
	public ICollection<Channel> Channels { get; set; } = new List<Channel>();
}
