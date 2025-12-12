using Corkboard.Data.DTOs;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models.ViewModels.ServersController;

/// <summary>
/// View model for rendering a chat partial with messages for a specific server/channel.
/// </summary>
[NotMapped]
public class ChatPartialModel
{
    /// <summary>
    /// The server identifier.
    /// </summary>
    public int ServerId { get; set; }

    /// <summary>
    /// The channel identifier.
    /// </summary>
    public int ChannelId { get; set; }

    /// <summary>
    /// The messages to display within the chat partial.
    /// </summary>
    public List<MessageDto> Messages { get; set; } = new();
}
