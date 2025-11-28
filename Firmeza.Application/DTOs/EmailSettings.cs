namespace Firmeza.Application.DTOs;

/// <summary>
/// Configuration settings for email service
/// Loaded from appsettings.json EmailSettings section
/// </summary>
public class EmailSettings
{
    /// <summary>SMTP server hostname (e.g., smtp.gmail.com)</summary>
    public string SmtpHost { get; set; } = string.Empty;
    
    /// <summary>SMTP server port (typically 587 for TLS or 465 for SSL)</summary>
    public int SmtpPort { get; set; }
    
    /// <summary>Email address that appears as the sender</summary>
    public string SenderEmail { get; set; } = string.Empty;
    
    /// <summary>Display name for the sender</summary>
    public string SenderName { get; set; } = string.Empty;
    
    /// <summary>SMTP authentication username (often same as SenderEmail)</summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>SMTP authentication password (use app-specific password for Gmail)</summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>Whether to use SSL/TLS encryption</summary>
    public bool EnableSsl { get; set; } = true;
}
