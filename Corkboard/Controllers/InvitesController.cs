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
public class InvitesController : BaseController
{
	private readonly IInviteService _inviteService;
	private readonly IServerService _serverService;

	/// <summary>
	/// Creates a new instance of <see cref="InvitesController"/>.
	/// </summary>
	/// <param name="inviteService">Invite service for data operations.</param>
	/// <param name="serverService">Server service for authorization checks.</param>
	/// <param name="userManager">User manager for identity operations.</param>
	public InvitesController(IInviteService inviteService, IServerService serverService, UserManager<UserAccount> userManager)
		: base(userManager)
	{
		_inviteService = inviteService;
		_serverService = serverService;
	}

	/// <summary>
	/// Displays all invites created by the current user.
	/// GET /Invites/Index
	/// </summary>
	/// <returns>View with list of invites created by the user.</returns>
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		string userId = CurrentUserId!;

		System.Collections.Generic.List<ServerInvite> created = await _inviteService.GetInvitesCreatedByUserAsync(userId);
		
		return View(created);
	}

	/// <summary>
	/// Displays the form for creating a new server invite.
	/// GET /Invites/Create
	/// </summary>
	/// <returns>View with invite creation form, or redirect if user has no servers.</returns>
	[HttpGet("Invites/Create/{serverId}")]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> Create(int serverId)
	{
		string userId = CurrentUserId!;

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
	[HttpPost("Invites/Create/{serverId}")]
	[Authorize(Policy = "ServerModerator")]
	public async Task<IActionResult> Create(int serverId, ServerInviteViewModel invite)
	{
		string userId = CurrentUserId!;

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
			await PopulateServersViewBag(userId);
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
		string userId = CurrentUserId!;
		ServerInvite? invite = await _inviteService.GetInviteAsync(id);
		if (invite == null || invite.CreatedById != userId)
		{
			return NotFound();
		}

		// if the invite is user-specific and the current user is the invited user, redirect to redeem page
		if (invite.InvitedUserId != null && invite.InvitedUserId == userId)
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
		string userId = CurrentUserId!;

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
		string userId = CurrentUserId!;

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
	/// Populates ViewBag.Servers with a SelectList of servers the user belongs to.
	/// </summary>
	/// <param name="userId">The user ID to retrieve servers for.</param>
	private async Task PopulateServersViewBag(string userId)
	{
		List<Server> servers = await _serverService.GetServersForUserAsync(userId);
		ViewBag.Servers = new SelectList(servers, "Id", "Name");
	}
}
