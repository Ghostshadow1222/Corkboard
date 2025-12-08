using System.ComponentModel.DataAnnotations;

namespace Corkboard.Models.ViewModels.ServersController;

/// <summary>
/// View model for editing server details.
/// </summary>
public class ServerFormViewModel
{
	/// <summary>
	/// Server ID.
	/// </summary>
	[Required]
	public int Id { get; set; }

	/// <summary>
	/// Name of the server.
	/// </summary>
	[Required]
	[StringLength(100, ErrorMessage = "Server name cannot exceed 100 characters.")]
	[Display(Name = "Server Name")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Description of the server.
	/// </summary>
	[StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
	[Display(Name = "Description")]
	public string? Description { get; set; }

	/// <summary>
	/// URL to the server's icon image.
	/// </summary>
	[Url(ErrorMessage = "Please enter a valid URL.")]
	[Display(Name = "Icon URL")]
	public string? IconUrl { get; set; }

	/// <summary>
	/// Privacy level of the server.
	/// </summary>
	[Required]
	[Display(Name = "Privacy Level")]
	public PrivacyLevel PrivacyLevel { get; set; }
}
