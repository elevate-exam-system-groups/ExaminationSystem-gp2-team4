namespace Examination_System.Features.Answers.DTOs
{
    public class GetAttemptAnswersDTO
    {
        public Guid QuestionId { get; init; }
        public Guid OptionId { get; init; } 
    }
}
