using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corkboard.Models;

public class ServerInviteViewModel
{
	/// <summary>
	/// Foreign key to the <see cref="Server"/> this invite is for.
	/// </summary>
	[Required]
	public int ServerId { get; set; }

	/// <summary>
	/// Foreign key to the <see cref="UserAccount"/> who is invited to the server (optional).
	/// </summary>
	public string? UserId { get; set; }

	public int DaysUntilExpires { get; set; } = 7;

	public bool OneTimeUse { get; set; } = true;
}
