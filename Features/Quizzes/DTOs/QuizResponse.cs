namespace Examination_System.Features.Quizzes.DTOs
{
    public class QuizResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int PassScore { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
