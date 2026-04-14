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
    }
}
