using Microsoft.AspNetCore.Mvc;
using Corkboard.Data.Services;

namespace Corkboard.Controllers;

/// <summary>
/// Controller for displaying and retrieving messages within channels.
/// </summary>
public class MessagesController : Controller
{
	private readonly IMessageService _messageService;

	/// <summary>
	/// Creates a new instance of <see cref="MessagesController"/>.
	/// </summary>
	/// <param name="messageService">Message service for data operations.</param>
	public MessagesController(IMessageService messageService)
	{
		_messageService = messageService;
	}

	/// <summary>
	/// Displays the chat interface with an optional channel identifier.
	/// GET /chat/{id?}
	/// </summary>
	/// <param name="id">Optional channel ID to load.</param>
	/// <returns>View with chat interface.</returns>
	[HttpGet("chat/{id?}")]
	public IActionResult Index(string? id = null)
	{
		// Expose the optional id to the view for later use.
		ViewData["ChatId"] = id;
		return View();
	}

	/// <summary>
	/// Returns all messages for a specific channel as JSON for API consumption.
	/// GET /api/messages/{channelId}
	/// </summary>
	/// <param name="channelId">The channel ID to retrieve messages from.</param>
	/// <returns>JSON array of message DTOs.</returns>
	[HttpGet("api/messages/{channelId}")]
	public async Task<IActionResult> GetMessages(int channelId)
	{
		var messages = await _messageService.GetMessagesForChannelAsync(channelId);
		return Json(messages);
	}
}
