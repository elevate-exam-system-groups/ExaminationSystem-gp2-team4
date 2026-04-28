namespace Examination_System.Features.Questions.ViewModels
{
    public class UpdateQuestionViewModel
    {
        public Guid QuestionId { get; set; }  
        public Guid QuizId { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<UpdateOptionDTO> Options { get; set; } = new List<UpdateOptionDTO>();
        public int OrderIndex { get; set; }
        public string? Explanation { get; set; }
    }
}
