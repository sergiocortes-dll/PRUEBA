using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Domain.Entities;

public class Email
{
    public Guid Id { get; private set; }
    public string From { get; private set; }
    public List<string> To { get; private set; }
    public List<string> Cc { get; private set; }
    public List<string> Bcc { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public bool IsHtml { get; private set; }
    public EmailStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }

    private Email() { } // Para EF Core

    public Email(string from, List<string> to, string subject, string body, bool isHtml = true)
    {
        Id = Guid.NewGuid();
        From = from ?? throw new ArgumentNullException(nameof(from));
        To = to ?? throw new ArgumentNullException(nameof(to));
        Cc = new List<string>();
        Bcc = new List<string>();
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        Body = body ?? throw new ArgumentNullException(nameof(body));
        IsHtml = isHtml;
        Status = EmailStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        RetryCount = 0;

        ValidateEmail();
    }

    public void AddCc(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email CC no puede estar vacío");
        Cc.Add(email);
    }

    public void AddBcc(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email BCC no puede estar vacío");
        Bcc.Add(email);
    }

    public void MarkAsSent()
    {
        Status = EmailStatus.Sent;
        SentAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = EmailStatus.Failed;
        ErrorMessage = errorMessage;
        RetryCount++;
    }

    public void MarkAsRetrying()
    {
        Status = EmailStatus.Retrying;
    }

    private void ValidateEmail()
    {
        if (To == null || !To.Any())
            throw new ArgumentException("Debe haber al menos un destinatario");

        if (string.IsNullOrWhiteSpace(From))
            throw new ArgumentException("El remitente es requerido");

        if (string.IsNullOrWhiteSpace(Subject))
            throw new ArgumentException("El asunto es requerido");
    }
}