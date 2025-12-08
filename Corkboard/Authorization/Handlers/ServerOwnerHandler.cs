using System.Security.Claims;
using Corkboard.Authorization.Requirements;
using Corkboard.Data.Services;
using Microsoft.AspNetCore.Authorization;

namespace Corkboard.Authorization.Handlers;

/// <summary>
/// Authorization handler that verifies a user is the owner of a server.
/// Extracts the server ID from route data and checks ownership via IServerService.
/// </summary>
public class ServerOwnerHandler : AuthorizationHandler<ServerOwnerRequirement>
{
	private readonly IServerService _serverService;

	/// <summary>
	/// Creates a new instance of <see cref="ServerOwnerHandler"/>.
	/// </summary>
	/// <param name="serverService">Service for server ownership checks.</param>
	public ServerOwnerHandler(IServerService serverService)
	{
		_serverService = serverService;
	}

	/// <summary>
	/// Handles the authorization check for server ownership.
	/// </summary>
	/// <param name="context">The authorization context.</param>
	/// <param name="requirement">The server owner requirement to evaluate.</param>
	protected override async Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		ServerOwnerRequirement requirement)
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
		
		// Check if user is the owner of the server
		bool isOwner = await _serverService.IsUserOwnerOfServerAsync(serverId, userId);
		if (isOwner)
		{
			context.Succeed(requirement);
			return;
		}
		
		context.Fail();
	}
}