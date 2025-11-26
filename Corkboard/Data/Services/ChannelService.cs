using Corkboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Corkboard.Data.Services;

/// <summary>
/// Service contract for channel-related queries and operations.
/// </summary>
public interface IChannelService
{
	/// <summary>
	/// Returns all channels for a given server ordered by creation time.
	/// </summary>
	/// <param name="serverId">Server id.</param>
	/// <returns>List of channels in the server.</returns>
	Task<List<Channel>> GetChannelsForServerAsync(int serverId);

	/// <summary>
	/// Retrieves a channel by id.
	/// </summary>
	/// <param name="id">Channel id.</param>
	/// <returns>The channel or <c>null</c> if it does not exist.</returns>
	Task<Channel?> GetChannelAsync(int id);

	/// <summary>
	/// Creates a new channel in a server.
	/// </summary>
	/// <param name="channel">Channel to create.</param>
	/// <returns>The created channel.</returns>
	Task<Channel> CreateChannelAsync(Channel channel);
}

/// <summary>
/// Concrete implementation of <see cref="IChannelService"/> for channel queries.
/// </summary>
public class ChannelService : IChannelService
{
	private readonly ApplicationDbContext _context;

	/// <summary>
	/// Creates a new instance of <see cref="ChannelService"/>.
	/// </summary>
	/// <param name="context">Application DbContext (injected).</param>
	public ChannelService(ApplicationDbContext context)
	{
		_context = context;
	}

	/// <inheritdoc/>
	public async Task<List<Channel>> GetChannelsForServerAsync(int serverId)
	{
		return await _context.Channels
			.Where(c => c.ServerId == serverId)
			.OrderBy(c => c.CreatedAt)
			.ToListAsync();
	}

	/// <inheritdoc/>
	public async Task<Channel?> GetChannelAsync(int id)
	{
		return await _context.Channels.FindAsync(id);
	}

	/// <inheritdoc/>
	public async Task<Channel> CreateChannelAsync(Channel channel)
	{
		channel.CreatedAt = DateTime.UtcNow;
		_context.Channels.Add(channel);
		await _context.SaveChangesAsync();
		return channel;
	}
}
