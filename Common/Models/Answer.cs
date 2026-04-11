using System;
using Examination_System.Common.Models;

namespace ExaminationSystem.API.Common.Models
{
    public class Answer : BaseEntity
    {
        public Guid AttemptId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid OptionId { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime? AnsweredAt { get; set; }
    }
}
