namespace Examination_System.Features.Attempts.DTOs;

public class AttemptResultsResponse
{
    public double Score { get; set; }
    public bool Passed { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectCount { get; set; }
    public List<PerQuestionDto> PerQuestion { get; set; } = [];
}

public class PerQuestionDto
{
    public Guid QuestionId { get; set; }
    public string StudentAnswer { get; set; }
    public string CorrectAnswer { get; set; }   // يتبعت بس بعد Submit
    public bool IsCorrect { get; set; }
    public string? Explanation { get; set; }    // لو موجود
}
