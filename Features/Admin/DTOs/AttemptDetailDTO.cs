namespace Examination_System.Features.Admin.DTOs
{
    public class AttemptDetailDTO
    {
        public Guid UserId { get; set; }
        public Guid QuizId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedAt { get; set; }
        public int TotalQuestions { get; set; }
        public int Score { get; set; }
        public bool IsPassed { get; set; }

        public List<AttemptQuestionDataDTO> QuestionsData { get; set; } = new List<AttemptQuestionDataDTO>();
    }
}
