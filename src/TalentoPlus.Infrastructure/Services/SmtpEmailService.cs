using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Domain.ValueObjects;
using TalentoPlus.Infrastructure.Configuration;

namespace TalentoPlus.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;
    
    public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }
    
    public async Task SendEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = email.IsHtml
            };

            message.To.Add(email.To);
            
            if (!string.IsNullOrEmpty(email.Cc))
                message.CC.Add(email.Cc);
                
            if (!string.IsNullOrEmpty(email.Bcc))
                message.Bcc.Add(email.Bcc);

            // Agregar attachments
            if (email.Attachments != null)
            {
                foreach (var attachment in email.Attachments)
                {
                    var stream = new MemoryStream(attachment.Content);
                    message.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.ContentType));
                }
            }

            using var smtpClient = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            await smtpClient.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Email sent successfully to {To}", email.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To}", email.To);
            throw;
        }
    }

    public async Task SendBulkEmailAsync(IEnumerable<Email> emails, CancellationToken cancellationToken = default)
    {
        var tasks = emails.Select(email => SendEmailAsync(email, cancellationToken));
        await Task.WhenAll(tasks);
    }
}