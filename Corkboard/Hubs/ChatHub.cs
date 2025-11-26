using Corkboard.Data.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace Corkboard.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(MessageDto messageDto)
    {
        await Clients.All.SendAsync("ReceiveMessage", messageDto);
    }
}