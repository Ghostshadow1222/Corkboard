using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Corkboard.Data.Services;
using Corkboard.Models;
using Corkboard.Models.ViewModels.ServersController;
using Corkboard.Models.ViewModels.InvitesController;
using Corkboard.Data.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Corkboard.Controllers;

/// <summary>
/// Controller for managing servers, including listing, creating, viewing details, joining, and managing invites.
/// </summary>
[Authorize]
public class ServersController : BaseController
{
    private readonly IMessageService _messageService;
	private readonly IInviteService _inviteService;
	private readonly IChannelService _channelService;

	#region Constructor

	/// <summary>
	/// Creates a new instance of <see cref="ServersController"/>.
	/// </summary>
	/// <param name="messageService">Message service for chat operations.</param>
	/// <param name="inviteService">Invite service for data operations.</param>
	/// <param name="channelService">Channel service for channel operations.</param>
	/// <param name="userManager">User manager for identity operations.</param>
	/// <param name="serverService">Server service for server operations.</param>
	public ServersController(IMessageService messageService, IInviteService inviteService, IChannelService channelService, UserManager<UserAccount> userManager, IServerService serverService)
		: base(userManager, serverService)
	{
        _messageService = messageService;
		_inviteService = inviteService;
		_channelService = channelService;
	}

	#endregion

	#region Servers

	/// <summary>
	/// Displays a list of all servers the current user is a member of.
	/// GET /Servers
	/// </summary>
	/// <returns>View with list of user's servers.</returns>
	[HttpGet("Servers/{serverId?}")]
	public async Task<IActionResult> Index(int? serverId)
	{
		if (serverId != null) 
		{
			return RedirectToAction(nameof(Channels), new { serverId = serverId} );
		}

		string userId = CurrentUserId!;
		List<Server> servers = await _serverService.GetServersForUserAsync(userId);
		ViewBag.CurrentUserId = userId;

		return View(servers);
	}

	/// <summary>
	/// Displays the form for creating a new server.
	/// GET /Servers/Create
	/// </summary>
	/// <returns>View with server creation form.</returns>
	[HttpGet("Servers/Create")]
	public IActionResult Create()
	{
		// Return an empty view model to the Razor Page
		return View(new ServerViewModel());
	}

	/// <summary>
	/// Handles the submission of a new server, creating it and adding the user as owner.
	/// POST /Servers/Create
	/// </summary>
	/// <param name="model">The server view model with creation details.</param>
	/// <returns>Redirect to server details on success, or view with errors on failure.</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(ServerViewModel model)
	{
		string userId = CurrentUserId!;

		// Basic validation
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		Server server = new Server
		{
			Name = model.Name,
			IconUrl = model.IconUrl,
			Description = model.Description,
			PrivacyLevel = model.PrivacyLevel,
			OwnerId = userId
		};

		// Create the server
		server = await _serverService.CreateServerAsync(server, userId);

		return RedirectToAction(nameof(Channels), new { serverId = server.Id });
	}

	/// <summary>
	/// Displays the form for editing a server.
	/// GET /Servers/{serverId}/Edit
	/// </summary>
	/// <param name="serverId">The server ID to edit.</param>
	/// <returns>View with server edit form.</returns>
	[HttpGet("Servers/{serverId}/Edit")]
	[Authorize(Policy = "ServerOwner")]
	public async Task<IActionResult> Edit(int serverId)
	{
		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		ServerViewModel model = new ServerViewModel
		{
			Id = server.Id,
			Name = server.Name,
			Description = server.Description,
			IconUrl = server.IconUrl,
			PrivacyLevel = server.PrivacyLevel
		};

		return View(model);
	}

