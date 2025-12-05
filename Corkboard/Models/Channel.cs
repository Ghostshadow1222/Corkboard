using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models;

/// <summary>
/// Represents a text channel within a server where messages can be posted.
/// </summary>
public class Channel
{
	/// <summary>
	/// Primary key for the channel.
	/// </summary>
	[Key]
	public int Id { get; set; }

	/// <summary>
	/// Display name of the channel.
	/// </summary>
	[Required]
	[MaxLength(100)]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Foreign key to the <see cref="Server"/> this channel belongs to.
	/// </summary>
	[Required]
	public int ServerId { get; set; }

	/// <summary>
	/// Navigation property for the server containing this channel.
	/// </summary>
	[ForeignKey(nameof(ServerId))]
	public Server Server { get; set; } = null!;

	/// <summary>
	/// UTC timestamp when the channel was created. Defaults to instance creation time.
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	/// Messages posted in this channel.
	/// </summary>
	public ICollection<Message> Messages { get; set; } = new List<Message>();
}
