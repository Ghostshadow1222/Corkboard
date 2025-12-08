using System.ComponentModel.DataAnnotations;

namespace Corkboard.Models.ViewModels.ServersController;

/// <summary>
/// View model used when creating or editing a server from a form.
/// </summary>
public class ServerViewModel
{
	/// <summary>
	/// Server ID (only required for edit scenarios).
	/// </summary>
	public int? Id { get; set; }

	/// <summary>
	/// Display name of the server.
	/// </summary>
	[Required]
	[MaxLength(100, ErrorMessage = "Server name cannot exceed 100 characters.")]
	[Display(Name = "Server Name")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Optional URL to an icon image for the server.
	/// </summary>
	[Url(ErrorMessage = "Please enter a valid URL.")]
	[MaxLength(2048)]
	[Display(Name = "Icon URL")]
	public string? IconUrl { get; set; }

	/// <summary>
	/// Optional textual description of the server.
	/// </summary>
	[MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
	[Display(Name = "Description")]
	public string? Description { get; set; }

	/// <summary>
	/// Privacy level of the server.
	/// </summary>
	[Required]
	[Display(Name = "Privacy Level")]
	public PrivacyLevel PrivacyLevel { get; set; } = PrivacyLevel.Public;
}
