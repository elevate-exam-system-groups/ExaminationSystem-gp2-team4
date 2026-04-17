using ExaminationSystem.API.Common.Models;

namespace Examination_System.Common.Models
{
    public class Quiz : BaseEntity
    {
        public Guid DiplomaId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int PassScore { get; set; }
        public string status { get; set; }
        public int QuestionsCount { get; set; }
        public Diploma Diploma { get; set; }
        public virtual ICollection<Attempt> Attempts { get; set; }
        
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
