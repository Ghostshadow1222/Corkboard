using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Corkboard.Data.Services;
using Corkboard.Data.DTOs;
using Corkboard.Models;

namespace Corkboard.Controllers;

/// <summary>
/// Controller for displaying and retrieving messages within channels.
/// </summary>
[Authorize]
public class MessagesController : BaseController
{
	private readonly IMessageService _messageService;

	/// <summary>
	/// Creates a new instance of <see cref="MessagesController"/>.
	/// </summary>
	/// <param name="messageService">Message service for data operations.</param>
	/// <param name="userManager">User manager for identity operations.</param>
	public MessagesController(IMessageService messageService, UserManager<UserAccount> userManager)
		: base(userManager)
	{
		_messageService = messageService;
	}

	/// <summary>
	/// Returns all messages for a specific channel as JSON for API consumption.
	/// GET /api/messages/{channelId}
	/// </summary>
	/// <param name="channelId">The channel ID to retrieve messages from.</param>
	/// <returns>JSON array of message DTOs.</returns>
	[HttpGet("api/messages/{serverId}/{channelId}")]
	[Authorize(Policy = "ServerMember")]
	public async Task<IActionResult> GetMessages(int serverId, int channelId)
	{
		List<MessageDto> messages = await _messageService.GetMessagesForChannelAsync(channelId);

		return Json(messages);
	}
}
