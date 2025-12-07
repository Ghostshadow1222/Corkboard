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
			RedirectToAction(nameof(Channels), new { serverId = serverId} );
		}

		string userId = CurrentUserId!;

		List<Server> servers = await _serverService.GetServersForUserAsync(userId);

		return View(servers);
	}

	/// <summary>
	/// Displays the main chat interface for a specific server and channel.
	/// GET /Servers/{serverId}/Channels/{channelId}
	/// </summary>
	/// <param name="serverId">The server ID</param>
	/// <param name="channelId">The channel ID within the server.</param>
	/// <returns>View with chat interface for the specified server and channel.</returns>
	[HttpGet("Servers/{serverId}/Channels/{channelId?}")]
	[Authorize(Policy = "ServerMember")]
	public async Task<IActionResult> Channels(int serverId, int? channelId)
	{
		string userId = CurrentUserId!;

		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return NotFound();
		}

		List<ChannelViewModel> channels = 
			(await _channelService.GetChannelsForServerAsync(serverId))
			.Select(c => new ChannelViewModel { Id = c.Id, Name = c.Name })
			.ToList();

		int SelectedChannelId = channelId ?? channels.FirstOrDefault()?.Id ?? _channelService
			.GetChannelsForServerAsync(serverId)
			.Result
			.First()
			.Id;

		List<ServerListItemViewModel> userServers = 
			(await _serverService.GetServersForUserAsync(userId))
			.Select(s => new ServerListItemViewModel { Id = s.Id, Name = s.Name, IconUrl = s.IconUrl })
			.ToList();

		List<MessageDto> messages = await _messageService.GetMessagesForChannelAsync(SelectedChannelId, 50);

		ServerChannelsViewModel model = new ServerChannelsViewModel
		{
			ServerId = server.Id,
			ServerName = server.Name,
			ChannelId = SelectedChannelId,
			Channels = channels,
			UserServers = userServers,
			Messages = messages
		};

		ViewBag.UserRole = await GetUserRoleInServerAsync(serverId);
		ViewBag.IsModerator = await IsUserModeratorOrOwnerAsync(serverId);
		ViewBag.IsOwner = await IsUserOwnerAsync(serverId);
		ViewBag.AllowModeratorInvites = server.PrivacyLevel == PrivacyLevel.ModeratorInvitePrivate;
		
		return View(model);
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
			OwnerId = userId
		};

		// Create the server
		server = await _serverService.CreateServerAsync(server, userId);

		return RedirectToAction(nameof(Channels), new { serverId = server.Id });
	}

	/// <summary>
	/// Displays information about a server before joining.
	/// GET /Servers/Join/{id}
	/// </summary>
	/// <param name="serverId">The server ID to join.</param>
	/// <returns>View with server information, or empty view if not found.</returns>
	[HttpGet]
	public async Task<IActionResult> Join(int serverId)
	{
		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return View();
		}
		return View(server);
	}

	/// <summary>
	/// Handles the confirmation of joining a server.
	/// POST /Servers/Join/{id}
	/// </summary>
	/// <param name="serverId">The server ID to join.</param>
	/// <returns>Redirect to server details on success, or appropriate error response.</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	[ActionName("Join")]
	public async Task<IActionResult> JoinConfirmed(int serverId)
    {
		string userId = CurrentUserId!;

		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
		{
			return RedirectToAction(nameof(Index));
		}

		if (server.PrivacyLevel != PrivacyLevel.Public)
		{
			return Unauthorized();
		}

		await _serverService.JoinServerAsync(serverId, userId);

        return RedirectToAction(nameof(Channels), new { serverId = serverId });
    }

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
		string userId = CurrentUserId!;

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
		string userId = CurrentUserId!;
		
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
	/// Creates a new channel in a server (API endpoint).
	/// POST /Servers/{serverId}/Channels/Create
	/// </summary>
	/// <param name="serverId">The server ID where the channel will be created.</param>
	/// <param name="request">The channel creation request containing channel details.</param>
	/// <returns>JSON response with created channel details or error.</returns>
	[HttpPost("Servers/{serverId}/Channels/Create")]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> CreateChannel(int serverId, [FromBody] CreateChannelRequest request)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		// Verify server exists
		Server? server = await _serverService.GetServerAsync(serverId);
		if (server == null)
			return NotFound();

		Channel channel = await _channelService.CreateChannelAsync(new Channel
		{
			Name = request.Name,
			ServerId = serverId
		});

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
		[System.ComponentModel.DataAnnotations.Required]
		[System.ComponentModel.DataAnnotations.MaxLength(100)]
		public string Name { get; set; } = string.Empty;
	}
}