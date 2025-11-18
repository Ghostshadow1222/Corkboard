using System.Linq;
using Corkboard.Data.Services;
using Corkboard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Corkboard.Controllers;

public class InvitesController : Controller
{
	private readonly IInviteService _inviteService;
	private readonly IServerService _serverService;
	private readonly UserManager<UserAccount> _userManager;

	public InvitesController(IInviteService inviteService, IServerService serverService, UserManager<UserAccount> userManager)
	{
		_inviteService = inviteService;
		_serverService = serverService;
		_userManager = userManager;
	}

	[HttpGet]
	public async Task<IActionResult> Create()
	{
		string? userId = _userManager.GetUserId(User);
		if (userId == null)
		{
			return Forbid();
		}

		await PopulateServersViewBag(userId);

		return View(new ServerInviteViewModel());
	}

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
			return Unauthorized();
		}

		// Ensure current user is authorized to create invites for this server
		bool isOwner = server.OwnerId == userId;
		bool isModerator = await _serverService.IsUserModeratorAsync(server.Id, userId);

		if (!isOwner && !isModerator)
		{
			return Unauthorized();
		}

		if (server.PrivacyLevel == PrivacyLevel.OwnerInvitePrivate && !isOwner)
		{
			return Unauthorized();
		}

		// Check for user specific invite and validate if so
		if (invite.UserId != null)
		{
			UserAccount? invitedUser = await _userManager.FindByIdAsync(invite.UserId);
			if (invitedUser == null)
			{
				ModelState.AddModelError("UserId", "The specified user does not exist.");
				await PopulateServersViewBag(userId);
				return View(invite);
			}
		}

		if (!ModelState.IsValid) 
		{ 
			await PopulateServersViewBag(userId);
			return View(invite);
		}

		// END validation

		// Create the invite

		// Need to generate a unique code for the invite
		string code = GenerateInviteCode(8);

		// Ensure code is unique
		while (await _inviteService.InviteCodeInUseAsync(code))
		{
			code = GenerateInviteCode(8);
		}

		ServerInvite newInvite = new ServerInvite
		{
			ServerId = invite.ServerId,
			CreatorId = userId,
			InvitedUserId = invite.UserId,
			Code = code,
			CreatedAt = DateTime.UtcNow,
			ExpiresAt = DateTime.UtcNow.AddDays(invite.DaysUntilExpires),
			OneTimeUse = invite.OneTimeUse,
			IsUsed = false,
			TimesUsed = 0
		};

		newInvite = await _inviteService.CreateInviteAsync(newInvite);

		return RedirectToAction(nameof(Details), "Invites", new { id = newInvite.Id });
	}

	[HttpGet("Invites/Details/{id}")]
	public async Task<IActionResult> Details(int id)
	{
		ServerInvite? invite = await _inviteService.GetInviteAsync(id);

		if (invite == null)
		{
			return NotFound();
		}

		return View(invite);
	}

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

	private async Task PopulateServersViewBag(string userId)
	{
		var servers = await _serverService.GetServersForUserAsync(userId);
		ViewBag.Servers = new SelectList(servers, "Id", "Name");
	}
}
