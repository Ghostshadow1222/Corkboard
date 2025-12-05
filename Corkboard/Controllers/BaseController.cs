using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Corkboard.Models;

namespace Corkboard.Controllers;

/// <summary>
/// Base controller providing common functionality for all controllers.
/// Provides easy access to the current user's ID without repeated UserManager injections.
/// </summary>
public abstract class BaseController : Controller
{
	protected readonly UserManager<UserAccount> _userManager;

	/// <summary>
	/// Creates a new instance of <see cref="BaseController"/>.
	/// </summary>
	/// <param name="userManager">User manager for identity operations.</param>
	protected BaseController(UserManager<UserAccount> userManager)
	{
		_userManager = userManager;
	}

	/// <summary>
	/// Gets the current user's ID from the User principal.
	/// </summary>
	/// <returns>The current user's ID, or null if not authenticated.</returns>
	protected string? CurrentUserId => _currentUserId ??= _userManager.GetUserId(User);
	private string? _currentUserId;
}
