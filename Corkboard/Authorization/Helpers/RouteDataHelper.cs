using Corkboard.Data.Services;
using Microsoft.AspNetCore.Routing;

namespace Corkboard.Authorization.Helpers;

/// <summary>
/// Helper class for extracting server IDs from route data in authorization handlers.
/// </summary>
public class RouteDataHelper
{
	private readonly IChannelService _channelService;

	/// <summary>
	/// Creates a new instance of <see cref="RouteDataHelper"/>.
	/// </summary>
	/// <param name="channelService">Service for resolving channels to servers.</param>
	public RouteDataHelper(IChannelService channelService)
	{
		_channelService = channelService;
	}

	/// <summary>
	/// Extracts the server ID from route data, handling both direct server IDs and channel IDs.
	/// </summary>
	/// <param name="routeData">The route data containing route parameters.</param>
	/// <param name="routeKey">The primary route key to look for (usually "serverId" or "id").</param>
	/// <returns>The server ID if found, otherwise null.</returns>
	public async Task<int?> GetServerIdFromRouteAsync(RouteData routeData, string routeKey = "serverId")
	{
		// Try the specified route key first
		if (routeData.Values.TryGetValue(routeKey, out object? serverIdValue) && 
		    int.TryParse(serverIdValue?.ToString(), out int serverId))
		{
			return serverId;
		}

		// Try "id" as fallback
		if (routeKey != "id" && 
		    routeData.Values.TryGetValue("id", out object? idValue) && 
		    int.TryParse(idValue?.ToString(), out int id))
		{
			return id;
		}

		// Check if we have a channelId that needs to be resolved to serverId
		if (routeData.Values.TryGetValue("channelId", out object? channelIdValue) && 
		    int.TryParse(channelIdValue?.ToString(), out int channelId))
		{
			Models.Channel? channel = await _channelService.GetChannelAsync(channelId);
			return channel?.ServerId;
		}

		return null;
	}

	/// <summary>
	/// Extracts the server ID from route data synchronously (without channel resolution).
	/// Use this when channel-to-server resolution is not needed.
	/// </summary>
	/// <param name="routeData">The route data containing route parameters.</param>
	/// <param name="routeKey">The primary route key to look for (usually "serverId" or "id").</param>
	/// <returns>The server ID if found, otherwise null.</returns>
	public int? GetServerIdFromRoute(RouteData routeData, string routeKey = "serverId")
	{
		// Try the specified route key first
		if (routeData.Values.TryGetValue(routeKey, out object? serverIdValue) && 
		    int.TryParse(serverIdValue?.ToString(), out int serverId))
		{
			return serverId;
		}

		// Try "id" as fallback
		if (routeKey != "id" && 
		    routeData.Values.TryGetValue("id", out object? idValue) && 
		    int.TryParse(idValue?.ToString(), out int id))
		{
			return id;
		}

		return null;
	}
}
