using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByEmailAsync(string email)
        => await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);

    public async Task<Employee?> GetByDocumentAsync(string document)
        => await _context.Employees.FirstOrDefaultAsync(e => e.Document == document);

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId)
    {
        return await _context.Employees
            .Where(e => e.DepartmentId == departmentId)
            .ToListAsync();
    }
}