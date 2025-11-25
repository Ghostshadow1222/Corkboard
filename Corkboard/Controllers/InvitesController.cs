using System.Linq;
using Corkboard.Data.Services;
using Corkboard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Corkboard.Controllers;

/// <summary>
/// Controller for managing server invites, including creation, viewing, and redemption.
/// Requires authentication for all actions.
/// </summary>
[Authorize]
public class InvitesController : Controller
{
	private readonly IInviteService _inviteService;
	private readonly IServerService _serverService;
	private readonly UserManager<UserAccount> _userManager;

	/// <summary>
	/// Creates a new instance of <see cref="InvitesController"/>.
	/// </summary>
	/// <param name="inviteService">Invite service for data operations.</param>
	/// <param name="serverService">Server service for authorization checks.</param>
	/// <param name="userManager">User manager for identity operations.</param>
	public InvitesController(IInviteService inviteService, IServerService serverService, UserManager<UserAccount> userManager)
	{
		_inviteService = inviteService;
		_serverService = serverService;
		_userManager = userManager;
	}

	/// <summary>
	/// Displays all invites created by the current user.
	/// GET /Invites/Index
	/// </summary>
	/// <returns>View with list of invites created by the user.</returns>
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		string? userId = _userManager.GetUserId(User);
		if (userId == null)
		{
			return Challenge();
		}

		System.Collections.Generic.List<ServerInvite> created = await _inviteService.GetInvitesCreatedByUserAsync(userId);
		
