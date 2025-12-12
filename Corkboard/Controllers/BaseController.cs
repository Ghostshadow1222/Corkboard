using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Corkboard.Models;
using Corkboard.Data.Services;

namespace Corkboard.Controllers;

/// <summary>
/// Base controller providing common functionality for all controllers.
/// Provides easy access to the current user's ID without repeated UserManager injections.
/// </summary>
public abstract class BaseController : Controller
{
	protected readonly UserManager<UserAccount> _userManager;

	protected readonly IServerService _serverService;

	/// <summary>
	/// Creates a new instance of <see cref="BaseController"/>.
	/// </summary>
	/// <param name="userManager">User manager for identity operations.</param>
	/// <param name="serverService">Server service for server operations.</param>
	protected BaseController(UserManager<UserAccount> userManager, IServerService serverService)
	{
		_userManager = userManager;
		_serverService = serverService;
	}

	/// <summary>
	/// Gets the current user's ID from the User principal.
	/// </summary>
	/// <returns>The current user's ID, or null if not authenticated.</returns>
	protected string? CurrentUserId => _currentUserId ??= _userManager.GetUserId(User);
	private string? _currentUserId;

	/// <summary>
	/// Gets the current user's role within the specified server.
	/// </summary>
	/// <param name="serverId">The server ID to evaluate membership for.</param>
	/// <returns>
	/// The user's <see cref="RoleType"/> if they are a member; otherwise <c>null</c>.
	/// Returns <c>null</c> if the user is not authenticated.
	/// </returns>
	protected async Task<RoleType?> GetUserRoleInServerAsync(int serverId)
	{
		if (CurrentUserId == null) return null;

		ServerMember? member = await _serverService.GetServerMemberAsync(serverId, CurrentUserId);

		return member?.Role;
	}

	/// <summary>
	/// Determines whether the current user is a moderator or owner of the specified server.
	/// </summary>
	/// <param name="serverId">The server ID to check authorization against.</param>
	/// <returns><c>true</c> if the user is a moderator or owner; otherwise <c>false</c>.</returns>
	protected async Task<bool> IsUserModeratorOrOwnerAsync(int serverId)
	{
		RoleType? role = await GetUserRoleInServerAsync(serverId);
		return role == RoleType.Moderator || role == RoleType.Owner;
	}

	/// <summary>
	/// Determines whether the current user is the owner of the specified server.
	/// </summary>
	/// <param name="serverId">The server ID to check ownership for.</param>
	/// <returns><c>true</c> if the user is the server owner; otherwise <c>false</c>.</returns>
	protected async Task<bool> IsUserOwnerAsync(int serverId)
	{
		RoleType? role = await GetUserRoleInServerAsync(serverId);
		return role == RoleType.Owner;
	}
}
