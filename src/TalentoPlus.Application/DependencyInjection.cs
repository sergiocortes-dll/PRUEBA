using Microsoft.Extensions.DependencyInjection;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Application.Services;
using TalentoPlus.Application.UseCases;

namespace TalentoPlus.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar servicios de aplicación
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IExcelImportService, ExcelImportService>();
        
        services.AddScoped<SendEmailUseCase>();
        
        return services;
    }
}