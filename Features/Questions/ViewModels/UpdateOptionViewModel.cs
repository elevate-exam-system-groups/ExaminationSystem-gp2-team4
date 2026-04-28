namespace Examination_System.Features.Questions.ViewModels
{
    public class UpdateOptionViewModel
    {
        public Guid? Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int OrderIndex { get; set; }
    }
}
