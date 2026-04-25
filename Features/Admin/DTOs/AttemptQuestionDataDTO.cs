namespace Examination_System.Features.Admin.DTOs
{
    public class AttemptQuestionDataDTO
    {
        public string QuestionText { get; set; } = string.Empty;
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set;}
    }
}
