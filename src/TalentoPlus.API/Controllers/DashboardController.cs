using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IAIService _aiService;

    public DashboardController(IEmployeeService employeeService, IAIService aiService)
    {
        _employeeService = employeeService;
        _aiService = aiService;
    }

    // GET: api/dashboard/summary (PROTEGIDO)
    [Authorize]
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetDashboardSummary()
    {
        try
        {
            var totalEmployees = await _employeeService.GetAllAsync();
            //var employeesOnVacation = await _employeeService.GetEmployeesOnVacationAsync();
            //var activeEmployees = await _employeeService.GetActiveEmployeesAsync();
            //var employeesByDepartment = await _employeeService.GetEmployeesByDepartmentAsync();

            return Ok(new DashboardSummaryDto
            {
                TotalEmployees = totalEmployees.Count(),
                //EmployeesOnVacation = employeesOnVacation,
                //ActiveEmployees = activeEmployees,
                //EmployeesByDepartment = employeesByDepartment,
                LastUpdated = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "Error al obtener resumen del dashboard",
                details = ex.Message
            });
        }
    }

    // POST: api/dashboard/ask (PROTEGIDO - Consulta IA)
    [Authorize]
    [HttpPost("ask")]
    public async Task<IActionResult> AskQuestion([FromBody] QuestionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest(new { message = "La pregunta no puede estar vacía" });

        try
        {
            var answer = await _aiService.AskQuestionAsync(request.Question);
            return Ok(new { 
                question = request.Question, 
                answer,
                timestamp = DateTime.UtcNow 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Error procesando la pregunta", 
                error = ex.Message 
            });
        }
    }
}

public class DashboardSummaryDto
{
    public int TotalEmployees { get; set; }
    public int EmployeesOnVacation { get; set; }
    public int ActiveEmployees { get; set; }
    public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class QuestionRequest
{
    public string Question { get; set; } = string.Empty;
}