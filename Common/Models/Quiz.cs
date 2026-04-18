using ExaminationSystem.API.Common.Models;

namespace Examination_System.Common.Models
{
    public class Quiz : BaseEntity
    {
        public Guid DiplomaId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int PassScore { get; set; }
        public string Status { get; set; } = string.Empty;
        public int QuestionsCount { get; set; }
        public int MaxAttempts { get; set; }
        public string? Instructions { get; set; }
        public Diploma Diploma { get; set; }

        public ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
