using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Corkboard.Data.Services;
using Corkboard.Models;

namespace Corkboard.Controllers;

public class ServersController : Controller
{
	private readonly IServerService _serverService;
	private readonly UserManager<UserAccount> _userManager;

	public ServersController(IServerService serverService, UserManager<UserAccount> userManager)
	{
		_serverService = serverService;
		_userManager = userManager;
	}

	// GET /Servers -> List servers the current user belongs to
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

	// GET /Servers/Details/{id} -> View server details
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

	// GET /Servers/Create -> Render server creation form
	[HttpGet]
	public IActionResult Create()
	{
		// Return an empty view model to the Razor Page
		return View(new ServerViewModel());
	}

	// POST /Servers/Create -> Create a new server and auto-join as owner
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

	// POST /Servers/Join/{id} -> Join an existing server
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Join(ServerInvite invite)
	{
		// Invite validation


		// Add user to server

		return View();
	}
}
