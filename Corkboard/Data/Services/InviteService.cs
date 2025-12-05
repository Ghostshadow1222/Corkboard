using Corkboard.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Corkboard.Data.Services;

/// <summary>
/// Service contract for server invite-related operations including creation, retrieval, and redemption.
/// </summary>
public interface IInviteService
{
	/// <summary>
	/// Retrieves a server invite by its identifier, including related server data.
	/// </summary>
	/// <param name="id">Invite id.</param>
	/// <returns>The invite or <c>null</c> if not found.</returns>
	public Task<ServerInvite?> GetInviteAsync(int id);

	/// <summary>
	/// Gets all invites created by a specific user, including server and creator information.
	/// </summary>
	/// <param name="userId">The user's Id.</param>
	/// <returns>List of invites created by the user.</returns>
	public Task<System.Collections.Generic.List<ServerInvite>> GetInvitesCreatedByUserAsync(string userId);

	/// <summary>
	/// Creates a new server invite and persists it to the database.
	/// </summary>
	/// <param name="invite">The invite entity to create.</param>
	/// <returns>The created invite with generated Id populated.</returns>
	public Task<ServerInvite> CreateInviteAsync(ServerInvite invite);

	/// <summary>
	/// Attempts to redeem an invite for a user, creating a server membership if valid.
	/// Handles validation for expiration, usage limits, and duplicate memberships.
	/// </summary>
	/// <param name="inviteId">The invite id to redeem.</param>
	/// <param name="userId">The user attempting to redeem the invite.</param>
	/// <returns>The created server membership or <c>null</c> if redemption failed.</returns>
	public Task<ServerMember?> RedeemInviteAsync(int inviteId, string userId);

	/// <summary>
	/// Checks whether an invite code is already in use (case-insensitive).
	/// </summary>
	/// <param name="code">The invite code to check.</param>
	/// <returns><c>true</c> if the code exists; otherwise <c>false</c>.</returns>
	public Task<bool> InviteCodeInUseAsync(string code);

	/// <summary>
	/// Retrieves an invite by its code (case-insensitive), including server information.
	/// </summary>
	/// <param name="code">The invite code to look up.</param>
	/// <returns>The invite or <c>null</c> if not found.</returns>
	public Task<ServerInvite?> GetInviteByCodeAsync(string code);

	/// <summary>
	/// Generates a unique invite code of the specified length that is not already in use.
	/// </summary>
	/// <param name="length">The desired length of the invite code.</param>
	/// <returns>A unique invite code string.</returns>
	public Task<string> GenerateUniqueInviteCodeAsync(int length = 8);

	/// <summary>
	/// Validates all authorization and business rules for creating a server invite.
	/// Checks server existence, privacy level permissions, invited user validation, and expiry rules.
	/// </summary>
	/// <param name="currentUserId">The ID of the user attempting to create the invite.</param>
	/// <param name="invite">The invite view model with creation details.</param>
	/// <returns>Validation result with errors, authorized status, and validated data.</returns>
	public Task<InviteValidationResult> ValidateCreateInviteAsync(string currentUserId, ServerInviteViewModel invite);
}

/// <summary>
/// Concrete implementation of <see cref="IInviteService"/> for managing server invites.
/// </summary>
public class InviteService : IInviteService
{
	private readonly ApplicationDbContext _context;
	private readonly Microsoft.AspNetCore.Identity.UserManager<UserAccount> _userManager;

	/// <summary>
	/// Creates a new instance of <see cref="InviteService"/>.
	/// </summary>
	/// <param name="context">Application DbContext (injected).</param>
	/// <param name="userManager">User manager for identity operations (injected).</param>
	public InviteService(ApplicationDbContext context, Microsoft.AspNetCore.Identity.UserManager<UserAccount> userManager)
	{
		_context = context;
		_userManager = userManager;
	}

	/// <inheritdoc/>
	public async Task<System.Collections.Generic.List<ServerInvite>> GetInvitesCreatedByUserAsync(string userId)
	{
		if (string.IsNullOrWhiteSpace(userId)) return new System.Collections.Generic.List<ServerInvite>();
		return await _context.ServerInvites
			.Include(i => i.Server)
			.Include(i => i.CreatedBy)
			.Where(i => i.CreatedById == userId)
			.ToListAsync();
	}

	/// <inheritdoc/>
	public async Task<ServerInvite?> GetInviteAsync(int id)
	{
		// Include the Server navigation property so views can access Model.Server safely
		return await _context.ServerInvites
			.Include(i => i.Server)
			.FirstOrDefaultAsync(i => i.Id == id);
	}

	/// <inheritdoc/>
	public async Task<ServerInvite> CreateInviteAsync(ServerInvite invite)
	{
		_context.ServerInvites.Add(invite);
		await _context.SaveChangesAsync();
		return invite;
	}

