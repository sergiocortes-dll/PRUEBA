using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories;

public class EmailRepository : IEmailRepository
{
    private readonly ApplicationDbContext _context;

    public EmailRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Email> GetByIdAsync(Guid id)
    {
        return await _context.Emails.FindAsync(id) 
               ?? throw new KeyNotFoundException($"Email con ID {id} no encontrado");
    }

    public async Task<List<Email>> GetPendingEmailsAsync()
    {
        return await _context.Emails
            .Where(e => e.Status == EmailStatus.Pending)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<Email> AddAsync(Email email)
    {
        await _context.Emails.AddAsync(email);
        await _context.SaveChangesAsync();
        return email;
    }

    public async Task UpdateAsync(Email email)
    {
        _context.Emails.Update(email);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Email>> GetFailedEmailsForRetryAsync(int maxRetries)
    {
        return await _context.Emails
            .Where(e => e.Status == EmailStatus.Failed && e.RetryCount < maxRetries)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync();
    }
}