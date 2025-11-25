using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Corkboard.Data.Services;
using Corkboard.Models;

namespace Corkboard.Controllers;

/// <summary>
/// Controller for managing and displaying channels within a server.
/// </summary>
[Authorize]
public class ChannelsController : Controller
{
	private readonly IChannelService _channelService;
	private readonly IServerService _serverService;
	private readonly UserManager<UserAccount> _userManager;

	/// <summary>
	/// Creates a new instance of <see cref="ChannelsController"/>.
	/// </summary>
	/// <param name="channelService">Channel service for data operations.</param>
	/// <param name="serverService">Server service for authorization checks.</param>
	/// <param name="userManager">User manager for identity operations.</param>
	public ChannelsController(IChannelService channelService, IServerService serverService, UserManager<UserAccount> userManager)
	{
		_channelService = channelService;
		_serverService = serverService;
		_userManager = userManager;
	}

	/// <summary>
	/// Displays a list of all channels in a specific server.
	/// GET /Channels/Index/{serverId}
	/// </summary>
	/// <param name="serverId">The server ID to retrieve channels for.</param>
	/// <returns>View with list of channels.</returns>
	public async Task<IActionResult> Index(int serverId)
	{
		string userId = _userManager.GetUserId(User)!;

		if (!await _serverService.IsUserMemberOfServerAsync(serverId, userId))
		{
			return Unauthorized();
		}

		ViewBag.ServerId = serverId;
		var channels = await _channelService.GetChannelsForServerAsync(serverId);

		return View(channels);
	}

	/// <summary>
	/// Opens a specific channel for viewing and chatting.
	/// GET /Channels/Open/{id}
	/// </summary>
	/// <param name="id">The channel ID to open.</param>
	/// <returns>View with channel details, or NotFound if channel doesn't exist.</returns>
	[HttpGet]
	public async Task<IActionResult> Open(int id)
	{
		string userId = _userManager.GetUserId(User)!;

		var channel = await _channelService.GetChannelAsync(id);
		if (channel == null)
		{
			return NotFound();
		}

		if (!await _serverService.IsUserMemberOfServerAsync(channel.ServerId, userId))
		{
			return Unauthorized();
		}

		ViewBag.ServerId = channel.ServerId;
		return View(channel);
	}
}
