using System.ComponentModel.DataAnnotations;

namespace Corkboard.Models.ViewModels.ServersController;

/// <summary>
/// View model for creating or editing a channel.
/// </summary>
public class ChannelFormViewModel
{
	/// <summary>
	/// Channel ID (for edit operations).
	/// </summary>
	public int? Id { get; set; }

	/// <summary>
	/// Server ID where the channel belongs.
	/// </summary>
	[Required]
	public int ServerId { get; set; }

	/// <summary>
	/// Name of the channel.
	/// </summary>
	[Required]
	[StringLength(100, ErrorMessage = "Channel name cannot exceed 100 characters.")]
	[Display(Name = "Channel Name")]
	public string Name { get; set; } = string.Empty;
}
