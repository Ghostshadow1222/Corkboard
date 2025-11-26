using Corkboard.Data.DTOs;
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

[NotMapped]
public class ChannelViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string IconUrl { get; set; } = string.Empty;
}

[NotMapped]
public class ServerChannelsViewModel
{
    public int ServerId { get; set; }

    public int ChannelId { get; set; }

    public string ServerName { get; set; } = string.Empty;

    public List<ChannelViewModel> Channels { get; set; } = new();

    public List<ServerListItemViewModel> UserServers { get; set; } = new();

    public List<MessageDto> Messages { get; set; } = new();
}

[NotMapped]
public class ServerListItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? IconUrl { get; set; }
}

[NotMapped]
public class ChatPartialModel
{
    public int ServerId { get; set; }
    public int ChannelId { get; set; }
    public List<MessageDto> Messages { get; set; } = new();
}