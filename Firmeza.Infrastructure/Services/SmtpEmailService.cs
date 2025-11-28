using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Application.Features.Email.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Firmeza.Infrastructure.Services;

/// <summary>
/// SMTP-based email service implementation using Gmail
/// Designed to be easily replaceable with enterprise SMTP providers
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailSettings> emailSettings, ILogger<SmtpEmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Sends an email using SMTP protocol
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body content</param>
    /// <param name="isHtml">Whether the body contains HTML content</param>
    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            // Validate settings
            if (string.IsNullOrEmpty(_emailSettings.SmtpHost) || string.IsNullOrEmpty(_emailSettings.SenderEmail))
            {
                _logger.LogError("Email settings are not configured correctly. Host: {Host}, Sender: {Sender}", 
                    _emailSettings.SmtpHost, _emailSettings.SenderEmail);
                return;
            }

            // Configure SMTP client with settings from configuration
            using var smtpClient = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort);
            smtpClient.EnableSsl = _emailSettings.EnableSsl;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            // Create email message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            
            mailMessage.To.Add(to);

            // Send email asynchronously
            await smtpClient.SendMailAsync(mailMessage);
            
            _logger.LogInformation("Email sent successfully to {Recipient} via {Host}:{Port}", to, _emailSettings.SmtpHost, _emailSettings.SmtpPort);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}. Host: {Host}, Port: {Port}, SSL: {Ssl}", 
                to, _emailSettings.SmtpHost, _emailSettings.SmtpPort, _emailSettings.EnableSsl);
            // We don't throw here to prevent crashing the background task or the request
            // throw new InvalidOperationException($"Failed to send email to {to}", ex);
        }
    }

    /// <summary>
    /// Sends a welcome email to newly registered users
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="userName">User's full name for personalization</param>
    public async Task SendWelcomeEmailAsync(string to, string userName)
    {
        var subject = "¡Bienvenido a Firmeza!";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #2c3e50;'>¡Bienvenido a Firmeza, {userName}!</h2>
                <p>Gracias por registrarte en nuestro sistema.</p>
                <p>Ahora puedes acceder a nuestra plataforma y comenzar a realizar tus compras.</p>
                <br/>
                <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
                <br/>
                <p style='color: #7f8c8d;'>Saludos,<br/>El equipo de Firmeza</p>
            </body>
            </html>";

        await SendEmailAsync(to, subject, body);
    }

    /// <summary>
    /// Sends a purchase confirmation email with order details
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="orderDetails">HTML-formatted order details table</param>
    public async Task SendPurchaseConfirmationAsync(string to, string orderDetails)
    {
        var subject = "Confirmación de Compra - Firmeza";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #27ae60;'>¡Compra Confirmada!</h2>
                <p>Tu pedido ha sido procesado exitosamente.</p>
                <h3>Detalles del Pedido:</h3>
                <div style='background-color: #ecf0f1; padding: 15px; border-radius: 5px;'>
                    {orderDetails}
                </div>
                <br/>
                <p>Recibirás una notificación cuando tu pedido esté listo para entrega.</p>
                <br/>
                <p style='color: #7f8c8d;'>Gracias por tu compra,<br/>El equipo de Firmeza</p>
            </body>
            </html>";

        await SendEmailAsync(to, subject, body);
    }
}