	/// <summary>
	/// Handles the submission of server edits.
	/// POST /Servers/{serverId}/Edit
	/// </summary>
	/// <param name="serverId">The server ID to edit.</param>
	/// <param name="model">The server form view model with updated details.</param>
	/// <returns>Redirect to server channels on success, or view with errors on failure.</returns>
	[HttpPost("Servers/{serverId}/Edit")]
	[ValidateAntiForgeryToken]
	[Authorize(Policy = "ServerOwner")]
	public async Task<IActionResult> Edit(int serverId, ServerViewModel model)
	{
		if (model.Id == null || serverId != model.Id.Value)
		{
			return BadRequest();
		}

		if (!ModelState.IsValid)
		{
			return View(model);
		}

		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		server.Name = model.Name;
		server.Description = model.Description;
		server.IconUrl = model.IconUrl;
		server.PrivacyLevel = model.PrivacyLevel;

		await _serverService.UpdateServerAsync(server);

		return RedirectToAction(nameof(Channels), new { serverId = serverId });
	}

	/// <summary>
	/// Displays the confirmation page for deleting a server.
	/// GET /Servers/{serverId}/Delete
	/// </summary>
	/// <param name="serverId">The server ID to delete.</param>
	/// <returns>View with server deletion confirmation.</returns>
	[HttpGet("Servers/{serverId}/Delete")]
	[Authorize(Policy = "ServerOwner")]
	public async Task<IActionResult> Delete(int serverId)
	{
		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		return View(server);
	}

	/// <summary>
	/// Handles the confirmation of deleting a server.
	/// POST /Servers/{serverId}/Delete
	/// </summary>
	/// <param name="serverId">The server ID to delete.</param>
	/// <returns>Redirect to server list on success.</returns>
	[HttpPost("Servers/{serverId}/Delete")]
	[ValidateAntiForgeryToken]
	[ActionName("Delete")]
	[Authorize(Policy = "ServerOwner")]
	public async Task<IActionResult> DeleteConfirmed(int serverId)
	{
		bool deleted = await _serverService.DeleteServerAsync(serverId);
		if (!deleted)
		{
			return NotFound();
		}

		return RedirectToAction(nameof(Index));
	}

	[HttpGet("Servers/{serverId}/Join")]
	public async Task<IActionResult> Join(int serverId)
	{
		string userId = CurrentUserId!;

		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		bool isMember = await _serverService.IsUserMemberOfServerAsync(serverId, userId);
		if (isMember)
		{
			TempData["Info"] = "You are already a member of this server.";
			return RedirectToAction(nameof(Channels), new { serverId = serverId });
		}

		if (server.PrivacyLevel != PrivacyLevel.Public)
		{
			TempData["Error"] = "This server is not public. You cannot join without an invite.";
		}

		return View(server);
	}

	[HttpPost("Servers/{serverId}/Join")]
	[ValidateAntiForgeryToken]
	[ActionName("Join")]
	public async Task<IActionResult> JoinConfirmed(int serverId)
	{
		string userId = CurrentUserId!;

		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		bool isMember = await _serverService.IsUserMemberOfServerAsync(serverId, userId);
		if (isMember)
		{
			TempData["Info"] = "You are already a member of this server.";
			return RedirectToAction(nameof(Channels), new { serverId = serverId });
		}

		if (server.PrivacyLevel != PrivacyLevel.Public)
		{
			TempData["Error"] = "This server is not public. You cannot join without an invite.";
			return RedirectToAction(nameof(Join), new { serverId = serverId });
		}

		await _serverService.JoinServerAsync(serverId, userId);

		return RedirectToAction(nameof(Channels), new { serverId = serverId });
	}

	/// <summary>
	/// Shows confirmation page for leaving a server.
	/// GET /Servers/{serverId}/Leave
	/// </summary>
	/// <param name="serverId">The server ID to leave.</param>
	/// <returns>Leave confirmation view.</returns>
	[HttpGet("Servers/{serverId}/Leave")]
	[Authorize(Policy = "ServerMember")]
	public async Task<IActionResult> Leave(int serverId)
	{
		string userId = CurrentUserId!;

		// Prevent owner from leaving their own server
		if (await IsUserOwnerAsync(serverId))
		{
			TempData["Error"] = "Server owners cannot leave their server. Delete the server instead.";
			return RedirectToAction(nameof(Channels), new { serverId = serverId });
		}

		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		return View("Leave", server);
	}

