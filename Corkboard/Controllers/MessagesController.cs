using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Corkboard.Data.Services;
using Corkboard.Models;
using Corkboard.Data.DTOs;

namespace Corkboard.Controllers;

/// <summary>
/// Controller for displaying and retrieving messages within channels.
/// </summary>
[Authorize]
public class MessagesController : Controller
{
	private readonly IMessageService _messageService;
	private readonly IChannelService _channelService;
	private readonly IServerService _serverService;
	private readonly UserManager<UserAccount> _userManager;

	/// <summary>
	/// Creates a new instance of <see cref="MessagesController"/>.
	/// </summary>
	/// <param name="messageService">Message service for data operations.</param>
	/// <param name="channelService">Channel service for retrieving channel information.</param>
	/// <param name="serverService">Server service for authorization checks.</param>
	/// <param name="userManager">User manager for identity operations.</param>
	public MessagesController(IMessageService messageService, IChannelService channelService, IServerService serverService, UserManager<UserAccount> userManager)
	{
		_messageService = messageService;
		_channelService = channelService;
		_serverService = serverService;
		_userManager = userManager;
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
		string userId = _userManager.GetUserId(User)!;

		Channel? channel = await _channelService.GetChannelAsync(channelId);
		if (channel == null)
		{
			return NotFound();
		}

		if (!await _serverService.IsUserMemberOfServerAsync(channel.ServerId, userId))
		{
			return Unauthorized();
		}

		List<MessageDto> messages = await _messageService.GetMessagesForChannelAsync(channelId);

		return Json(messages);
	}
}
