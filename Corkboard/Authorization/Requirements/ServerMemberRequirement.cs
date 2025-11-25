using Microsoft.AspNetCore.Authorization;

namespace Corkboard.Authorization.Requirements;

/// <summary>
/// Authorization requirement to verify that a user is a member of a specific server.
/// The server ID can be extracted from route data using the specified route key.
/// </summary>
public class ServerMemberRequirement : IAuthorizationRequirement
{
	/// <summary>
	/// The route parameter name to extract the server ID from.
	/// Defaults to "serverId" but can be customized for different routes.
	/// </summary>
	public string ServerIdRouteKey { get; set; } = "serverId";

	/// <summary>
	/// Creates a new instance of <see cref="ServerMemberRequirement"/> with default route key.
	/// </summary>
	public ServerMemberRequirement()
	{
	}

	/// <summary>
	/// Creates a new instance of <see cref="ServerMemberRequirement"/> with a custom route key.
	/// </summary>
	/// <param name="serverIdRouteKey">The route parameter name containing the server ID.</param>
	public ServerMemberRequirement(string serverIdRouteKey)
	{
		ServerIdRouteKey = serverIdRouteKey;
	}
}
