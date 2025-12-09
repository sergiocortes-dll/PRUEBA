using Microsoft.Extensions.Logging;
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
    }

    public async Task<byte[]> GenerateEmployeePdfAsync(int employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
            throw new ArgumentException($"Empleado con ID {employeeId} no encontrado");

        // Por ahora, generamos un PDF simple con texto
        // En producción, usar una librería como QuestPDF o iTextSharp
        
        var pdfContent = $@"
            HOJA DE VIDA - TALENTOPLUS
            ============================
            
            DATOS PERSONALES:
            ------------------
            Documento: {employee.Document}
            Nombre: {employee.Name} {employee.Surname}
            Fecha de Nacimiento: {employee.BirthDate:dd/MM/yyyy}
            Dirección: {employee.Address}
            Teléfono: {employee.Phone}
            Email: {employee.Email}
            
            INFORMACIÓN LABORAL:
            --------------------
            Cargo: [Información del cargo]
            Salario: {GetPropertyValue(employee, "Salary")}
            Fecha de Ingreso: {GetPropertyValue(employee, "HireDate")}
            Estado: {GetPropertyValue(employee, "Status")}
            
            EDUCACIÓN:
            ----------
            Nivel Educativo: [Información del nivel educativo]
            
            PERFIL PROFESIONAL:
            -------------------
            {GetPropertyValue(employee, "ProfessionalProfile")}
            
            DEPARTAMENTO:
            -------------
            [Información del departamento]
            
            Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}
        ";

        // Convertir a bytes (en producción, usar librería de PDF)
        var bytes = System.Text.Encoding.UTF8.GetBytes(pdfContent);
        
        _logger.LogInformation($"PDF generado para empleado {employeeId}");
        
        return bytes;
    }

    private string GetPropertyValue(object obj, string propertyName)
    {
        var property = obj.GetType().GetProperty(propertyName);
        if (property != null)
        {
            var value = property.GetValue(obj);
            return value?.ToString() ?? "No especificado";
        }
        return "No especificado";
    }
}