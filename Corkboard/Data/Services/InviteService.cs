using Corkboard.Models;

namespace Corkboard.Data.Services;

public interface IInviteService
{
	public Task<ServerInvite?> GetInviteAsync(int id);

	public Task<ServerInvite> CreateInviteAsync(ServerInvite invite);
}

public class InviteService : IInviteService
{
	private readonly ApplicationDbContext _context;

	/// <summary>
	/// Creates a new instance of <see cref="InviteService"/>.
	/// </summary>
	/// <param name="context">Application DbContext (injected).</param>
	public InviteService(ApplicationDbContext context)
	{
		_context = context;
	}

	/// <inheritdoc/>
	public async Task<ServerInvite?> GetInviteAsync(int id)
	{
		return await _context.ServerInvites.FindAsync(id);
	}

	public async Task<ServerInvite> CreateInviteAsync(ServerInvite invite)
	{
		_context.ServerInvites.Add(invite);
		await _context.SaveChangesAsync();
		return invite;
	}
}
