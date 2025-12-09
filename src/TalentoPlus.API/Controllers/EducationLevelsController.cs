using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EducationLevelsController : ControllerBase
{
    private readonly IRepository<EducationLevel> _educationLevelRepository;
    private readonly ApplicationDbContext _context;

    public EducationLevelsController(
        IRepository<EducationLevel> educationLevelRepository,
        ApplicationDbContext context)
    {
        _educationLevelRepository = educationLevelRepository;
        _context = context;
    }

    // GET: api/educationlevels (PÚBLICO)
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EducationLevel>>> GetEducationLevels()
    {
        var educationLevels = await _context.EducationLevels.ToListAsync();
        return Ok(educationLevels);
    }

    // POST: api/educationlevels (PROTEGIDO)
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<EducationLevel>> CreateEducationLevel([FromBody] CreateEducationLevelRequest request)
    {
        var educationLevel = new EducationLevel(request.Name);
        await _educationLevelRepository.AddAsync(educationLevel);
        
        return CreatedAtAction(nameof(GetEducationLevels), new { id = educationLevel.Id }, educationLevel);
    }
}

public class CreateEducationLevelRequest
{
    public string Name { get; set; } = string.Empty;
}