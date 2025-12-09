using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class PdfService : IPdfService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<PdfService> _logger;

    public PdfService(IEmployeeRepository employeeRepository, ILogger<PdfService> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
        
        // Configuración de licencia (Community para uso gratuito)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateEmployeePdfAsync(int employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
            throw new ArgumentException($"Empleado con ID {employeeId} no encontrado");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(50);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Text("HOJA DE VIDA - TALENTOPLUS")
                    .FontSize(20)
                    .Bold()
                    .AlignCenter();

                page.Content().Column(column =>
                {
                    column.Spacing(10);

                    // Datos Personales
                    column.Item().Text("DATOS PERSONALES").Bold().FontSize(14);
                    column.Item().Text($"Documento: {employee.Document}");
                    column.Item().Text($"Nombre: {employee.Name} {employee.Surname}");
                    column.Item().Text($"Fecha de Nacimiento: {employee.BirthDate:dd/MM/yyyy}");
                    column.Item().Text($"Dirección: {employee.Address}");
                    column.Item().Text($"Teléfono: {employee.Phone}");
                    column.Item().Text($"Email: {employee.Email}");

                    column.Item().PaddingTop(20);

                    // Información Laboral
                    column.Item().Text("INFORMACIÓN LABORAL").Bold().FontSize(14);
                    column.Item().Text($"Cargo: {GetPropertyValue(employee, "Position") ?? "No especificado"}");
                    column.Item().Text($"Salario: {GetPropertyValue(employee, "Salary") ?? "No especificado"}");
                    column.Item().Text($"Estado: {GetPropertyValue(employee, "Status") ?? "Activo"}");
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm} - Página ");
                    text.CurrentPageNumber();
                });
            });
        });

        return document.GeneratePdf();
    }

    private string? GetPropertyValue(object obj, string propertyName)
    {
        var property = obj.GetType().GetProperty(propertyName);
        if (property != null)
        {
            var value = property.GetValue(obj);
            return value?.ToString();
        }
        return null;
    }
}