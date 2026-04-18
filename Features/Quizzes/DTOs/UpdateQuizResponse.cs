namespace Examination_System.Features.Quizzes.DTOs;

public class UpdateQuizResponse
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Instructions { get; set; }
    public int? PassScore { get; set; }
    public int? MaxAttempts { get; set; }
    public int? DurationMinutes { get; set; }
    public int? QuestionsCount { get; set; }
    
}