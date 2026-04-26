namespace Examination_System.Features.StudentDashboard.DTOs
{
    public class StudentDashboardResponse
    {
        public List<EnrolledDiplomaDto> EnrolledDiplomas { get; set; } = new();
        public List<RecentQuizAttemptDto> RecentQuizAttempts { get; set; } = new();
        public OverallStatsDto OverallStats { get; set; } = new();
    }

    public class EnrolledDiplomaDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public int QuizCount { get; set; }
        public int CompletedQuizCount { get; set; }
    }

    public class RecentQuizAttemptDto
    {
        public Guid AttemptId { get; set; }
        public Guid QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public string DiplomaTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPassed { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }

    public class OverallStatsDto
    {
        public int TotalQuizzesTaken { get; set; }
        public double AvgScore { get; set; }
        public double PassRate { get; set; }
    }
}