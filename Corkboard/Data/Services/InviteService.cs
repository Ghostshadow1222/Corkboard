using Corkboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Corkboard.Data.Services;

public interface IInviteService
{
	public Task<ServerInvite?> GetInviteAsync(int id);

	public Task<ServerInvite> CreateInviteAsync(ServerInvite invite);

	Task<bool> InviteCodeInUseAsync(string code);
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
		// Include the Server navigation property so views can access Model.Server safely
		return await _context.ServerInvites
			.Include(i => i.Server)
			.FirstOrDefaultAsync(i => i.Id == id);
	}

	public async Task<ServerInvite> CreateInviteAsync(ServerInvite invite)
	{
		_context.ServerInvites.Add(invite);
		await _context.SaveChangesAsync();
		return invite;
	}

	public async Task<bool> InviteCodeInUseAsync(string code)
	{
		return await _context.ServerInvites.AnyAsync(i => i.Code == code);
	}
}
