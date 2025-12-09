using TalentoPlus.Domain.ValueObjects;

namespace TalentoPlus.Domain.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task SendBulkEmailAsync(IEnumerable<Email> emails, CancellationToken cancellationToken = default);
}