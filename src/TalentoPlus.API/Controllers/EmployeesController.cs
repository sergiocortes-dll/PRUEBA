using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.DTOs.Employee;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IEmployeeService service,  ILogger<EmployeesController> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
    {
        try
        {
            var employees = await _service.GetAllAsync();
            return Ok(new
            {
                success = true,
                count = employees.Count(),
                data = employees
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleados.");

            return StatusCode(500, new
            {
                success = false,
                message = "Error interno del servidor",
                error = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        try
        {
            var employee = await _service.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Empleado con ID {id} no encontrado."
                });
            }

            return Ok(new
            {
                success = true,
                data = employee
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener empleado con ID {id}.");
            return StatusCode(500, new
            {
                success = false,
                message = "Error interno del servidor.",
                error = ex.Message
            });
        }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Datos de entrada inválidos.",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }
            
            var employee = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetEmployee),
                new { id = employee.Id },
                new
                {
                    success = true,
                    message = "Empleado creado exitosamente.",
                    data = employee
                });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear empleado");
            return StatusCode(500, new
            {
                success = false,
                message = "Error interno del servidor",
                error = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateEmployee(int id, CreateEmployeeDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Datos de entrada inválidos"
                });
            }

            var result = await _service.UpdateAsync(id, dto);
            
            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Empleado con ID {id} no encontrado"
                });
            }
            
            return Ok(new
            {
                success = true,
                message = "Empleado actualizado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar empleado con ID {id}");
            return StatusCode(500, new
            {
                success = false,
                message = "Error interno del servidor",
                error = ex.Message
            });
        }
    }
    
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            var result = await _service.DeleteAsync(id);
            
            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Empleado con ID {id} no encontrado"
                });
            }
            
            return Ok(new
            {
                success = true,
                message = "Empleado eliminado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar empleado con ID {id}");
            return StatusCode(500, new
            {
                success = false,
                message = "Error interno del servidor",
                error = ex.Message
            });
        }
    }
    
    [HttpGet("search")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> SearchEmployees(
        [FromQuery] string query,
        [FromQuery] string? department = null,
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            // Implementación básica de búsqueda
            var allEmployees = await _service.GetAllAsync();
            
            var filtered = allEmployees.AsQueryable();
            
            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToLower();
                filtered = filtered.Where(e =>
                    e.Name.ToLower().Contains(query) ||
                    e.Surname.ToLower().Contains(query) ||
                    e.Document.Contains(query) ||
                    e.Email.ToLower().Contains(query));
            }
            
            if (!string.IsNullOrEmpty(department))
            {
                filtered = filtered.Where(e =>
                    e.DepartmentName.ToLower().Contains(department.ToLower()));
            }
            
            if (!string.IsNullOrEmpty(status))
            {
                filtered = filtered.Where(e =>
                    e.Status.ToLower().Contains(status.ToLower()));
            }
            
            // Paginación
            var totalCount = filtered.Count();
            var employees = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            return Ok(new
            {
                success = true,
                total = totalCount,
                page,
                pageSize,
                data = employees
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en búsqueda de empleados");
            return StatusCode(500, new
            {
                success = false,
                message = "Error interno del servidor",
                error = ex.Message
            });
        }
    }
}