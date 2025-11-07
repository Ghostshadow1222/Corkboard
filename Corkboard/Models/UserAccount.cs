using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Corkboard.Models;

public class UserAccount : IdentityUser
{
    /// <summary>
    /// The user's full name (optional).
    /// </summary>
    [MaxLength(100)]
    public string? FullName { get; set; }

    /// <summary>
    /// Optional URL to the user's profile picture.
    /// </summary>
    [Url]
    [MaxLength(2048)]
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Short biography or user-provided description.
    /// </summary>
    [MaxLength(1000)]
    public string? Bio { get; set; }

    /// <summary>
    /// The user's major/department (optional).
    /// </summary>
    [MaxLength(100)]
    public string? Major { get; set; }
}
