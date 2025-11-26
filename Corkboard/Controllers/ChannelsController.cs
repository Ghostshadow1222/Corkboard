using System.ComponentModel.DataAnnotations;
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
[Route("[controller]")]
public class ChannelsController : BaseController
{
	private readonly IChannelService _channelService;

	/// <summary>
	/// Creates a new instance of <see cref="ChannelsController"/>.
	/// </summary>
	/// <param name="channelService">Channel service for data operations.</param>
	/// <param name="userManager">User manager for identity operations.</param>
	public ChannelsController(IChannelService channelService, UserManager<UserAccount> userManager)
		: base(userManager)
	{
		_channelService = channelService;
	}

	/// <summary>
	/// Creates a new channel in a server (API endpoint).
	/// POST /Channels/Create/{serverId}
	/// </summary>
	[HttpPost("Create/{serverId}")]
	[Authorize(Policy = "ServerModerator")]
	[IgnoreAntiforgeryToken]
	public async Task<IActionResult> Create(int serverId, [FromBody] CreateChannelRequest request)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		if (serverId != request.ServerId)
		{
			return BadRequest();
		}

		string userId = CurrentUserId!;

		Channel channel = new Channel
		{
			Name = request.Name,
			ServerId = request.ServerId
		};

		channel = await _channelService.CreateChannelAsync(channel);

		return Ok(new { id = channel.Id, name = channel.Name, serverId = channel.ServerId });
	}

	/// <summary>
	/// Request model for creating a channel.
	/// </summary>
	public class CreateChannelRequest
	{
		/// <summary>
		/// Server ID where the channel will be created.
		/// </summary>
		public int ServerId { get; set; }

		/// <summary>
		/// Name of the channel.
		/// </summary>
		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;
	}
}
