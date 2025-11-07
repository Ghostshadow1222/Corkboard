using Microsoft.AspNetCore.Mvc;
using Corkboard.Data.Services;

namespace Corkboard.Controllers;
public class MessagesController : Controller
{
	private readonly IMessageService _messageService;

	public MessagesController(IMessageService messageService)
	{
		_messageService = messageService;
	}

	[HttpGet("chat/{id?}")]
	public IActionResult Index(string? id = null)
	{
		// Expose the optional id to the view for later use.
		ViewData["ChatId"] = id;
		return View();
	}

	// GET /api/messages/{channelId} -> Return all messages in a channel (JSON)
	[HttpGet("api/messages/{channelId}")]
	public async Task<IActionResult> GetMessages(int channelId)
	{
		var messages = await _messageService.GetMessagesForChannelAsync(channelId);
		return Json(messages);
	}
}
