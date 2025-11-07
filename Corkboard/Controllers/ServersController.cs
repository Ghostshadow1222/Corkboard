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
		var userId = _userManager.GetUserId(User);
		if (userId == null)
		{
			return Challenge();
		}

		var servers = await _serverService.GetServersForUserAsync(userId);

		return View(servers);
	}

	// GET /Servers/Create -> Render server creation form
	[HttpGet]
	public IActionResult Create()
	{
		return View();
	}

	// POST /Servers/Create -> Create a new server and auto-join as owner
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind("Name,IconUrl,Description")] Server model)
	{
		var userId = _userManager.GetUserId(User);
		if (userId == null)
		{
			return Challenge();
		}

		// Basic validation
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var server = new Server
		{
			Name = model.Name,
			IconUrl = model.IconUrl,
			Description = model.Description,
			OwnerId = userId
		};

		await _serverService.CreateServerAsync(server, userId);

		return RedirectToAction(nameof(Index));
	}

	// POST /Servers/Join/{id} -> Join an existing server
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Join(int id)
	{
		var userId = _userManager.GetUserId(User);
		if (userId == null)
		{
			return Challenge();
		}

		var server = await _serverService.GetServerAsync(id);
		if (server == null)
		{
			return NotFound();
		}

		await _serverService.JoinServerAsync(id, userId);

		return RedirectToAction("Index", "Channels", new { serverId = id });
	}
}
