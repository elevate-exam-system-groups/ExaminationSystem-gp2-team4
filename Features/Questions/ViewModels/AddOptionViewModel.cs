namespace Examination_System.Features.Questions.ViewModels
{
    public class AddOptionViewModel
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int OrderIndex { get; set; }
    }
}
