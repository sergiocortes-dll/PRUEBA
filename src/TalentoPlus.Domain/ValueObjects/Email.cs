namespace TalentoPlus.Domain.ValueObjects;

public record Email
{
    public string To { get; init; }
    public string? Cc { get; init; }
    public string? Bcc { get; init; }
    public string Subject { get; init; }
    public string Body { get; init; }
    public bool IsHtml { get; init; }
    public List<EmailAttachment>? Attachments { get; init; }

    public Email(string to, string subject, string body, bool isHtml = true)
    {
        if (string.IsNullOrWhiteSpace(to))
            throw new ArgumentException("Email recipient is required", nameof(to));
        
        if (string.IsNullOrWhiteSpace(subject))
            throw new ArgumentException("Email subject is required", nameof(subject));

        To = to;
        Subject = subject;
        Body = body;
        IsHtml = isHtml;
    }
}

public record EmailAttachment(string FileName, byte[] Content, string ContentType);