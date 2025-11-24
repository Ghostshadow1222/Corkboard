using Microsoft.AspNetCore.Mvc;
using Corkboard.Data.Services;

namespace Corkboard.Controllers;

/// <summary>
/// Controller for managing and displaying channels within a server.
/// </summary>
public class ChannelsController : Controller
{
	private readonly IChannelService _channelService;

	/// <summary>
	/// Creates a new instance of <see cref="ChannelsController"/>.
	/// </summary>
	/// <param name="channelService">Channel service for data operations.</param>
	public ChannelsController(IChannelService channelService)
	{
		_channelService = channelService;
	}

	/// <summary>
	/// Displays a list of all channels in a specific server.
	/// GET /Channels/Index/{serverId}
	/// </summary>
	/// <param name="serverId">The server ID to retrieve channels for.</param>
	/// <returns>View with list of channels.</returns>
	public async Task<IActionResult> Index(int serverId)
	{
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
		var channel = await _channelService.GetChannelAsync(id);
		if (channel == null)
		{
			return NotFound();
		}

		ViewBag.ServerId = channel.ServerId;
		return View(channel);
	}
}
