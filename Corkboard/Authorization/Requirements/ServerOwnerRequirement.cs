using Microsoft.AspNetCore.Authorization;

namespace Corkboard.Authorization.Requirements;

/// <summary>
/// Authorization requirement to verify that a user is the owner of a specific server.
/// The server ID can be extracted from route data using the specified route key.
/// </summary>
public class ServerOwnerRequirement : IAuthorizationRequirement
{
	/// <summary>
	/// The route parameter name to extract the server ID from.
	/// Defaults to "serverId" but can be customized for different routes.
	/// </summary>
	public string ServerIdRouteKey { get; }

	/// <summary>
	/// Creates a new instance of <see cref="ServerOwnerRequirement"/> with the provided route key.
	/// </summary>
	public ServerOwnerRequirement(string serverIdRouteKey)
    {
        ServerIdRouteKey = serverIdRouteKey;
    }
}