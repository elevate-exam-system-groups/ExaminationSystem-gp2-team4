namespace Examination_System.Features.Questions.DTOs
{
    public class AddOptionDTO
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int OrderIndex { get; set; }
    }
}
