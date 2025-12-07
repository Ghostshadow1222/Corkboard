using Corkboard.Data.DTOs;
using Corkboard.Models;
using Corkboard.Data.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace Corkboard.Hubs;

public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IServerService _serverService;
    private readonly IChannelService _channelService;

    public ChatHub(IMessageService messageService, IServerService serverService, IChannelService channelService)
    {
        _messageService = messageService;
        _serverService = serverService;
        _channelService = channelService;
    }

    public async Task JoinChannel(int channelId)
    {
        
        string? userId = Context.UserIdentifier;
        if (userId == null)
        {
            throw new HubException("Not authenticated.");
        }

        // Authorize user membership in channel
        Channel? channel = await _channelService.GetChannelAsync(channelId);
        if (channel == null)
        {
            throw new HubException("Channel not found.");
        }

        bool isMember = await _serverService.IsUserMemberOfServerAsync(channel.ServerId, userId);
        if (!isMember)
        {
            throw new HubException("Access denied to channel.");
        }
        
        string groupName = $"channel:{channelId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveChannel(int channelId)
    {
        string groupName = $"channel:{channelId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task SendMessage(int channelId, string messageContent)
    {
        string? userId = Context.UserIdentifier;
        if (userId == null)
        {
            throw new HubException("Not authenticated.");
        }
        // Verify membership before sending
        Channel? channel = await _channelService.GetChannelAsync(channelId);
        if (channel == null) throw new HubException("Channel not found.");

        bool isMember = await _serverService.IsUserMemberOfServerAsync(channel.ServerId, userId);
        if (!isMember) 
        {
            throw new HubException("Access denied to channel.");
        }

        // Create Message and MessageDto instances
        Message? newMessage = new Message
        {
            ChannelId = channelId,
            SenderId = Context.UserIdentifier!,
            MessageContent = messageContent,
        };

        // Save the new message to the database
        newMessage = await _messageService.SaveMessageAsync(newMessage);
        if (newMessage == null)
        {
            throw new HubException("Failed to save message.");
        }

        // Avoid accessing the navigation property `Sender` which may not be loaded
        // Use the current principal's name when available, otherwise fall back to the SenderId
        string senderName = Context.User?.Identity?.Name ?? newMessage.SenderId;

        MessageDto newMessageDto = new MessageDto
        {
            Text = newMessage.MessageContent,
            SenderUsername = senderName,
            Timestamp = newMessage.CreatedAt
        };

        string groupName = $"channel:{channelId}";
        await Clients.Group(groupName).SendAsync("ReceiveMessage", newMessageDto);
    }
}