		return View(created);
	}

	/// <summary>
	/// Displays the form for creating a new server invite.
	/// GET /Invites/Create
	/// </summary>
	/// <returns>View with invite creation form, or redirect if user has no servers.</returns>
	public async Task<IActionResult> Create()
	{
		string? userId = _userManager.GetUserId(User);
		if (userId == null)
		{
			return Challenge();
		}

		await PopulateServersViewBag(userId);

		if (ViewBag.Servers == null || ((SelectList)ViewBag.Servers).Count() == 0)
		{
			TempData["ErrorMessage"] = "You need to be a member of a server before creating an invite. Create one here.";
			return RedirectToAction(nameof(ServersController.Create), "Servers");
		}

		return View(new ServerInviteViewModel());
	}

	/// <summary>
	/// Handles the submission of a new server invite, validating authorization and creating the invite.
	/// POST /Invites/Create
	/// </summary>
	/// <param name="invite">The invite view model with creation details.</param>
	/// <returns>Redirect to invite details on success, or view with errors on failure.</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(ServerInviteViewModel invite)
	{
		string? userId = _userManager.GetUserId(User);
		if (userId == null)
		{
			return Unauthorized();
		}

		Server? server = await _serverService.GetServerAsync(invite.ServerId);
		if (server == null)
		{
			return NotFound();
		}

		// Ensure current user is authorized to create invites for this server
		bool isOwner = server.OwnerId == userId;
		bool isModerator = await _serverService.IsUserModeratorOfServerAsync(server.Id, userId);

		if (!isOwner && !isModerator)
		{
			return Unauthorized();
		}

		if (server.PrivacyLevel == PrivacyLevel.OwnerInvitePrivate && !isOwner)
		{
			return Unauthorized();
		}

		// Check for user specific invite and validate if so
		string? invitedUserId = null;
		if (!string.IsNullOrWhiteSpace(invite.Username))
		{
			string username = invite.Username!.Trim();
			UserAccount? invitedUser = await _userManager.FindByNameAsync(username);
			if (invitedUser == null)
			{
				ModelState.AddModelError("Username", "The specified user does not exist.");
				await PopulateServersViewBag(userId);
				return View(invite);
			}
			invitedUserId = invitedUser.Id;
		}

		// Validate expiry input
		if (invite.Expires && invite.ExpiresAt != null && invite.ExpiresAt <= DateTime.UtcNow)
		{
			ModelState.AddModelError("ExpiresAt", "Expiration must be a future date/time.");
		}

		if (!ModelState.IsValid)
		{
			await PopulateServersViewBag(userId);
			return View(invite);
		}

		// END validation

		// Create the invite

		// Need to generate a unique code for the invite
		string inviteCode = GenerateInviteCode(8);

		// Ensure code is unique
		while (await _inviteService.InviteCodeInUseAsync(inviteCode))
		{
			inviteCode = GenerateInviteCode(8);
		}

		// If user did not opt-in to an expiration, ensure ExpiresAt is null
		DateTime? expiresAt = invite.Expires ? invite.ExpiresAt : null;

		ServerInvite newInvite = new ServerInvite
		{
			ServerId = invite.ServerId,
			CreatedById = userId,
			InvitedUserId = invitedUserId,
			InviteCode = inviteCode,
			CreatedAt = DateTime.UtcNow,
			ExpiresAt = expiresAt,
			OneTimeUse = invite.OneTimeUse,
			IsUsed = false,
			TimesUsed = 0
		};

		newInvite = await _inviteService.CreateInviteAsync(newInvite);

		return RedirectToAction(nameof(Details), "Invites", new { id = newInvite.Id });
	}

	/// <summary>
	/// Displays detailed information about a specific invite.
	/// GET /Invites/Details/{id}
	/// </summary>
	/// <param name="id">The invite ID to display.</param>
	/// <returns>View with invite details, or NotFound if invite doesn't exist.</returns>
	[HttpGet("Invites/Details/{id}")]
	public async Task<IActionResult> Details(int id)
	{
		string userId = _userManager.GetUserId(User)!;
		ServerInvite? invite = await _inviteService.GetInviteAsync(id);
		if (invite == null || invite.CreatedById != userId)
		{
			return NotFound();
		}

		if (invite.InvitedUserId != null && invite.InvitedUserId != userId)
		{
			return RedirectToAction(nameof(Redeem), new { code = invite.InviteCode });
		}

		return View(invite);
	}

	/// <summary>
	/// Displays the invite redemption page where users can join a server.
	/// GET /Invites/Redeem/{code}
	/// </summary>
	/// <param name="code">The invite code to redeem.</param>
	/// <returns>View with redemption details, NotFound if invalid, or Unauthorized if user-specific.</returns>
	[HttpGet("/Invites/Redeem/{code}")]
	public async Task<IActionResult> Redeem(string code)
	{
		string? userId = _userManager.GetUserId(User);
		if (userId == null)
		{
			return Unauthorized();
		}

		ServerInvite? invite = await _inviteService.GetInviteByCodeAsync(code);

		if (invite == null || (invite.OneTimeUse && invite.IsUsed) || (invite.ExpiresAt != null && invite.ExpiresAt < DateTime.UtcNow))
		{
			return NotFound();
		}

		if (invite.InvitedUserId != null && invite.InvitedUserId != userId)
		{
			return Unauthorized();
		}

		return View(invite);
	}

	/// <summary>
	/// Processes the redemption of an invite, adding the user to the server if valid.
	/// POST /Invites/Redeem/{code}
	/// </summary>
	/// <param name="code">The invite code to redeem.</param>
	/// <returns>Redirect to server details on success, or view with errors on failure.</returns>
	[HttpPost("/Invites/Redeem/{code}")]
	[ValidateAntiForgeryToken]
	[ActionName("Redeem")]
	public async Task<IActionResult> RedeemConfirmed(string code)
	{
		string? userId = _userManager.GetUserId(User);
		if (userId == null)
		{
			return Unauthorized();
		}

		ServerInvite? invite = await _inviteService.GetInviteByCodeAsync(code);

		if (invite == null)
		{
			ModelState.AddModelError(string.Empty, "The invite could not be found.");
			return NotFound();
		}

		if ((invite.OneTimeUse && invite.IsUsed) || (invite.ExpiresAt != null && invite.ExpiresAt < DateTime.UtcNow))
		{
			ModelState.AddModelError(string.Empty, "This invite is expired or already used.");
			return View(invite);
		}

		if (invite.InvitedUserId != null && invite.InvitedUserId != userId)
		{
			return Unauthorized();
		}

		ServerMember? member = await _inviteService.RedeemInviteAsync(invite.Id, userId);
		if (member == null)
		{
			ModelState.AddModelError(string.Empty, "Could not redeem the invite. You may already be a member or the invite is no longer valid.");
			// Re-fetch to ensure view has current state
			invite = await _inviteService.GetInviteByCodeAsync(code);
			return View(invite);
		}

		return RedirectToAction(nameof(ServersController.Details), "Servers", new { id = invite.ServerId });
	}

	/// <summary>
	/// Generates a random alphanumeric invite code.
	/// </summary>
	/// <param name="length">The length of the code to generate. Defaults to 8.</param>
	/// <returns>A randomly generated invite code string.</returns>
	private static string GenerateInviteCode(int length = 8)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		char[] result = new char[length];
		for (int i = 0; i < length; i++)
		{
			int idx = RandomNumberGenerator.GetInt32(chars.Length);
			result[i] = chars[idx];
		}
		return new string(result);
	}

	/// <summary>
	/// Populates ViewBag.Servers with a SelectList of servers the user belongs to.
	/// </summary>
	/// <param name="userId">The user ID to retrieve servers for.</param>
	private async Task PopulateServersViewBag(string userId)
	{
		System.Collections.Generic.List<Server> servers = await _serverService.GetServersForUserAsync(userId);
		ViewBag.Servers = new SelectList(servers, "Id", "Name");
	}
}
