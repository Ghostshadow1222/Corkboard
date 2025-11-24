using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models;

/// <summary>
/// View model used when creating or editing a server from a form.
/// Contains only the fields the user can submit.
/// </summary>
[NotMapped]
public class ServerViewModel
{
    /// <summary>
    /// Display name of the server.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional URL to an icon image for the server.
    /// </summary>
    [Url]
    [MaxLength(2048)]
    public string? IconUrl { get; set; }

    /// <summary>
    /// Optional textual description of the server.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }
}
