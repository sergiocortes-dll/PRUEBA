using TalentoPlus.Application.DTOs.Employee;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Application.Services;

public interface IEmployeeService
{
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<IEnumerable<EmployeeDto>> GetByDepartmentAsync(int departmentId);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
    Task<bool> UpdateAsync(int id, CreateEmployeeDto dto);
    Task<bool> DeleteAsync(int id);
}

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repo;

    public EmployeeService(IEmployeeRepository repo)
    {
        _repo = repo;
    }
    
    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var employee = await _repo.GetByIdAsync(id);
        return employee == null ? null : MapToDto(employee);
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
        var employees = await _repo.GetAllAsync();
        return employees.Select(MapToDto);
    }

    public async Task<IEnumerable<EmployeeDto>> GetByDepartmentAsync(int departmentId)
    {
        var employees = await _repo.GetByDepartmentAsync(departmentId);

        return employees.Select(MapToDto);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        var exists = await _repo.ExistsAsync(x => x.Document == dto.Document);
        if (exists)
            throw new InvalidOperationException("Ya existe un empleado con ese documento.");

        var employee = new Employee(
            dto.Document,
            dto.Name,
            dto.Surname,
            dto.BirthDate,
            dto.Address,
            dto.Phone,
            dto.Email,
            dto.Salary,
            dto.HireDate,
            dto.DepartmentId,
            dto.JobTitleId,
            dto.EducationLevelId,
            dto.ProfessionalProfile
        );

        await _repo.AddAsync(employee);
        await _repo.SaveChangesAsync();

        return MapToDto(employee);
    }

    public async Task<bool> UpdateAsync(int id, CreateEmployeeDto dto)
    {
        var employee = await _repo.GetByIdAsync(id);

        if (employee == null) return false;

        if (dto.Document != employee.Document)
        {
            employee.ChangeDocument(dto.Document);
        }
        
        employee.UpdatePersonalInfo(
            dto.Name,
            dto.Surname,
            dto.BirthDate,
            dto.Address,
            dto.Phone,
            dto.Email
        );
        
        employee.UpdateJobInfo(
            dto.Salary,
            dto.DepartmentId,
            dto.JobTitleId,
            dto.EducationLevelId,
            dto.ProfessionalProfile
        );
        
        _repo.Update(employee);
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _repo.GetByIdAsync(id);
        
        if (employee == null) return false;
        
        _repo.Delete(employee);
        await _repo.SaveChangesAsync();
        return true;
    }

    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            Document = employee.Document,
            Name = employee.Name,
            Surname = employee.Surname,
            BirthDate = employee.BirthDate,
            Address = employee.Address,
            Phone = employee.Phone,
            Email = employee.Email,
            CreatedAt = employee.CreatedAt,
            ModifiedAt = employee.ModifiedAt
        };
    }
}