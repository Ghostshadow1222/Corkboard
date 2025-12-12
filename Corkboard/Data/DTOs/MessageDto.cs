namespace Corkboard.Data.DTOs;

/// <summary>
/// Data Transfer Object representing a message for client consumption.
/// Includes only the fields required to render chat items.
/// </summary>
public class MessageDto
{
	/// <summary>
	/// Unique identifier of the message.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// Text content of the message.
	/// </summary>
	public string Text { get; set; } = null!;

	/// <summary>
	/// Username of the message sender.
	/// </summary>
	public string SenderUsername { get; set; } = null!;

	/// <summary>
	/// UTC timestamp when the message was created.
	/// </summary>
	public DateTime Timestamp { get; set; }
}
