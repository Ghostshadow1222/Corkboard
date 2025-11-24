using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Corkboard.Data.Services;
using Corkboard.Models;

namespace Corkboard.Controllers;

/// <summary>
/// Controller for managing servers, including listing, creating, viewing details, and joining.
/// </summary>
public class ServersController : Controller
{
	private readonly IServerService _serverService;
	private readonly UserManager<UserAccount> _userManager;

	/// <summary>
	/// Creates a new instance of <see cref="ServersController"/>.
	/// </summary>
	/// <param name="serverService">Server service for data operations.</param>
	/// <param name="userManager">User manager for identity operations.</param>
	public ServersController(IServerService serverService, UserManager<UserAccount> userManager)
	{
		_serverService = serverService;
		_userManager = userManager;
	}

	/// <summary>
	/// Displays a list of all servers the current user is a member of.
	/// GET /Servers
	/// </summary>
	/// <returns>View with list of user's servers.</returns>
	public async Task<IActionResult> Index()
	{
		string? userId = _userManager.GetUserId(User);
		if (userId == null)
		{
			return Challenge();
		}

		List<Server> servers = await _serverService.GetServersForUserAsync(userId);

		return View(servers);
	}

	/// <summary>
	/// Displays detailed information about a specific server.
	/// GET /Servers/Details/{id}
	/// </summary>
	/// <param name="id">The server ID to display.</param>
	/// <returns>View with server details, or redirect to Index if not found.</returns>
	[HttpGet("Servers/Details/{id}")]
	public async Task<IActionResult> Details(int id)
	{
		Server? server = await _serverService.GetServerAsync(id);
		if (server == null)
		{
			return RedirectToAction(nameof(Index));
		}
		return View(server);
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
		string? userId = _userManager.GetUserId(User);
		if (userId == null)
		{
			return Challenge();
		}

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

		return RedirectToAction(nameof(Details), new { id = server.Id });
	}

	/// <summary>
	/// Displays information about a server before joining.
	/// GET /Servers/Join/{id}
	/// </summary>
	/// <param name="id">The server ID to join.</param>
	/// <returns>View with server information, or empty view if not found.</returns>
	[HttpGet]
	public async Task<IActionResult> Join(int id)
	{
		Server? server = await _serverService.GetServerAsync(id);
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
	/// <param name="id">The server ID to join.</param>
	/// <returns>Redirect to server details on success, or appropriate error response.</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	[ActionName("Join")]
	public async Task<IActionResult> JoinConfirmed(int id)
    {
		string? userId = _userManager.GetUserId(User);
		if (userId == null)
		{
			return Challenge();;
		}

		Server? server = await _serverService.GetServerAsync(id);
		if (server == null)
		{
			return RedirectToAction(nameof(Index));
		}

		if (server.PrivacyLevel != PrivacyLevel.Public)
		{
			return Unauthorized();
		}

		await _serverService.JoinServerAsync(id, userId);

        return RedirectToAction(nameof(Details), new { id = id });
    }
}
