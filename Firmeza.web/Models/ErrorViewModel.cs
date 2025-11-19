namespace Firmeza.web.Models;

/// <summary>
/// View model for error handling
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

