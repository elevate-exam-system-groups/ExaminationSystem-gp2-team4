namespace Examination_System.Features.Quizzes.DTOs
{
    public class GetQuizzesResponse
    {
        public List<QuizResponse> Quizzes { get; set; } = new List<QuizResponse>();
        public int PageNum { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalCount { get; set; }
    }
}