	/// <summary>
	/// Handles leaving a server.
	/// POST /Servers/{serverId}/Leave
	/// </summary>
	/// <param name="serverId">The server ID to leave.</param>
	/// <returns>Redirect to server list on success.</returns>
	[HttpPost("Servers/{serverId}/Leave")]
	[ValidateAntiForgeryToken]
	[Authorize(Policy = "ServerMember")]
	[ActionName("Leave")]
	public async Task<IActionResult> LeaveConfirmed(int serverId)
	{
		string userId = CurrentUserId!;

		// Prevent owner from leaving their own server
		if (await IsUserOwnerAsync(serverId))
		{
			TempData["Error"] = "Server owners cannot leave their server. Delete the server instead.";
			return RedirectToAction(nameof(Channels), new { serverId = serverId });
		}

		bool left = await _serverService.LeaveServerAsync(serverId, userId);
		if (!left)
		{
			return NotFound();
		}

		return RedirectToAction(nameof(Index));
	}

	#endregion

	#region Channels

	/// <summary>
	/// Displays the main chat interface for a specific server and channel.
	/// GET /Servers/{serverId}/Channels/{channelId}
	/// </summary>
	/// <param name="serverId">The server ID</param>
	/// <param name="channelId">The channel ID within the server.</param>
	/// <returns>View with chat interface for the specified server and channel.</returns>
	[HttpGet("Servers/{serverId?}/Channels/{channelId?}")]
	[Authorize(Policy = "ServerMember")]
	public async Task<IActionResult> Channels(int? serverId, int? channelId)
	{
		string userId = CurrentUserId!;

		if (serverId == null)
		{
			serverId = (await _serverService.GetServersForUserAsync(userId)).FirstOrDefault()?.Id;
		}

		if (serverId == null)
		{
			return RedirectToAction(nameof(Index));
		}

		Server? server = await _serverService.GetServerAsync(serverId.Value);
		if (server == null)
		{
			return NotFound();
		}
		
		// get list of channels for server
		List<ChannelViewModel> channels = 
		(await _channelService.GetChannelsForServerAsync(serverId.Value))
		.Select(c => new ChannelViewModel { Id = c.Id, Name = c.Name })
		.ToList();		
		
		// Determine selected channel
		int? SelectedChannelId = channelId;
		if (SelectedChannelId == null && channels.Count > 0)
		{
			SelectedChannelId = channels[0].Id;
		}

		List<ServerListItemViewModel> userServers = 
			(await _serverService.GetServersForUserAsync(userId))
			.Select(s => new ServerListItemViewModel 
			{ 
				Id = s.Id,
				Name = s.Name,
				IconUrl = s.IconUrl
			})
			.ToList();

		List<MessageDto> messages = new List<MessageDto>();
		if (SelectedChannelId != null)
		{
			messages = 
			(await _messageService.GetMessagesForChannelAsync((int)SelectedChannelId, 50))
			.Select(m => new MessageDto
            {
				Id = m.Id,
				Text = m.MessageContent,
				SenderUsername = m.Sender?.UserName ?? "Unknown",
				Timestamp = m.CreatedAt
			})
			.ToList();
		}

		ServerChannelsViewModel model = new ServerChannelsViewModel
		{
			ServerId = server.Id,
			ServerName = server.Name,
			SelectedChannelId = SelectedChannelId,
			Channels = channels,
			UserServers = userServers,
			Messages = messages
		};

		ViewBag.UserRole = await GetUserRoleInServerAsync(serverId.Value);
		ViewBag.IsModerator = await IsUserModeratorOrOwnerAsync(serverId.Value);
		ViewBag.IsOwner = await IsUserOwnerAsync(serverId.Value);
		ViewBag.AllowModeratorInvites = server.PrivacyLevel == PrivacyLevel.ModeratorInvitePrivate;
		
		return View(model);
	}

