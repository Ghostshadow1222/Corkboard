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
	/// <returns>List of message DTOs.</returns>
	Task<List<MessageDto>> GetMessagesForChannelAsync(int channelId, int? limit = null);
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
	public async Task<List<MessageDto>> GetMessagesForChannelAsync(int channelId, int? limit = null)
	{
		List<MessageDto> messages = await _context.Messages
			.Where(m => m.ChannelId == channelId)
			.Include(m => m.Sender)
			.OrderBy(m => m.CreatedAt)
			.Take(limit ?? int.MaxValue)
			.Select(m => new MessageDto
			{
				Text = m.MessageContent,
				SenderUsername = m.Sender.UserName!,
				Timestamp = m.CreatedAt
			})
			.ToListAsync();

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