	/// <inheritdoc/>
	public async Task<ServerMember?> RedeemInviteAsync(int inviteId, string userId)
	{
		// Use a transaction to reduce race conditions when redeeming invites.
		using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

		ServerInvite? invite = await _context.ServerInvites
			.Include(i => i.Server)
			.FirstOrDefaultAsync(i => i.Id == inviteId);

		if (invite == null)
		{
			return null;
		}

		// Reject if invite is one-time and already used
		if (invite.OneTimeUse && invite.IsUsed)
		{
			return null;
		}

		// Reject if expired
		if (invite.ExpiresAt != null && invite.ExpiresAt < DateTime.UtcNow)
		{
			return null;
		}

		// Don't add duplicate membership (fast pre-check)
		bool alreadyMember = await _context.ServerMembers
			.AnyAsync(m => m.ServerId == invite.ServerId && m.UserId == userId);
		if (alreadyMember)
		{
			return null;
		}

		ServerMember member = new ServerMember
		{
			InviteId = invite.Id,
			ServerId = invite.ServerId,
			UserId = userId,
			Role = RoleType.Member
		};

		_context.ServerMembers.Add(member);

		// Update invite usage metadata
		invite.TimesUsed += 1;
		if (invite.OneTimeUse)
		{
			invite.IsUsed = true;
		}

		try
		{
			await _context.SaveChangesAsync();
			await transaction.CommitAsync();
			return member;
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException)
		{
			// If a concurrent request inserted the same ServerMember, treat as already-member.
			await transaction.RollbackAsync();
			bool nowMember = await _context.ServerMembers.AnyAsync(m => m.ServerId == invite.ServerId && m.UserId == userId);
			if (nowMember)
			{
				return null;
			}
			// Unknown DB error - rethrow so callers can handle/log it.
			throw;
		}
	}

	/// <inheritdoc/>
	public async Task<ServerInvite?> GetInviteByCodeAsync(string code)
	{
		if (string.IsNullOrWhiteSpace(code)) return null;
		string normalized = code.Trim().ToUpper();
		return await _context.ServerInvites
			.Include(i => i.Server)
			.FirstOrDefaultAsync(i => i.InviteCode.ToUpper() == normalized);
	}

	/// <inheritdoc/>
	public async Task<bool> InviteCodeInUseAsync(string code)
	{
		if (string.IsNullOrWhiteSpace(code)) return false;
		string normalized = code.Trim().ToUpper();
		return await _context.ServerInvites.AnyAsync(i => i.InviteCode.ToUpper() == normalized);
	}

	/// <inheritdoc/>
	public async Task<string> GenerateUniqueInviteCodeAsync(int length = 8)
    {
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		const int maxRetries = 10;
		for (int attempt = 0; attempt < maxRetries; attempt++)
		{
			char[] result = new char[length];
			for (int i = 0; i < length; i++)
			{
				int idx = RandomNumberGenerator.GetInt32(chars.Length);
				result[i] = chars[idx];
			}

			string code = new string(result);
			if (!await InviteCodeInUseAsync(code))
			{
				return code;
			}
			// else, try again
		}
		throw new System.Exception($"Failed to generate a unique invite code after {maxRetries} retries.");
    }

	/// <inheritdoc/>
	public async Task<InviteValidationResult> ValidateCreateInviteAsync(string currentUserId, ServerInviteViewModel invite)
	{
		InviteValidationResult result = new InviteValidationResult();

		if (string.IsNullOrWhiteSpace(currentUserId))
		{
			result.IsValid = false;
			result.Unauthorized = true;
			return result;
		}

		Server? server = await _context.Servers.FindAsync(invite.ServerId);
		if (server == null)
		{
			result.IsValid = false;
			result.NotFound = true;
			return result;
		}

		// Check for owner-only privacy level
		if (server.PrivacyLevel == PrivacyLevel.OwnerInvitePrivate && server.OwnerId != currentUserId)
		{
			result.IsValid = false;
			result.Unauthorized = true;
			return result;
		}

		// Resolve invited username, if present
		if (!string.IsNullOrWhiteSpace(invite.Username))
		{
			string username = invite.Username.Trim();
			UserAccount? invitedUser = await _userManager.FindByNameAsync(username);
			if (invitedUser == null)
			{
				result.IsValid = false;
				result.FieldErrors["Username"] = "The specified user does not exist.";
				return result;
			}
			if (invitedUser.Id == currentUserId)
			{
				result.IsValid = false;
				result.FieldErrors["Username"] = "You cannot invite yourself.";
				return result;
			}
			if (invitedUser != null && invite.OneTimeUse == false)
			{
				result.IsValid = false;
				result.FieldErrors["OneTimeUse"] = "Invites for specific users cannot be multi-use.";
				return result;
			}
			result.InvitedUserId = invitedUser?.Id;
		}

		// Validate expiry
		if (invite.Expires && invite.ExpiresAt != null && invite.ExpiresAt <= DateTime.UtcNow)
		{
			result.IsValid = false;
			result.FieldErrors["ExpiresAt"] = "Expiration must be a future date/time.";
		}

		// If no expiry selected, keep null
		result.ValidatedExpiresAt = invite.Expires ? invite.ExpiresAt : null;

		return result;
	}
}