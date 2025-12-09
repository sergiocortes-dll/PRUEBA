using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IEmailRepository
{
    Task<Email> GetByIdAsync(Guid id);
    Task<List<Email>> GetPendingEmailsAsync();
    Task<Email> AddAsync(Email email);
    Task UpdateAsync(Email email);
    Task<List<Email>> GetFailedEmailsForRetryAsync(int maxRetries);
}