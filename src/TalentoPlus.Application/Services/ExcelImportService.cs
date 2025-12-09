using TalentoPlus.Application.DTOs;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Domain.Models;

namespace TalentoPlus.Application.Services;

public class ExcelImportService : IExcelImportService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IExcelFileReader _excelFileReader;
    private readonly IRepository<Department> _departmentRepository;
    private readonly IRepository<JobTitle> _jobTitleRepository;
    private readonly IRepository<EducationLevel> _educationLevelRepository;

    public ExcelImportService(
        IEmployeeRepository employeeRepository,
        IExcelFileReader excelFileReader,
        IRepository<Department> departmentRepository,
        IRepository<JobTitle> jobTitleRepository,
        IRepository<EducationLevel> educationLevelRepository)
    {
        _employeeRepository = employeeRepository;
        _excelFileReader = excelFileReader;
        _departmentRepository = departmentRepository;
        _jobTitleRepository = jobTitleRepository;
        _educationLevelRepository = educationLevelRepository;
    }

    public async Task<ExcelImportResult> ImportEmployeesAsync(Stream fileStream)
    {
        var result = new ExcelImportResult();
        
        try
        {
            // 1. Leer datos del Excel
            var excelData = await _excelFileReader.ReadEmployeesAsync(fileStream);
            result.Messages.Add($"Se leyeron {excelData.Count} registros del Excel");
            
            // 2. Procesar cada registro
            foreach (var data in excelData)
            {
                try
                {
                    await ProcessEmployeeAsync(data, result);
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.Errors.Add($"Error en registro {data.Documento}: {ex.Message}");
                }
            }

            await _employeeRepository.SaveChangesAsync();
            result.Success = result.ErrorCount == 0;
            result.Messages.Add($"Importación completada: {result.ImportedCount} nuevos, {result.UpdatedCount} actualizados, {result.ErrorCount} errores");
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Error general: {ex.Message}");
        }
        
        return result;
    }

    private async Task ProcessEmployeeAsync(EmployeeExcelDto data, ExcelImportResult result)
    {
        // Buscar o crear departamento
        var department = await FindOrCreateDepartmentAsync(data.Departamento);
        
        // Buscar o crear cargo
        var jobTitle = await FindOrCreateJobTitleAsync(data.Cargo);
        
        // Buscar o crear nivel educativo
        var educationLevel = await FindOrCreateEducationLevelAsync(data.NivelEducativo);
        
        // Mapear estado
        var status = MapStatus(data.Estado);
        
        // Verificar si el empleado ya existe
        var existingEmployee = await _employeeRepository.GetByDocumentAsync(data.Documento);
        
        if (existingEmployee == null)
        {
            // Crear nuevo empleado
            var employee = new Employee(
                data.Documento,
                data.Nombres,
                data.Apellidos,
                data.FechaNacimiento,
                data.Direccion,
                data.Telefono,
                data.Email,
                data.Salario,
                data.FechaIngreso,
                department.Id,
                jobTitle.Id,
                educationLevel.Id,
                data.PerfilProfesional
            );

            employee.ChangeStatus(status);

            await _employeeRepository.AddAsync(employee);
            result.ImportedCount++;
            result.Messages.Add($"Empleado creado: {data.Nombres} {data.Apellidos}");
        }
        else
        {
            // Actualizar empleado existente
            UpdateEmployeeData(existingEmployee, data, department.Id, jobTitle.Id, educationLevel.Id, status);
            _employeeRepository.Update(existingEmployee);
            result.UpdatedCount++;
            result.Messages.Add($"Empleado actualizado: {data.Nombres} {data.Apellidos}");
        }
    }

    private async Task<Department> FindOrCreateDepartmentAsync(string departmentName)
    {
        var departments = await _departmentRepository.GetAllAsync();
        var existing = departments.FirstOrDefault(d => 
            d.Name.Equals(departmentName, StringComparison.OrdinalIgnoreCase));
        
        if (existing != null) return existing;
        
        // Crear nuevo departamento
        var newDepartment = new Department(departmentName);
        await _departmentRepository.AddAsync(newDepartment);
        await _departmentRepository.SaveChangesAsync();
        return newDepartment;
    }

    private async Task<JobTitle> FindOrCreateJobTitleAsync(string jobTitleName)
    {
        var jobTitles = await _jobTitleRepository.GetAllAsync();
        var existing = jobTitles.FirstOrDefault(j => 
            j.Name.Equals(jobTitleName, StringComparison.OrdinalIgnoreCase));
        
        if (existing != null) return existing;
        
        // Crear nuevo cargo
        var newJobTitle = new JobTitle(jobTitleName);
        await _jobTitleRepository.AddAsync(newJobTitle);
        await _jobTitleRepository.SaveChangesAsync();
        return newJobTitle;
    }

    private async Task<EducationLevel> FindOrCreateEducationLevelAsync(string educationLevelName)
    {
        var educationLevels = await _educationLevelRepository.GetAllAsync();
        var existing = educationLevels.FirstOrDefault(e => 
            e.Name.Equals(educationLevelName, StringComparison.OrdinalIgnoreCase));
        
        if (existing != null) return existing;
        
        // Crear nuevo nivel educativo
        var newEducationLevel = new EducationLevel(educationLevelName);
        await _educationLevelRepository.AddAsync(newEducationLevel);
        await _educationLevelRepository.SaveChangesAsync();
        return newEducationLevel;
    }

    private EmployeeStatus MapStatus(string status)
    {
        return status.ToLower() switch
        {
            "activo" => EmployeeStatus.Active,
            "inactivo" => EmployeeStatus.Inactive,
            "vacaciones" => EmployeeStatus.OnVacation,
            _ => EmployeeStatus.Active
        };
    }

    private void UpdateEmployeeData(
        Employee employee,
        EmployeeExcelDto data,
        int departmentId,
        int jobTitleId,
        int educationLevelId,
        EmployeeStatus status)
    {
        // Actualizar datos personales
        employee.UpdatePersonalInfo(
            data.Nombres,
            data.Apellidos,
            data.FechaNacimiento,
            data.Direccion,
            data.Telefono,
            data.Email
        );

        // Actualizar información laboral
        employee.UpdateJobInfo(
            data.Salario,
            departmentId,
            jobTitleId,
            educationLevelId,
            data.PerfilProfesional
        );

        // Actualizar estado
        employee.ChangeStatus(status);
    }
}
