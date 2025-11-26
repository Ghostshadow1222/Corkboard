using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Corkboard.Data.Services;
using Corkboard.Models;

namespace Corkboard.Controllers;

/// <summary>
/// Controller for managing servers, including listing, creating, viewing details, and joining.
/// </summary>
[Authorize]
public class ServersController : BaseController
{
	private readonly IServerService _serverService;

	/// <summary>
	/// Creates a new instance of <see cref="ServersController"/>.
	/// </summary>
	/// <param name="serverService">Server service for data operations.</param>
	/// <param name="userManager">User manager for identity operations.</param>
	public ServersController(IServerService serverService, UserManager<UserAccount> userManager)
		: base(userManager)
	{
		_serverService = serverService;
	}

	/// <summary>
	/// Displays a list of all servers the current user is a member of.
	/// GET /Servers
	/// </summary>
	/// <returns>View with list of user's servers.</returns>
	public async Task<IActionResult> Index()
	{
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
			(await _serverService.GetChannelsForServerAsync(serverId))
			.Select(c => new ChannelViewModel { Id = c.Id, Name = c.Name })
			.ToList();

		int SelectedChannelId = channelId ?? channels.FirstOrDefault()?.Id ?? _serverService
			.GetChannelsForServerAsync(serverId)
			.Result
			.First()
			.Id;

		ServerChannelsViewModel model = new ServerChannelsViewModel
		{
			ServerId = server.Id,
			ServerName = server.Name,
			SelectedChannelId = SelectedChannelId,
			Channels = channels
		};
		
		return View(model);
	}

	/// <summary>
	/// Displays the form for creating a new server.
	/// GET /Servers/Create
	/// </summary>
	/// <returns>View with server creation form.</returns>
	[HttpGet]
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

		return RedirectToAction(nameof(Channels), new { id = server.Id });
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
}
