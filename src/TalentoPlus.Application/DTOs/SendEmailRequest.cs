namespace TalentoPlus.Application.DTOs.Employee;

public record SendEmailRequest(
    string From,
    List<string> To,
    string Subject,
    string Body,
    bool IsHtml = true,
    List<string>? Cc = null,
    List<string>? Bcc = null
);

public record SendEmailResponse(
    Guid EmailId,
    string Status,
    string Message
);