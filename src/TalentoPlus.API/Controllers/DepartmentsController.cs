using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IRepository<Department> _departmentRepository;
    private readonly ApplicationDbContext _context;

    public DepartmentsController(
        IRepository<Department> departmentRepository,
        ApplicationDbContext context)
    {
        _departmentRepository = departmentRepository;
        _context = context;
    }

    // GET: api/departments (PÚBLICO)
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
    {
        var departments = await _context.Departments.ToListAsync();
        return Ok(departments);
    }

    // GET: api/departments/{id} (PÚBLICO)
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<Department>> GetDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        
        if (department == null)
            return NotFound(new { message = $"Departamento con ID {id} no encontrado" });
        
        return Ok(department);
    }

    // POST: api/departments (PROTEGIDO - solo admin)
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Department>> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        try
        {
            var department = new Department(request.Name);
            await _departmentRepository.AddAsync(department);
            
            return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, department);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Error al crear departamento", 
                error = ex.Message 
            });
        }
    }

    // PUT: api/departments/{id} (PROTEGIDO - solo admin)
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentRequest request)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
            return NotFound(new { message = $"Departamento con ID {id} no encontrado" });

        department.UpdateInfo(request.Name);
        _departmentRepository.Update(department);
        
        return NoContent();
    }

    // DELETE: api/departments/{id} (PROTEGIDO - solo admin)
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
            return NotFound(new { message = $"Departamento con ID {id} no encontrado" });

        _departmentRepository.Delete(department);
        return NoContent();
    }
}

public class CreateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
}