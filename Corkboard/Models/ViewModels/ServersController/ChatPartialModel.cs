using Corkboard.Data.DTOs;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models.ViewModels.ServersController;

[NotMapped]
public class ChatPartialModel
{
    public int ServerId { get; set; }
    public int ChannelId { get; set; }
    public List<MessageDto> Messages { get; set; } = new();
}
