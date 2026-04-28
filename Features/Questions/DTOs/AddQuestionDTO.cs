namespace Examination_System.Features.Questions.DTOs
{
    public class AddQuestionDTO
    {
        public Guid QuizId { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<AddOptionDTO> Options { get; set; } = new List<AddOptionDTO>();
        public int OrderIndex { get; set; }
        public string? Explanation { get; set; }
    }
}
