namespace Examination_System.Features.Admin.DTOs
{
    public class AttemptDTO
    {
        public Guid AttemptId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public string QuizTitle { get; set; }
        public int Score { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
