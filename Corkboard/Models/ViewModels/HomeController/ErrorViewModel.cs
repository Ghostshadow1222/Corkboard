namespace Corkboard.Models.ViewModels.HomeController;

/// <summary>
/// View model for displaying error information, including the current request ID.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// The ID of the current request for tracing and diagnostics.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Indicates whether a non-empty <see cref="RequestId"/> is available.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
