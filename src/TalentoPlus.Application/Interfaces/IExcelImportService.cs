namespace TalentoPlus.Application.Interfaces;

public interface IExcelImportService
{
    Task<ExcelImportResult> ImportEmployeesAsync(Stream fileStream);
}

public class ExcelImportResult
{
    public bool Success { get; set; }
    public int ImportedCount { get; set; }
    public int UpdatedCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Messages { get; set; } = new();
}