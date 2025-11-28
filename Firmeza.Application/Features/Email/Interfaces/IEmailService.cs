namespace Firmeza.Application.Features.Email.Interfaces;

/// <summary>
/// Interface for email service operations
/// Designed to be easily replaceable with different SMTP providers
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email asynchronously
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body (supports HTML)</param>
    /// <param name="isHtml">Whether the body contains HTML</param>
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    
    /// <summary>
    /// Sends a welcome email to a new user
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="userName">User's name</param>
    Task SendWelcomeEmailAsync(string to, string userName);
    
    /// <summary>
    /// Sends a purchase confirmation email
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="orderDetails">Order details to include in email</param>
    Task SendPurchaseConfirmationAsync(string to, string orderDetails);
}
