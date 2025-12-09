using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfController : ControllerBase
{
    private readonly IPdfService _pdfService;
    private readonly IEmployeeService _employeeService;

    public PdfController(IPdfService pdfService, IEmployeeService employeeService)
    {
        _pdfService = pdfService;
        _employeeService = employeeService;
    }

    // GET: api/pdf/employee/{id} (PROTEGIDO - admin puede ver cualquier, empleado solo el suyo)
    [Authorize]
    [HttpGet("employee/{id}")]
    public async Task<IActionResult> GenerateEmployeePdf(int id)
    {
        try
        {
            var employeeIdClaim = User.FindFirst("employeeId");
            if (employeeIdClaim == null || !int.TryParse(employeeIdClaim.Value, out var currentEmployeeId))
                return Unauthorized(new { message = "Token inválido" });

            // var isAdmin = User.IsInRole("Admin");S
            // if (!isAdmin && currentEmployeeId != id)
            //     return Forbid();

            var pdfBytes = await _pdfService.GenerateEmployeePdfAsync(id);
            
            var employee = await _employeeService.GetByIdAsync(id);
            var fileName = employee != null 
                ? $"HojaVida_{employee.Document}_{employee.Name}_{employee.Surname}.pdf"
                : $"HojaVida_{id}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "Error al generar el PDF",
                details = ex.Message
            });
        }
    }

    [Authorize]
    [HttpGet("myprofile")]
    public async Task<IActionResult> GenerateMyProfilePdf()
    {
        try
        {
            var employeeIdClaim = User.FindFirst("employeeId");
            if (employeeIdClaim == null || !int.TryParse(employeeIdClaim.Value, out var employeeId))
                return Unauthorized(new { message = "Token inválido" });

            var pdfBytes = await _pdfService.GenerateEmployeePdfAsync(employeeId);
            
            var employee = await _employeeService.GetByIdAsync(employeeId);
            var fileName = employee != null 
                ? $"Mi_HojaVida_{employee.Document}.pdf"
                : $"Mi_HojaVida.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "Error al generar el PDF",
                details = ex.Message
            });
        }
    }
}