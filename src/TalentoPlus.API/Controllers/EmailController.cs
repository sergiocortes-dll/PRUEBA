using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.DTOs.Employee;
using TalentoPlus.Application.UseCases;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly SendEmailUseCase _sendEmailUseCase;

    public EmailController(SendEmailUseCase sendEmailUseCase)
    {
        _sendEmailUseCase = sendEmailUseCase;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
    {
        var response = await _sendEmailUseCase.ExecuteAsync(request);
        
        if (response.Status == "Error")
            return BadRequest(response);

        return Ok(response);
    }
    
    [HttpPost("welcome")]
    public async Task<IActionResult> SendWelcomeEmail([FromBody] WelcomeEmailRequest request)
    {
        var success = await _sendEmailUseCase.SendWelcomeEmailAsync(
            request.Email,
            request.FullName,
            request.HireDate
        );

        return success 
            ? Ok(new { message = "Email enviado" })
            : StatusCode(500, new { message = "Error al enviar" });
    }
}