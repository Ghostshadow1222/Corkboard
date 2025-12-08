using Corkboard.Data.DTOs;
using Corkboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Corkboard.Data.Services;

/// <summary>
/// Service contract for retrieving messages for channels.
/// </summary>
public interface IMessageService
{
	/// <summary>
    /// Saves a new message to the database.
    /// </summary>
    /// <param name="message">The message to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
	Task<Message?> SaveMessageAsync(Message message);

	/// <summary>
	/// Retrieves DTOs for messages in the specified channel ordered chronologically.
	/// </summary>
	/// <param name="channelId">Channel id.</param>
	/// <param name="limit">Maximum number of messages to retrieve.</param>
	/// <returns>List of message DTOs.</returns>
	Task<List<Message>> GetMessagesForChannelAsync(int channelId, int? limit = null);

	/// <summary>
	/// Retrieves DTOs for messages in the specified channel before a given timestamp.
	/// </summary>
	/// <param name="channelId">Channel id.</param>
	/// <param name="beforeTimestamp">Retrieve messages created before this timestamp.</param>
	/// <param name="limit">Maximum number of messages to retrieve.</param>
	/// <returns>List of message DTOs ordered chronologically.</returns>
	Task<List<Message>> GetMessagesBeforeTimestampAsync(int channelId, DateTime beforeTimestamp, int limit = 50);
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
	public async Task<List<Message>> GetMessagesForChannelAsync(int channelId, int? limit = null)
	{
		List<Message> messages = await _context.Messages
			.Where(m => m.ChannelId == channelId)
			.Include(m => m.Sender)
			.OrderByDescending(m => m.CreatedAt)
			.Take(limit ?? int.MaxValue)
			.ToListAsync();

		// Reverse to get chronological order (oldest to newest)
		messages.Reverse();
		return messages;
	}

	/// <inheritdoc/>
	public async Task<List<Message>> GetMessagesBeforeTimestampAsync(int channelId, DateTime beforeTimestamp, int limit = 50)
	{
		// Fetch messages older than the given timestamp, ordered descending, then reverse
		List<Message> messages = await _context.Messages
			.Where(m => m.ChannelId == channelId && m.CreatedAt < beforeTimestamp)
			.Include(m => m.Sender)
			.OrderByDescending(m => m.CreatedAt)
			.Take(limit)
			.ToListAsync();

		// Reverse to get chronological order (oldest to newest)
		messages.Reverse();
		return messages;
	}

	/// <inheritdoc/>
	public async Task<Message?> SaveMessageAsync(Message message)
    {
		// Validate channel exists
		Channel? channel = await _context.Channels.FindAsync(message.ChannelId);
		if (channel == null)
		{
			throw new InvalidOperationException($"Channel with ID {message.ChannelId} not found.");
		}
		// Validate server exists
		Server? server = await _context.Servers.FindAsync(channel.ServerId);
		if (server == null)
		{
			throw new InvalidOperationException($"Server with ID {channel.ServerId} not found.");
		}

		server.LastMessageTimeStamp = message.CreatedAt;
		_context.Messages.Add(message);
		await _context.SaveChangesAsync();
		return message;
    }
}