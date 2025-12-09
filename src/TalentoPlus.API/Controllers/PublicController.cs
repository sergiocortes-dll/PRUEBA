using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/public")]
[AllowAnonymous]
public class PublicController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PublicController> _logger;

    public PublicController(
        ApplicationDbContext context,
        ILogger<PublicController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    [HttpGet("departments")]
    public async Task<ActionResult> GetPublicDepartments()
    {
        try
        {
            var departments = await _context.Departments
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                })
                .OrderBy(d => d.Name)
                .ToListAsync();
            
            return Ok(new
            {
                success = true,
                count = departments.Count,
                data = departments
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener departamentos públicos");
            return StatusCode(500, new
            {
                success = false,
                message = "Error interno del servidor"
            });
        }
    }

    // GET: api/public/job-titles
    [HttpGet("job-titles")]
    public async Task<ActionResult> GetPublicJobTitles()
    {
        try
        {
            var jobTitles = await _context.JobTitles
                .Select(j => new
                {
                    j.Id,
                    j.Name
                })
                .OrderBy(j => j.Name)
                .ToListAsync();
            
            return Ok(new
            {
                success = true,
                count = jobTitles.Count,
                data = jobTitles
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cargos públicos");
            return StatusCode(500, new
            {
                success = false,
                message = "Error interno del servidor"
            });
        }
    }

    // GET: api/public/education-levels
    [HttpGet("education-levels")]
    public async Task<ActionResult> GetPublicEducationLevels()
    {
        try
        {
            var educationLevels = await _context.EducationLevels
                .Select(e => new
                {
                    e.Id,
                    e.Name
                })
                .ToListAsync();
            
            return Ok(new
            {
                success = true,
                count = educationLevels.Count,
                data = educationLevels
            });
        }
        catch (Exception ex)
    {
            _logger.LogError(ex, "Error al obtener niveles educativos públicos");
            return StatusCode(500, new
            {
                success = false,
                message = "Error interno del servidor"
            });
        }
    }

    // GET: api/public/company-info
    [HttpGet("company-info")]
    public ActionResult GetCompanyInfo()
    {
        try
        {
            var companyInfo = new
            {
                name = "TalentoPlus S.A.S.",
                description = "Empresa líder en gestión de talento humano",
                contact = new
                {
                    email = "contacto@talentoplus.com",
                    phone = "+57 1 1234567",
                    address = "Calle 123 #45-67, Bogotá, Colombia"
                },
                services = new[]
                {
                    "Gestión de empleados",
                    "Desarrollo organizacional",
                    "Capacitación y entrenamiento",
                    "Consultoría en recursos humanos"
                }
            };
            
            return Ok(new
            {
                success = true,
                data = companyInfo
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener información de la empresa");
            return StatusCode(500, new
            {
                success = false,
                message = "Error interno del servidor"
            });
        }
    }

    // GET: api/public/health
    [HttpGet("health")]
    public async Task<ActionResult> HealthCheck()
    {
        try
        {
            // Verificar conexión a la base de datos
            var canConnect = await _context.Database.CanConnectAsync();
            
            var healthInfo = new
            {
                status = canConnect ? "healthy" : "unhealthy",
                timestamp = DateTime.UtcNow,
                services = new
                {
                    database = canConnect ? "connected" : "disconnected",
                    api = "running"
                },
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
            };
            
            return Ok(healthInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en health check");
            return StatusCode(500, new
            {
                status = "unhealthy",
                error = ex.Message
            });
        }
    }

    // GET: api/public/statistics
    [HttpGet("statistics")]
    public async Task<ActionResult> GetPublicStatistics()
    {
        try
        {
            var statistics = new
            {
                totalEmployees = await _context.Employees.CountAsync(),
                activeEmployees = await _context.Employees.CountAsync(e => e.Status == EmployeeStatus.Active),
                totalDepartments = await _context.Departments.CountAsync(),
                averageTenure = await CalculateAverageTenureAsync()
            };
            
            return Ok(new
            {
                success = true,
                data = statistics
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas públicas");
            return StatusCode(500, new
            {
                success = false,
                message = "Error interno del servidor"
            });
        }
    }

    private async Task<double> CalculateAverageTenureAsync()
    {
        try
        {
            var activeEmployees = await _context.Employees
                .Where(e => e.Status == EmployeeStatus.Active)
                .ToListAsync();
            
            if (!activeEmployees.Any()) return 0;
            
            var totalDays = activeEmployees
                .Sum(e => (DateTime.UtcNow - e.HireDate).TotalDays);
            
            return Math.Round(totalDays / activeEmployees.Count / 365, 2); // En años
        }
        catch
        {
            return 0;
        }
    }
}