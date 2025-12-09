using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobTitlesController : ControllerBase
{
    private readonly IRepository<JobTitle> _jobTitleRepository;
    private readonly ApplicationDbContext _context;

    public JobTitlesController(
        IRepository<JobTitle> jobTitleRepository,
        ApplicationDbContext context)
    {
        _jobTitleRepository = jobTitleRepository;
        _context = context;
    }

    // GET: api/jobtitles (PÚBLICO)
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobTitle>>> GetJobTitles()
    {
        var jobTitles = await _context.JobTitles.ToListAsync();
        return Ok(jobTitles);
    }

    // POST: api/jobtitles (PROTEGIDO)
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<JobTitle>> CreateJobTitle([FromBody] CreateJobTitleRequest request)
    {
        var jobTitle = new JobTitle(request.Name);
        await _jobTitleRepository.AddAsync(jobTitle);
        
        return CreatedAtAction(nameof(GetJobTitles), new { id = jobTitle.Id }, jobTitle);
    }
}

public class CreateJobTitleRequest
{
    public string Name { get; set; } = string.Empty;
}