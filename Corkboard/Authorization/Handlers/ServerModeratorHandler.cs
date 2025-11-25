using System.Security.Claims;
using Corkboard.Authorization.Requirements;
using Corkboard.Authorization.Helpers;
using Corkboard.Data.Services;
using Microsoft.AspNetCore.Authorization;

namespace Corkboard.Authorization.Handlers;

/// <summary>
/// Authorization handler that verifies a user is a moderator or owner of a server.
/// Extracts the server ID from route data and checks moderator status via IServerService.
/// </summary>
public class ServerModeratorHandler : AuthorizationHandler<ServerModeratorRequirement>
{
	private readonly IServerService _serverService;
	private readonly RouteDataHelper _routeDataHelper;
	private readonly IHttpContextAccessor _httpContextAccessor;

	/// <summary>
	/// Creates a new instance of <see cref="ServerModeratorHandler"/>.
	/// </summary>
	/// <param name="serverService">Service for server moderator checks.</param>
	/// <param name="routeDataHelper">Helper for extracting server IDs from route data.</param>
	/// <param name="httpContextAccessor">Accessor for HTTP context and route data.</param>
	public ServerModeratorHandler(
		IServerService serverService,
		RouteDataHelper routeDataHelper,
		IHttpContextAccessor httpContextAccessor)
	{
		_serverService = serverService;
		_routeDataHelper = routeDataHelper;
		_httpContextAccessor = httpContextAccessor;
	}

	/// <summary>
	/// Handles the authorization check for server moderator status.
	/// </summary>
	/// <param name="context">The authorization context.</param>
	/// <param name="requirement">The server moderator requirement to evaluate.</param>
	protected override async Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		ServerModeratorRequirement requirement)
	{
		// Get user ID from claims
		string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrEmpty(userId))
		{
			context.Fail();
			return;
		}

		// Get HTTP context and route data
		HttpContext? httpContext = _httpContextAccessor.HttpContext;
		if (httpContext == null)
		{
			context.Fail();
			return;
		}

		// Try to get server ID from route data
		RouteData routeData = httpContext.GetRouteData();
		int? serverId = _routeDataHelper.GetServerIdFromRoute(routeData, requirement.ServerIdRouteKey);
		if (serverId == null)
		{
			context.Fail();
			return;
		}

		// Check if user is a moderator or owner of the server
		bool isModerator = await _serverService.IsUserModeratorOfServerAsync(serverId.Value, userId);
		if (isModerator)
		{
			context.Succeed(requirement);
		}
		else
		{
			context.Fail();
		}
	}
}