	/// <summary>
	/// Displays a table of all channels in a server with edit and delete links.
	/// GET /Servers/{serverId}/Channels/Edit
	/// </summary>
	/// <param name="serverId">The server ID to view channels for.</param>
	/// <returns>View with list of channels and management options.</returns>
	[HttpGet("Servers/{serverId}/Channels/Edit")]
	[ActionName("EditChannels")]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> EditChannels(int serverId)
	{
		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		List<Channel> channels = await _channelService.GetChannelsForServerAsync(serverId);

		ViewBag.ServerName = server.Name;
		ViewBag.ServerId = serverId;

		return View(channels);
	}

	/// <summary>
	/// Displays the form for creating a new channel in a server.
	/// GET /Servers/{serverId}/Channels/Create
	/// </summary>
	/// <param name="serverId">The server ID where the channel will be created.</param>
	/// <returns>View with channel creation form.</returns>
	[HttpGet("Servers/{serverId}/Channels/Create")]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> CreateChannel(int serverId)
	{
		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		ViewBag.ServerName = server.Name;

		return View(new ChannelFormViewModel { ServerId = serverId });
	}

	/// <summary>
	/// Handles the submission of a new channel.
	/// POST /Servers/{serverId}/Channels/Create
	/// </summary>
	/// <param name="serverId">The server ID where the channel will be created.</param>
	/// <param name="model">The channel form view model with creation details.</param>
	/// <returns>Redirect to server channels on success, or view with errors on failure.</returns>
	[HttpPost("Servers/{serverId}/Channels/Create")]
	[ValidateAntiForgeryToken]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> CreateChannel(int serverId, ChannelFormViewModel model)
	{
		if (serverId != model.ServerId)
		{
			return BadRequest();
		}

		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		if (!ModelState.IsValid)
		{
			ViewBag.ServerName = server.Name;
			return View(model);
		}

		Channel channel = await _channelService.CreateChannelAsync(new Channel
		{
			Name = model.Name,
			ServerId = serverId
		});

		return RedirectToAction(nameof(Channels), new { serverId = serverId });
	}

	/// <summary>
	/// Displays the form for editing a channel.
	/// GET /Servers/{serverId}/Channels/{id}/Edit
	/// </summary>
	/// <param name="serverId">The server ID where the channel belongs.</param>
	/// <param name="id">The channel ID to edit.</param>
	/// <returns>View with channel edit form.</returns>
	[HttpGet("Servers/{serverId}/Channels/{id}/Edit")]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> EditChannel(int serverId, int id)
	{
		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		Channel? channel = await _channelService.GetChannelAsync(id);
		if (channel == null || channel.ServerId != serverId)
		{
			return NotFound();
		}

		ViewBag.ServerName = server.Name;

		ChannelFormViewModel model = new ChannelFormViewModel
		{
			Id = channel.Id,
			ServerId = channel.ServerId,
			Name = channel.Name
		};

		return View(model);
	}

	/// <summary>
	/// Handles the submission of channel edits.
	/// POST /Servers/{serverId}/Channels/{id}/Edit
	/// </summary>
	/// <param name="serverId">The server ID where the channel belongs.</param>
	/// <param name="id">The channel ID to edit.</param>
	/// <param name="model">The channel form view model with updated details.</param>
	/// <returns>Redirect to server channels on success, or view with errors on failure.</returns>
	[HttpPost("Servers/{serverId}/Channels/{id}/Edit")]
	[ValidateAntiForgeryToken]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> EditChannel(int serverId, int id, ChannelFormViewModel model)
	{
		if (id != model.Id || serverId != model.ServerId)
		{
			return BadRequest();
		}

		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		if (!ModelState.IsValid)
		{
			ViewBag.ServerName = server.Name;
			return View(model);
		}

		Channel? channel = await _channelService.GetChannelAsync(id);
		if (channel == null || channel.ServerId != serverId)
		{
			return NotFound();
		}

		channel.Name = model.Name;

		await _channelService.UpdateChannelAsync(channel);

		return RedirectToAction(nameof(Channels), new { serverId = serverId });
	}

