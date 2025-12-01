using Corkboard.Data.DTOs;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models.ViewModels.ServersController;

[NotMapped]
public class ServerChannelsViewModel
{
    public int ServerId { get; set; }

    public int ChannelId { get; set; }

    public string ServerName { get; set; } = string.Empty;

    public List<ChannelViewModel> Channels { get; set; } = new();

    public List<ServerListItemViewModel> UserServers { get; set; } = new();

    public List<MessageDto> Messages { get; set; } = new();
}
