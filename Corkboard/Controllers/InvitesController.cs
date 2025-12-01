using System.Linq;
using Corkboard.Data.Services;
using Corkboard.Models;
using Corkboard.Models.ViewModels.InvitesController;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Corkboard.Controllers;

/// <summary>
/// Controller for redeeming server invites.
/// Invite creation and management are handled by ServersController.
/// </summary>
[Authorize]
public class InvitesController : BaseController
{
	private readonly IInviteService _inviteService;

	/// <summary>
	/// Creates a new instance of <see cref="InvitesController"/>.
	/// </summary>
	/// <param name="inviteService">Invite service for data operations.</param>
	/// <param name="userManager">User manager for identity operations.</param>
	/// <param name="serverService">Server service for data operations.</param>
	public InvitesController(IInviteService inviteService, UserManager<UserAccount> userManager, IServerService serverService)
		: base(userManager, serverService)
	{
		_inviteService = inviteService;
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

		return RedirectToAction(nameof(ServersController.Channels), "Servers", new { serverId = invite.ServerId });
	}
}
