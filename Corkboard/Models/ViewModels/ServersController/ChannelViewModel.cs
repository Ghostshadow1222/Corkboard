using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models.ViewModels.ServersController;

/// <summary>
/// Lightweight view model representing a channel for listing and selection.
/// </summary>
[NotMapped]
public class ChannelViewModel
{
    /// <summary>
    /// Channel identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Display name of the channel.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
