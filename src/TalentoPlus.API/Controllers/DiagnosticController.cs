using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/diagnostic")]
public class DiagnosticController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DiagnosticController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("employees/count")]
    public async Task<IActionResult> GetEmployeeCount()
    {
        try
        {
            var count = await _context.Employees.CountAsync();
            return Ok(new { 
                count, 
                message = $"Total empleados en la base de datos: {count}" 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                error = ex.Message, 
                details = ex.StackTrace 
            });
        }
    }

    [HttpGet("employees/raw")]
    public async Task<IActionResult> GetEmployeesRaw()
    {
        try
        {
            // Obtener datos sin relaciones primero
            var employees = await _context.Employees
                .Select(e => new
                {
                    e.Id,
                    e.Document,
                    e.Name,
                    e.Surname,
                    e.Email
                })
                .ToListAsync();
            
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                error = ex.Message, 
                details = ex.StackTrace 
            });
        }
    }

    [HttpGet("tables")]
    public async Task<IActionResult> GetTableInfo()
    {
        try
        {
            var tables = new
            {
                Employees = await _context.Employees.CountAsync(),
                Departments = await _context.Departments.CountAsync(),
                JobTitles = await _context.JobTitles.CountAsync(),
                EducationLevels = await _context.EducationLevels.CountAsync(),
                ProfessionalProfiles = await _context.ProfessionalProfiles.CountAsync()
            };
            
            return Ok(tables);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                error = ex.Message, 
                details = ex.StackTrace 
            });
        }
    }
}