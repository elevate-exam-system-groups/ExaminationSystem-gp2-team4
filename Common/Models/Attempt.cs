using ExaminationSystem.API.Common.Models;

namespace Examination_System.Common.Models
{
    public class Attempt : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid QuizId { get; set; }
        public string Status { get; set; } =string.Empty; 
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedAt { get; set; }
        public int TotalQuestions { get; set; }
        public int Score { get; set; }
        public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public virtual User User { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}
