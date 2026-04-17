namespace Examination_System.Common.Models
{
    public class Attempt : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public Guid QuizId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedAt { get; set; }
        public int TotalQuestions { get; set; }
        public int Score { get; set; }
        public ApplicationUser User { get; set; }
        public Quiz Quiz { get; set; }
    }
}
