namespace Examination_System.Features.Quizzes.DTOs;

public class CreateQuizRequst
{
    public string Title { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int PassScore { get; set; }
    public Guid DiplomaId { get; set; }
    public int MaxAttempts { get; set; }
    public string? Instructions { get; set; }

    public int QuestionsCount { get; set; }
}