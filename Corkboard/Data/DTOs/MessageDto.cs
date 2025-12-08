namespace Corkboard.Data.DTOs;

/// <summary>
/// Data Transfer Object representing a message returned by the API.
/// Contains only the fields required by clients to display a message.
/// </summary>
public class MessageDto
{
	/// <summary>
	/// The message ID.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// The textual content of the message.
	/// </summary>
	public string Text { get; set; } = null!;

	/// <summary>
	/// The sender's username.
	/// </summary>
	public string SenderUsername { get; set; } = null!;

	/// <summary>
	/// UTC timestamp when the message was created.
	/// </summary>
	public DateTime Timestamp { get; set; }
}
