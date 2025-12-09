using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TalentoPlus.Infrastructure.Services;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    private readonly IExcelFileReader _excelFileReader;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DebugController> _logger;

    public DebugController(
        IExcelFileReader excelFileReader,
        ApplicationDbContext context,
        ILogger<DebugController> logger)
    {
        _excelFileReader = excelFileReader;
        _context = context;
        _logger = logger;
    }

    [HttpPost("read-excel")]
    public async Task<IActionResult> ReadExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No se proporcionó archivo" });

        try
        {
            using var stream = file.OpenReadStream();
            
            // Crear una instancia temporal del ExcelFileReader
            var reader = new ExcelFileReader();
            var employees = await reader.ReadEmployeesAsync(stream);
            
            var sampleData = employees.Take(5).Select((e, i) => new
            {
                Row = i + 2,
                Documento = e.Documento,
                Nombres = e.Nombres,
                Apellidos = e.Apellidos,
                FechaNacimiento = e.FechaNacimiento.ToString("yyyy-MM-dd"),
                Salario = e.Salario,
                SalarioRaw = e.Salario.ToString("N2"),
                Estado = e.Estado,
                NivelEducativo = e.NivelEducativo,
                Departamento = e.Departamento,
                IsValid = !string.IsNullOrWhiteSpace(e.Documento) && 
                         !string.IsNullOrWhiteSpace(e.Nombres) &&
                         !string.IsNullOrWhiteSpace(e.Apellidos)
            }).ToList();

            return Ok(new
            {
                totalRecords = employees.Count,
                validRecords = employees.Count(e => !string.IsNullOrWhiteSpace(e.Documento) && 
                                                   !string.IsNullOrWhiteSpace(e.Nombres) &&
                                                   !string.IsNullOrWhiteSpace(e.Apellidos)),
                sampleData,
                issues = employees.Where(e => string.IsNullOrWhiteSpace(e.Documento) || 
                                             string.IsNullOrWhiteSpace(e.Nombres) ||
                                             string.IsNullOrWhiteSpace(e.Apellidos))
                                 .Select(e => new { e.Documento, e.Nombres, e.Apellidos })
                                 .ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer el Excel");
            return StatusCode(500, new
            {
                error = ex.Message,
                details = ex.StackTrace
            });
        }
    }

    [HttpPost("test-parse")]
    public IActionResult TestParse()
    {
        // Test de los métodos de parseo
        var testCases = new[]
        {
            "4190629",
            "2.388.163",
            "1.825.425",
            "6.675.009",
            "2.896.984",
            "3.578.279",
            "abc",
            "",
            "1,500,000.50"
        };

        var results = testCases.Select(tc => new
        {
            Input = tc,
            Output = TestParseDecimal(tc),
            IsValid = decimal.TryParse(tc, out _)
        }).ToList();

        return Ok(results);
    }

    [HttpPost("insert-manual")]
    public async Task<IActionResult> InsertManual()
    {
        try
        {
            // Verificar que la base de datos tenga los datos de referencia
            var departmentExists = await _context.Departments.AnyAsync();
            var jobTitleExists = await _context.JobTitles.AnyAsync();
            var educationLevelExists = await _context.EducationLevels.AnyAsync();

            if (!departmentExists || !jobTitleExists || !educationLevelExists)
            {
                return BadRequest(new
                {
                    message = "Faltan datos de referencia. Ejecuta las migraciones primero.",
                    departmentExists,
                    jobTitleExists,
                    educationLevelExists
                });
            }

            // Tomar datos del primer registro del Excel manualmente
            var employee = new TalentoPlus.Domain.Entities.Employee(
                "96239103", // Documento
                "Laura",    // Nombres
                "Martínez", // Apellidos
                new DateTime(1981, 9, 11), // FechaNacimiento
                "Cra 23 #45-12 Cali", // Direccion
                "338033519", // Telefono
                "laura.martínez27@correo.com", // Email
                4190629M, // Salario (nota la 'M' para decimal)
                new DateTime(2014, 9, 1), // FechaIngreso
                1, // DepartmentId
                1, // JobTitleId
                1, // EducationLevelId
                "Enfoque analítico y habilidades de comunicación." // Estado
            );

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Empleado insertado manualmente",
                id = employee.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en InsertManual");
            return StatusCode(500, new
            {
                error = ex.Message,
                inner = ex.InnerException?.Message,
                stack = ex.StackTrace
            });
        }
    }

    private decimal TestParseDecimal(string value)
    {
        // Eliminar puntos de miles
        var cleanValue = value?.Replace(".", "").Replace(",", ".");
        
        if (decimal.TryParse(cleanValue, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        
        return 0;
    }
}