	/// <summary>
	/// Displays the confirmation page for deleting a channel.
	/// GET /Servers/{serverId}/Channels/{id}/Delete
	/// </summary>
	/// <param name="serverId">The server ID where the channel belongs.</param>
	/// <param name="id">The channel ID to delete.</param>
	/// <returns>View with channel deletion confirmation.</returns>
	[HttpGet("Servers/{serverId}/Channels/{id}/Delete")]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> DeleteChannel(int serverId, int id)
	{
		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		Channel? channel = await _channelService.GetChannelAsync(id);
		if (channel == null || channel.ServerId != serverId)
		{
			return NotFound();
		}

		ViewBag.ServerName = server.Name;

		return View(channel);
	}

	/// <summary>
	/// Handles the confirmation of deleting a channel.
	/// POST /Servers/{serverId}/Channels/{id}/Delete
	/// </summary>
	/// <param name="serverId">The server ID where the channel belongs.</param>
	/// <param name="id">The channel ID to delete.</param>
	/// <returns>Redirect to server channels on success.</returns>
	[HttpPost("Servers/{serverId}/Channels/{id}/Delete")]
	[ValidateAntiForgeryToken]
	[ActionName("DeleteChannel")]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> DeleteChannelConfirmed(int serverId, int id)
	{
		Channel? channel = await _channelService.GetChannelAsync(id);
		if (channel == null || channel.ServerId != serverId)
		{
			return NotFound();
		}

		bool deleted = await _channelService.DeleteChannelAsync(id);
		if (!deleted)
		{
			return NotFound();
		}

		return RedirectToAction(nameof(Channels), new { serverId = serverId });
	}

	#endregion

	#region Invites

	/// <summary>
	/// Displays all invites for a specific server.
	/// GET /Servers/{serverId}/Invites
	/// </summary>
	/// <param name="serverId">The server ID to view invites for.</param>
	/// <returns>View with list of invites for the server.</returns>
	[HttpGet("Servers/{serverId}/Invites")]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> Invites(int serverId)
	{
		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		List<ServerInvite> invites = await _inviteService.GetInvitesForServerAsync(serverId);
		
		ViewBag.ServerName = server.Name;
		ViewBag.ServerId = serverId;
		
		return View(invites);
	}

	/// <summary>
	/// Displays the form for creating a new server invite.
	/// GET /Servers/{serverId}/Invites/Create
	/// </summary>
	/// <param name="serverId">The server ID to create an invite for.</param>
	/// <returns>View with invite creation form.</returns>
	[HttpGet("Servers/{serverId}/Invites/Create")]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> CreateInvite(int serverId)
	{
		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		ViewBag.ServerName = server.Name;
		
		return View(new ServerInviteViewModel { ServerId = serverId });
	}

