using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models;

/// <summary>
/// Represents a message posted in a channel by a user.
/// </summary>
public class Message
{
	/// <summary>
	/// Primary key for the message.
	/// </summary>
	[Key]
	public int Id { get; set; }

	/// <summary>
	/// Foreign key to the <see cref="Channel"/> this message was posted in.
	/// </summary>
	[Required]
	public int ChannelId { get; set; }

	/// <summary>
	/// Navigation property for the channel containing this message.
	/// </summary>
	[ForeignKey(nameof(ChannelId))]
	public Channel Channel { get; set; } = null!;

	/// <summary>
	/// Foreign key to the <see cref="UserAccount"/> who sent the message.
	/// </summary>
	[Required]
	public required string SenderId { get; set; }

	/// <summary>
	/// Navigation property for the message sender.
	/// </summary>
	[ForeignKey(nameof(SenderId))]
	public UserAccount Sender { get; set; } = null!;

	/// <summary>
	/// The textual content of the message.
	/// </summary>
	[Required]
	[MaxLength(5000)]
	public string MessageContent { get; set; } = string.Empty;

	/// <summary>
	/// UTC timestamp when the message was created. Defaults to the time the instance is created.
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
