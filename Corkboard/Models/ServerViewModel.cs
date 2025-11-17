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
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Url]
    [MaxLength(2048)]
    public string? IconUrl { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}
