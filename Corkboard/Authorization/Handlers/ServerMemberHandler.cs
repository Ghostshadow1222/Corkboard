using System.Security.Claims;
using Corkboard.Authorization.Requirements;
using Corkboard.Authorization.Helpers;
using Corkboard.Data.Services;
using Microsoft.AspNetCore.Authorization;

namespace Corkboard.Authorization.Handlers;

/// <summary>
/// Authorization handler that verifies a user is a member of a server.
/// Extracts the server ID from route data and checks membership via IServerService.
/// </summary>
public class ServerMemberHandler : AuthorizationHandler<ServerMemberRequirement>
{
	private readonly IServerService _serverService;
	private readonly RouteDataHelper _routeDataHelper;
	private readonly IHttpContextAccessor _httpContextAccessor;

	/// <summary>
	/// Creates a new instance of <see cref="ServerMemberHandler"/>.
	/// </summary>
	/// <param name="serverService">Service for server membership checks.</param>
	/// <param name="routeDataHelper">Helper for extracting server IDs from route data.</param>
	/// <param name="httpContextAccessor">Accessor for HTTP context and route data.</param>
	public ServerMemberHandler(
		IServerService serverService,
		RouteDataHelper routeDataHelper,
		IHttpContextAccessor httpContextAccessor)
	{
		_serverService = serverService;
		_routeDataHelper = routeDataHelper;
		_httpContextAccessor = httpContextAccessor;
	}

	/// <summary>
	/// Handles the authorization check for server membership.
	/// </summary>
	/// <param name="context">The authorization context.</param>
	/// <param name="requirement">The server member requirement to evaluate.</param>
	protected override async Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		ServerMemberRequirement requirement)
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
		int? serverId = await _routeDataHelper.GetServerIdFromRouteAsync(routeData, requirement.ServerIdRouteKey);
		if (serverId == null)
		{
			context.Fail();
			return;
		}

		// Check if user is a member of the server
		bool isMember = await _serverService.IsUserMemberOfServerAsync(serverId.Value, userId);
		if (isMember)
		{
			context.Succeed(requirement);
		}
		else
		{
			context.Fail();
		}
	}
}
