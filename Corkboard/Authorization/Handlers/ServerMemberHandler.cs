using System.Security.Claims;
using Corkboard.Authorization.Requirements;
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

	/// <summary>
	/// Creates a new instance of <see cref="ServerMemberHandler"/>.
	/// </summary>
	/// <param name="serverService">Service for server membership checks.</param>
	public ServerMemberHandler(IServerService serverService)
	{
		_serverService = serverService;
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

		// Get httpContext
		if (context.Resource is not HttpContext httpContext)
		{
			context.Fail();
			return;
		}

		// Get groupId value as a string within an object
		object? routeValue = null;
		httpContext.Request.RouteValues.TryGetValue(requirement.ServerIdRouteKey, out routeValue);
		if (routeValue == null)
		{
			context.Fail();
			return;
		}

		// Try to convert groupId string to int
		string serverIdStr = routeValue.ToString() ?? string.Empty;
		if (!int.TryParse(serverIdStr, out int serverId))
		{
			context.Fail();
			return;
		}
		
		// Check for membership record once we have groupId and userId
		bool isMember = await _serverService.IsUserMemberOfServerAsync(serverId, userId);
		if (isMember)
		{
			context.Succeed(requirement);
			return;
		}
		
		context.Fail();
	}
}
