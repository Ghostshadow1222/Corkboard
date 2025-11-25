using Corkboard.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Corkboard.Data.Services;

/// <summary>
/// Service contract for retrieving messages for channels.
/// </summary>
public interface IMessageService
{
	/// <summary>
	/// Retrieves DTOs for messages in the specified channel ordered chronologically.
	/// </summary>
	/// <param name="channelId">Channel id.</param>
	/// <returns>List of message DTOs.</returns>
	Task<List<MessageDto>> GetMessagesForChannelAsync(int channelId);
}

/// <summary>
/// Concrete implementation of <see cref="IMessageService"/> for retrieving messages.
/// </summary>
public class MessageService : IMessageService
{
	private readonly ApplicationDbContext _context;

	/// <summary>
	/// Creates a new instance of <see cref="MessageService"/>.
	/// </summary>
	/// <param name="context">Application DbContext (injected).</param>
	public MessageService(ApplicationDbContext context)
	{
		_context = context;
	}

	/// <inheritdoc/>
	public async Task<List<MessageDto>> GetMessagesForChannelAsync(int channelId)
	{
		return await _context.Messages
			.Where(m => m.ChannelId == channelId)
			.Include(m => m.Sender)
			.OrderBy(m => m.CreatedAt)
			.Select(m => new MessageDto
			{
				Text = m.Content,
				Sender = m.Sender.UserName ?? "Unknown",
				ImageUrl = m.Sender.ProfilePictureUrl,
				Timestamp = m.CreatedAt
			})
			.ToListAsync();
	}
}
