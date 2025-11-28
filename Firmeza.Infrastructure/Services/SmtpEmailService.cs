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

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            using var smtpClient = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                EnableSsl = _emailSettings.EnableSsl,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            
            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
            
            _logger.LogInformation("Email sent successfully to {Recipient}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}", to);
            throw new InvalidOperationException($"Failed to send email to {to}", ex);
        }
    }

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
