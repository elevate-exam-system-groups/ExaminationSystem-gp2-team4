namespace Examination_System.Features.Quizzes.DTOs;


public class CreateQuizResult
{
    public Guid Id { get; set; }
    public Guid DiplomaId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public int TotalQuestions { get; set; }
    public int PassScore { get; set; }
}
