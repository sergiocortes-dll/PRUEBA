using TalentoPlus.Domain.Models;

namespace TalentoPlus.Domain.Interfaces;

public interface IExcelFileReader
{
    Task<List<EmployeeExcelDto>> ReadEmployeesAsync(Stream fileStream);
    bool ValidateExcelStructure(Stream fileStream);
}