using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models.ViewModels.ServersController;

[NotMapped]
public class ChannelViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
