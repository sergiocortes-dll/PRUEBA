using Microsoft.Extensions.Logging;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Domain.ValueObjects;

namespace TalentoPlus.Application.Services;

public class NotificationService
{
    private IEmailService _emailService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IEmailService emailService, ILogger<NotificationService> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }
    
    public async Task NotifyUserRegistrationAsync(string userEmail, string userName)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("El correo del usuario es obligatorio.", nameof(userEmail));

        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("El nombre del usuario es obligatorio.", nameof(userName));

        // Validación de email más decente sin hacer un regex monstruoso
        try
        {
            var _ = new System.Net.Mail.MailAddress(userEmail);
        }
        catch
        {
            throw new ArgumentException("El correo del usuario no tiene un formato válido.", nameof(userEmail));
        }

        var email = new Email(
            to: userEmail,
            subject: "Bienvenido a nuestra aplicación",
            body: $"<h1>Hola {userName}</h1><p>Gracias por registrarte.</p>",
            isHtml: true
        );

        await _emailService.SendEmailAsync(email);

        _logger.LogInformation("Registration email sent to {Email}", userEmail);
    }
}