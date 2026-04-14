namespace Examination_System.Features.Diplomas.DTOs
{
    public class DiplomaResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int QuizCount { get; set; }
        public decimal StudentProgress { get; set; }
    }
}
