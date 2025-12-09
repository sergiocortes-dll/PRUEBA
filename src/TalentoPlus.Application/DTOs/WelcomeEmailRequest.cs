namespace TalentoPlus.Application.DTOs.Employee;

public record WelcomeEmailRequest(
    string Email,
    string FullName,
    DateTime HireDate
);

public record CreateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string DocumentNumber,
    DateTime HireDate
);