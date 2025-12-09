using OfficeOpenXml;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Domain.Models;

namespace TalentoPlus.Infrastructure.Services;

public class ExcelFileReader : IExcelFileReader
{
    public ExcelFileReader()
    {
        // Establecer el contexto de licencia para EPPlus
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<List<EmployeeExcelDto>> ReadEmployeesAsync(Stream fileStream)
    {
        var employees = new List<EmployeeExcelDto>();

        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets["Empleados"];
        
        if (worksheet == null)
        {
            throw new InvalidOperationException("No se encontró la hoja 'Empleados' en el archivo Excel");
        }

        var rowCount = worksheet.Dimension.Rows;
        
        // Empezar desde la fila 2 (la fila 1 es el encabezado)
        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var employee = new EmployeeExcelDto
                {
                    Documento = GetCellValue(worksheet, row, 1),
                    Nombres = GetCellValue(worksheet, row, 2),
                    Apellidos = GetCellValue(worksheet, row, 3),
                    FechaNacimiento = ParseDate(GetCellValue(worksheet, row, 4)),
                    Direccion = GetCellValue(worksheet, row, 5),
                    Telefono = GetCellValue(worksheet, row, 6),
                    Email = GetCellValue(worksheet, row, 7),
                    Cargo = GetCellValue(worksheet, row, 8),
                    Salario = ParseDecimal(GetCellValue(worksheet, row, 9)),
                    FechaIngreso = ParseDate(GetCellValue(worksheet, row, 10)),
                    Estado = GetCellValue(worksheet, row, 11),
                    NivelEducativo = GetCellValue(worksheet, row, 12),
                    PerfilProfesional = GetCellValue(worksheet, row, 13),
                    Departamento = GetCellValue(worksheet, row, 14)
                };

                // Validar datos básicos
                if (!string.IsNullOrWhiteSpace(employee.Documento) && 
                    !string.IsNullOrWhiteSpace(employee.Nombres) &&
                    !string.IsNullOrWhiteSpace(employee.Apellidos))
                {
                    employees.Add(employee);
                }
            }
            catch (Exception ex)
            {
                // Registrar error pero continuar con las siguientes filas
                Console.WriteLine($"Error en fila {row}: {ex.Message}");
            }
        }

        return employees;
    }

    public bool ValidateExcelStructure(Stream fileStream)
    {
        try
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets["Empleados"];
            
            if (worksheet == null)
                return false;

            // Verificar encabezados esperados
            var expectedHeaders = new List<string>
            {
                "Documento", "Nombres", "Apellidos", "FechaNacimiento", "Direccion",
                "Telefono", "Email", "Cargo", "Salario", "FechaIngreso", 
                "Estado", "NivelEducativo", "PerfilProfesional", "Departamento"
            };

            for (int i = 0; i < expectedHeaders.Count; i++)
            {
                var header = GetCellValue(worksheet, 1, i + 1);
                if (!expectedHeaders[i].Equals(header, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private string GetCellValue(ExcelWorksheet worksheet, int row, int col)
    {
        return worksheet.Cells[row, col]?.Text?.Trim() ?? string.Empty;
    }

    private DateTime ParseDate(string dateString)
    {
        if (DateTime.TryParse(dateString, out var date))
            return date;
        
        // Intentar con formato específico si el parseo normal falla
        if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out date))
            return date;
        
        return DateTime.MinValue;
    }

    private decimal ParseDecimal(string decimalString)
    {
        if (decimal.TryParse(decimalString, out var value))
            return value;
        return 0;
    }
}