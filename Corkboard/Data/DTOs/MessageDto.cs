namespace Corkboard.Data.DTOs;

/// <summary>
/// Data Transfer Object representing a message returned by the API.
/// Contains only the fields required by clients to display a message.
/// </summary>
public class MessageDto
{
	/// <summary>
	/// The textual content of the message.
	/// </summary>
	public string Text { get; set; } = string.Empty;

	/// <summary>
	/// The sender's username.
	/// </summary>
	public string Sender { get; set; } = string.Empty;

	/// <summary>
	/// Optional URL to the sender's profile image.
	/// </summary>
	public string? ImageUrl { get; set; }

	/// <summary>
	/// UTC timestamp when the message was created.
	/// </summary>
	public DateTime Timestamp { get; set; }
}
