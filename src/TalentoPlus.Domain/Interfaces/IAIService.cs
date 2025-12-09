namespace TalentoPlus.Domain.Interfaces;

public interface IAIService
{
    public Task<string> AskQuestionAsync(string question);
}