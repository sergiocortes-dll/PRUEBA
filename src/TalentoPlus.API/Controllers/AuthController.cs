using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly NotificationService  _notificationService;
    private readonly IEmployeeRepository _employeeRepository;
    // private readonly IEmailService _emailService;

    public AuthController(
        IConfiguration configuration,
        IEmployeeRepository employeeRepository,
        NotificationService  notificationService
        )
    {
        _configuration = configuration;
        _employeeRepository = employeeRepository;
        _notificationService = notificationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var existing = await _employeeRepository.GetByDocumentAsync(request.Documento);
            if (existing != null)
                return BadRequest(new { message = "Ya existe un empleado con ese documento" });

            existing = await _employeeRepository.GetByEmailAsync(request.Email);
            if (existing != null)
                return BadRequest(new { message = "Ya existe un empleado con ese email" });

            var employee = new Employee(
                request.Documento,
                request.Nombres,
                request.Apellidos,
                request.FechaNacimiento,
                request.Direccion,
                request.Telefono,
                request.Email
            );

            employee.UpdateJobInfo(
                request.Salario,
                request.DepartamentoId,
                request.CargoId,
                request.NivelEducativoId,
                request.PerfilProfesional
            );
            
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync(); 
            
            await _notificationService.NotifyUserRegistrationAsync(
                employee.Email,
                employee.Name);
            
            return Ok(new
            {
                message = "Registro exitoso.",
                employeeId = employee.Id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error en el registro",
                error = ex.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Buscar empleado por documento o email
            var employee = await _employeeRepository.GetByDocumentAsync(request.Documento);
            if (employee == null)
            {
                employee = await _employeeRepository.GetByEmailAsync(request.Documento);
            }

            if (employee == null || employee.Email != request.Email)
                return Unauthorized(new { message = "Credenciales inválidas" });

            // Generar token JWT
            var token = GenerateJwtToken(employee);

            return Ok(new
            {
                token,
                employee = new
                {
                    employee.Id,
                    employee.Document,
                    employee.Name,
                    employee.Surname,
                    employee.Email
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Error en el login", 
                error = ex.Message 
            });
        }
    }

    private string GenerateJwtToken(Employee employee)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, employee.Document),
            new Claim(JwtRegisteredClaimNames.Email, employee.Email),
            new Claim("employeeId", employee.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "TalentoPlus",
            audience: "TalentoPlusClients",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class RegisterRequest
{
    public string Documento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Salario { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string Estado { get; set; } = "Activo";
    public int DepartamentoId { get; set; } = 1;
    public int CargoId { get; set; } = 1;
    public int NivelEducativoId { get; set; } = 1;
    public string PerfilProfesional { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Documento { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}