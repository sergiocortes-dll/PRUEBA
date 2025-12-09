using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TalentoPlus.Application.DTOs.Employee;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Application.UseCases;

public class SendEmailUseCase
{
    private readonly IEmailRepository _emailRepository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<SendEmailUseCase> _logger;
    private readonly string _fromEmail;
    
    public SendEmailUseCase(
        IEmailRepository emailRepository, 
        IEmailSender emailSender,  
        ILogger<SendEmailUseCase> logger,
        IConfiguration configuration)
    {
        _emailRepository = emailRepository;
        _emailSender = emailSender;
        _logger = logger;
        _fromEmail = configuration["EmailConfiguration:Username"] 
                     ?? "contacto.sergiocortes@gmail.com";
    }
    
    public async Task<bool> SendWelcomeEmailAsync(string toEmail, string fullName, DateTime hireDate)
    {
        try
        {
            var subject = "¡Bienvenido a TalentoPlus!";
            var body = BuildWelcomeEmailBody(fullName, hireDate);

            // Crear entidad Email con DDD
            var email = new Email(
                from: _fromEmail,
                to: new List<string> { toEmail },
                subject: subject,
                body: body,
                isHtml: true
            );

            // Persistir email
            await _emailRepository.AddAsync(email);

            try
            {
                // Enviar email
                await _emailSender.SendAsync(email);
                email.MarkAsSent();
                
                _logger.LogInformation(
                    "Email de bienvenida enviado exitosamente a {Email} para {FullName}", 
                    toEmail, fullName);
            }
            catch (Exception ex)
            {
                email.MarkAsFailed(ex.Message);
                _logger.LogError(ex, 
                    "Error al enviar email de bienvenida a {Email}", toEmail);
            }

            // Actualizar estado
            await _emailRepository.UpdateAsync(email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error crítico en SendWelcomeEmailUseCase para {Email}", toEmail);
            return false;
        }
    }
    public async Task<SendEmailResponse> ExecuteAsync(SendEmailRequest request)
    {
        try
        {
            var email = new Email(
                request.From,
                request.To,
                request.Subject,
                request.Body,
                request.IsHtml
            );

            if (request.Cc != null)
                foreach (var cc in request.Cc)
                    email.AddCc(cc);

            if (request.Bcc != null)
                foreach (var bcc in request.Bcc)
                    email.AddBcc(bcc);

            await _emailRepository.AddAsync(email);

            try
            {
                await _emailSender.SendAsync(email);
                email.MarkAsSent();
            }
            catch (Exception ex)
            {
                email.MarkAsFailed(ex.Message);
            }

            await _emailRepository.UpdateAsync(email);

            return new SendEmailResponse(
                email.Id,
                email.Status.ToString(),
                email.Status == EmailStatus.Sent ? "Email enviado exitosamente" : "Email falló al enviar"
            );
        }
        catch (Exception ex)
        {
            return new SendEmailResponse(
                Guid.Empty,
                "Error",
                $"Error al procesar el email: {ex.Message}"
            );
        }
    }

    private string BuildWelcomeEmailBody(string fullName, DateTime hireDate)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 0 0 5px 5px; }}
        .info-box {{ background-color: white; padding: 15px; margin: 20px 0; border-left: 4px solid #4CAF50; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>¡Bienvenido a TalentoPlus!</h1>
        </div>
        <div class='content'>
            <h2>Hola {fullName},</h2>
            <p>Nos complace darte la bienvenida a <strong>TalentoPlus</strong>. Tu registro ha sido completado exitosamente.</p>
            
            <div class='info-box'>
                <p><strong>📅 Fecha de Ingreso:</strong> {hireDate:dddd, dd MMMM yyyy}</p>
                <p><strong>✉️ Email registrado:</strong> Este correo</p>
            </div>
            
            <h3>Próximos Pasos:</h3>
            <ol>
                <li>Accede a la plataforma usando tu documento y email</li>
                <li>Completa tu perfil profesional</li>
                <li>Explora las oportunidades disponibles</li>
            </ol>
            
            <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
            
            <p>¡Éxitos en tu nuevo camino profesional!</p>
            
            <p>Saludos cordiales,<br>
            <strong>Equipo de TalentoPlus</strong></p>
        </div>
        <div class='footer'>
            <p>Este es un correo automático, por favor no responder.</p>
            <p>&copy; 2024 TalentoPlus. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }
}