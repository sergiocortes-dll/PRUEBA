using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class AIService : IAIService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AIService> _logger;

    public AIService(IConfiguration configuration, ILogger<AIService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> AskQuestionAsync(string question)
    {
        // Por ahora, respuestas simuladas
        // En producción, integrar con Gemini/OpenAI
        
        await Task.Delay(500); // Simular procesamiento
        
        var lowerQuestion = question.ToLower();
        
        if (lowerQuestion.Contains("cuántos") && lowerQuestion.Contains("auxiliares"))
            return "Hay 15 auxiliares en la plataforma.";
        
        if (lowerQuestion.Contains("departamento") && lowerQuestion.Contains("tecnología"))
            return "Hay 28 empleados en el departamento de Tecnología.";
        
        if (lowerQuestion.Contains("inactivo"))
            return "Hay 42 empleados en estado Inactivo.";
        
        return $"Respuesta simulada para: '{question}'. En producción, esto se conectaría a un servicio de IA real.";
    }
}