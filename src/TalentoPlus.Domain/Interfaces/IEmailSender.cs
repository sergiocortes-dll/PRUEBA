using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IEmailSender
{
    Task SendAsync(Email email);
}