	/// <summary>
	/// Handles the submission of a new server invite, validating authorization and creating the invite.
	/// POST /Servers/{serverId}/Invites/Create
	/// </summary>
	/// <param name="serverId">The server ID to create an invite for.</param>
	/// <param name="invite">The invite view model with creation details.</param>
	/// <returns>Redirect to invite details on success, or view with errors on failure.</returns>
	[HttpPost("Servers/{serverId}/Invites/Create")]
	[ValidateAntiForgeryToken]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> CreateInvite(int serverId, ServerInviteViewModel invite)
	{
		string userId = CurrentUserId!;

		if (serverId != invite.ServerId)
		{
			return BadRequest();
		}

		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		if (!ModelState.IsValid)
		{
			ViewBag.ServerName = server.Name;
			return View(invite);
		}

		// Validate invite creation authorization and business rules via service
		InviteValidationResult validation = await _inviteService.ValidateCreateInviteAsync(userId, invite);

		if (validation.NotFound)
		{
			return NotFound();
		}
		if (validation.Unauthorized)
		{
			return Unauthorized();
		}

		if (!validation.IsValid)
		{
			foreach (KeyValuePair<string, string> kv in validation.FieldErrors)
			{
				ModelState.AddModelError(kv.Key, kv.Value);
			}
			ViewBag.ServerName = server.Name;
			return View(invite);
		}

		// Create the invite using validated data
		string inviteCode = await _inviteService.GenerateUniqueInviteCodeAsync(8);
		DateTime? expiresAt = validation.ValidatedExpiresAt;

		ServerInvite newInvite = new ServerInvite
		{
			ServerId = invite.ServerId,
			CreatedById = userId,
			InvitedUserId = validation.InvitedUserId,
			InviteCode = inviteCode,
			CreatedAt = DateTime.UtcNow,
			ExpiresAt = expiresAt,
			OneTimeUse = invite.OneTimeUse,
			IsUsed = false,
			TimesUsed = 0
		};

		newInvite = await _inviteService.CreateInviteAsync(newInvite);

		return RedirectToAction(nameof(InviteDetails), new { serverId = serverId, id = newInvite.Id });
	}

	/// <summary>
	/// Displays detailed information about a specific invite for a server.
	/// GET /Servers/{serverId}/Invites/{id}
	/// </summary>
	/// <param name="serverId">The server ID the invite belongs to.</param>
	/// <param name="id">The invite ID to display.</param>
	/// <returns>View with invite details, or NotFound if invite doesn't exist or doesn't belong to server.</returns>
	[HttpGet("Servers/{serverId}/Invites/{id}")]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> InviteDetails(int serverId, int id)
	{
		ServerInvite? invite = await _inviteService.GetInviteAsync(id);
		if (invite == null || invite.ServerId != serverId)
		{
			return NotFound();
		}

		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		ViewBag.ServerName = server.Name;
		ViewBag.ServerId = serverId;

		return View(invite);
	}

	/// <summary>
	/// Displays the confirmation page for deleting an invite.
	/// GET /Servers/{serverId}/Invites/{id}/Delete
	/// </summary>
	/// <param name="serverId">The server ID the invite belongs to.</param>
	/// <param name="id">The invite ID to delete.</param>
	/// <returns>View with invite deletion confirmation.</returns>
	[HttpGet("Servers/{serverId}/Invites/{id}/Delete")]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> DeleteInvite(int serverId, int id)
	{
		ServerInvite? invite = await _inviteService.GetInviteAsync(id);
		if (invite == null || invite.ServerId != serverId)
		{
			return NotFound();
		}

		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		ViewBag.ServerName = server.Name;
		ViewBag.ServerId = serverId;

		return View(invite);
	}

	/// <summary>
	/// Handles the confirmation of deleting an invite.
	/// POST /Servers/{serverId}/Invites/{id}/Delete
	/// </summary>
	/// <param name="serverId">The server ID the invite belongs to.</param>
	/// <param name="id">The invite ID to delete.</param>
	/// <returns>Redirect to invites list on success.</returns>
	[HttpPost("Servers/{serverId}/Invites/{id}/Delete")]
	[ValidateAntiForgeryToken]
	[ActionName("DeleteInvite")]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> DeleteInviteConfirmed(int serverId, int id)
	{
		ServerInvite? invite = await _inviteService.GetInviteAsync(id);
		if (invite == null || invite.ServerId != serverId)
		{
			return NotFound();
		}

		bool deleted = await _inviteService.DeleteInviteAsync(id);
		if (!deleted)
		{
			return NotFound();
		}

		return RedirectToAction(nameof(Invites), new { serverId = serverId });
	}

	#endregion
}