using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models.ViewModels.ServersController;

/// <summary>
/// Lightweight view model representing a server in a sidebar list.
/// </summary>
[NotMapped]
public class ServerListItemViewModel
{
    /// <summary>
    /// Server identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Display name of the server.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional URL to the server icon.
    /// </summary>
    public string? IconUrl { get; set; }
}
