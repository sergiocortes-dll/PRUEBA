using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/excel")]
public class ExcelImportController : ControllerBase
{
    private readonly IExcelImportService _excelImportService;
    private readonly IExcelFileReader _excelFileReader;

    public ExcelImportController(
        IExcelImportService excelImportService,
        IExcelFileReader excelFileReader)
    {
        _excelImportService = excelImportService;
        _excelFileReader = excelFileReader;
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No se proporcionó archivo" });

        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Solo se permiten archivos .xlsx" });

        using var stream = file.OpenReadStream();
        var isValid = _excelFileReader.ValidateExcelStructure(stream);

        return Ok(new
        {
            isValid,
            message = isValid ? "Estructura del Excel válida" : "Estructura del Excel inválida"
        });
    }
    
    [HttpPost("import")]
    public async Task<IActionResult> ImportEmployees(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No se proporcionó archivo" });

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _excelImportService.ImportEmployeesAsync(stream);

            if (result.Success)
            {
                return Ok(new
                {
                    success = true,
                    message = "Importación completada exitosamente",
                    data = result
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    message = "Importación completada con errores",
                    data = result
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Error durante la importación",
                error = ex.Message
            });
        }
    }
    
    [HttpGet("template")]
    public IActionResult DownloadTemplate()
    {
        // Crear un template básico
        using var package = new OfficeOpenXml.ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Empleados");
        
        // Encabezados
        var headers = new string[]
        {
            "Documento", "Nombres", "Apellidos", "FechaNacimiento", "Direccion",
            "Telefono", "Email", "Cargo", "Salario", "FechaIngreso", 
            "Estado", "NivelEducativo", "PerfilProfesional", "Departamento"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        // Ejemplos de datos
        worksheet.Cells[2, 1].Value = "12345678";
        worksheet.Cells[2, 2].Value = "Juan";
        worksheet.Cells[2, 3].Value = "Pérez";
        worksheet.Cells[2, 4].Value = "1990-01-15";
        worksheet.Cells[2, 5].Value = "Calle Falsa 123";
        worksheet.Cells[2, 6].Value = "3001234567";
        worksheet.Cells[2, 7].Value = "juan.perez@email.com";
        worksheet.Cells[2, 8].Value = "Desarrollador";
        worksheet.Cells[2, 9].Value = 3000000;
        worksheet.Cells[2, 10].Value = "2020-01-01";
        worksheet.Cells[2, 11].Value = "Activo";
        worksheet.Cells[2, 12].Value = "Profesional";
        worksheet.Cells[2, 13].Value = "Enfoque analítico y habilidades de comunicación";
        worksheet.Cells[2, 14].Value = "Tecnología";

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;

        return File(stream, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            "Template_Empleados.xlsx");
    }
}