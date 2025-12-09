using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByEmailAsync(string email);
    Task<Employee?> GetByDocumentAsync(string document);
    Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
}