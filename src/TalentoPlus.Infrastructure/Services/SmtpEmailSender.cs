using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Configuration;

namespace TalentoPlus.Infrastructure.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailConfiguration _config;

    public SmtpEmailSender(IOptions<EmailConfiguration> config)
    {
        _config = config.Value ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task SendAsync(Email email)
    {
        using var mailMessage = new MailMessage
        {
            From = new MailAddress(email.From),
            Subject = email.Subject,
            Body = email.Body,
            IsBodyHtml = email.IsHtml
        };

        foreach (var to in email.To)
            mailMessage.To.Add(to);

        foreach (var cc in email.Cc)
            mailMessage.CC.Add(cc);

        foreach (var bcc in email.Bcc)
            mailMessage.Bcc.Add(bcc);

        using var smtpClient = new SmtpClient(_config.SmtpHost, _config.SmtpPort)
        {
            Credentials = new NetworkCredential(_config.Username, _config.Password),
            EnableSsl = _config.EnableSsl
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}