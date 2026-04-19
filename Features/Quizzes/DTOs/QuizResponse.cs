namespace Examination_System.Features.Quizzes.DTOs
{
    public class QuizResponse
    {
        public Guid Id { get; set; }
        
        public Guid DiplomaId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }

        public int QuestionsCount { get; set; }
        
        public string? Instructions { get; set; }
        public int MaxAttempts { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int PassScore { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
