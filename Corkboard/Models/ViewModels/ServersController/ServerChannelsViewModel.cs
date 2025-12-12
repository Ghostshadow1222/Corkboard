using Corkboard.Data.DTOs;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models.ViewModels.ServersController;

/// <summary>
/// View model for the server channels page, including navigation and current messages.
/// </summary>
[NotMapped]
public class ServerChannelsViewModel
{
    /// <summary>
    /// The current server identifier.
    /// </summary>
    public int ServerId { get; set; }

    /// <summary>
    /// The currently selected channel identifier, if any.
    /// </summary>
    public int? SelectedChannelId { get; set; }

    /// <summary>
    /// The display name of the current server.
    /// </summary>
    public string ServerName { get; set; } = string.Empty;

    /// <summary>
    /// List of channels available within the server.
    /// </summary>
    public List<ChannelViewModel> Channels { get; set; } = new();

    /// <summary>
    /// List of servers the current user belongs to for quick navigation.
    /// </summary>
    public List<ServerListItemViewModel> UserServers { get; set; } = new();

    /// <summary>
    /// The messages for the selected channel.
    /// </summary>
    public List<MessageDto> Messages { get; set; } = new();
}
