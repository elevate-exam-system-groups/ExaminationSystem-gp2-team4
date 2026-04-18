using ExaminationSystem.API.Common.Models;

namespace Examination_System.Common.Models
{
    public class Attempt : BaseEntity
    {
<<<<<<< HEAD
        public Guid UserId { get; set; }
=======
        public string UserId { get; set; } = string.Empty;
>>>>>>> origin/Test
        public Guid QuizId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedAt { get; set; }
        public int TotalQuestions { get; set; }
        public int Score { get; set; }
<<<<<<< HEAD
        public virtual Quiz? Quiz { get; set; }
        public ApplicationUser? User { get; set; }
=======
        public virtual Quiz Quiz { get; set; }
        public ApplicationUser User { get; set; }
>>>>>>> origin/Test
    }
}
