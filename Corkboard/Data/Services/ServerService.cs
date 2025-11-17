using Corkboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Corkboard.Data.Services;

/// <summary>
/// Service contract for server-related operations (servers, membership, creation).
/// Implementations should be registered as scoped to match the DbContext lifetime.
/// </summary>
public interface IServerService
{
	/// <summary>
	/// Gets the list of servers the specified user belongs to.
	/// </summary>
	/// <param name="userId">The user's Id.</param>
	/// <returns>List of servers the user is a member of.</returns>
	Task<List<Server>> GetServersForUserAsync(string userId);

	/// <summary>
	/// Retrieves a server by its identifier.
	/// </summary>
	/// <param name="id">Server id.</param>
	/// <returns>The server or <c>null</c> if not found.</returns>
	Task<Server?> GetServerAsync(int id);

	/// <summary>
	/// Creates a new server and performs necessary setup such as adding the owner as a member
	/// and creating a default "general" channel.
	/// </summary>
	/// <param name="server">Server entity to create (OwnerId may be set by caller).</param>
	/// <param name="ownerUserId">User Id of the server owner.</param>
	/// <returns>The created server with generated Id populated.</returns>
	Task<Server> CreateServerAsync(Server server, string ownerUserId);

	/// <summary>
	/// Adds the specified user as a member of the given server if they are not already a member.
	/// </summary>
	/// <param name="serverId">Server id to join.</param>
	/// <param name="userId">User id to add as a member.</param>
	Task JoinServerAsync(int serverId, string userId);
}

/// <summary>
/// Concrete implementation of <see cref="IServerService"/> responsible for server persistence
/// and related setup operations.
/// </summary>
public class ServerService : IServerService
{
	private readonly ApplicationDbContext _context;

	/// <summary>
	/// Creates a new instance of <see cref="ServerService"/>.
	/// </summary>
	/// <param name="context">Application DbContext (injected).</param>
	public ServerService(ApplicationDbContext context)
	{
		_context = context;
	}

	/// <inheritdoc/>
	public async Task<List<Server>> GetServersForUserAsync(string userId)
	{
		// Query the Servers directly and include related data so the returned Server
		// instances have their Members populated (MemberCount will be accurate).
		return await _context.Servers
			.Where(s => s.Members.Any(sm => sm.UserId == userId))
			.Include(s => s.Members)
				.ThenInclude(sm => sm.User)
			.Include(s => s.Channels)
			.Include(s => s.Owner)
			.ToListAsync();
	}

	/// <inheritdoc/>
	public async Task<Server?> GetServerAsync(int id)
	{
		// Use eager loading to include related collections and navigation properties so
		// that callers (e.g. controller views) can see members, their users, channels and owner.
		return await _context.Servers
			.Include(s => s.Members)
				.ThenInclude(sm => sm.User)
			.Include(s => s.Channels)
			.Include(s => s.Owner)
			.SingleOrDefaultAsync(s => s.Id == id);
	}

	/// <inheritdoc/>
	public async Task<Server> CreateServerAsync(Server server, string ownerUserId)
	{
		// Run within the same DbContext and save changes once to ensure consistency.
		_context.Servers.Add(server);
		await _context.SaveChangesAsync();

		ServerMember member = new ServerMember
		{
			ServerId = server.Id,
			UserId = ownerUserId,
			Role = "owner"
		};
		_context.ServerMembers.Add(member);

		Channel general = new Channel
		{
			Name = "general",
			ServerId = server.Id
		};
		_context.Channels.Add(general);

		await _context.SaveChangesAsync();
		return server;
	}

	/// <inheritdoc/>
	public async Task JoinServerAsync(int serverId, string userId)
	{
		bool exists = await _context.ServerMembers.AnyAsync(sm => sm.ServerId == serverId && sm.UserId == userId);
		if (!exists)
		{
			ServerMember member = new ServerMember { ServerId = serverId, UserId = userId };
			_context.ServerMembers.Add(member);
			await _context.SaveChangesAsync();
		}
	}
}
