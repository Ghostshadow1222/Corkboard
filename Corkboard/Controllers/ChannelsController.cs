using Microsoft.AspNetCore.Mvc;
using Corkboard.Data.Services;

namespace Corkboard.Controllers;

public class ChannelsController : Controller
{
	private readonly IChannelService _channelService;

	public ChannelsController(IChannelService channelService)
	{
		_channelService = channelService;
	}

	// GET /Channels/Index/{serverId} -> List all channels in a server
	public async Task<IActionResult> Index(int serverId)
	{
		ViewBag.ServerId = serverId;
		var channels = await _channelService.GetChannelsForServerAsync(serverId);

		return View(channels);
	}

	// GET /Channels/Open/{id} -> Display the chat view for a channel
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
