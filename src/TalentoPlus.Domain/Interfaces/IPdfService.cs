namespace TalentoPlus.Domain.Interfaces;

public interface IPdfService
{
    public Task<byte[]> GenerateEmployeePdfAsync(int employeeId);
    
}