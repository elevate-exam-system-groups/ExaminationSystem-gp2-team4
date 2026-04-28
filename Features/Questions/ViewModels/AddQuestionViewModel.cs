namespace Examination_System.Features.Questions.ViewModels
{
    public class AddQuestionViewModel
    {
        public Guid QuizId { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<AddOptionViewModel> Options { get; set; } = new List<AddOptionViewModel>();
        public int OrderIndex { get; set; }
        public string? Explanation { get; set; }
    }
}
