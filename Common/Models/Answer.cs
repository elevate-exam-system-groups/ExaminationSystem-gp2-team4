using System;
using Examination_System.Common.Models;

namespace Examination_System.Common.Models
{
    public class Answer : BaseEntity
    {
        public Guid AttemptId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid OptionId { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime? AnsweredAt { get; set; }
<<<<<<< HEAD
        
        public virtual Attempt Attempt { get; set; }
        public virtual Question Question { get; set; }
        public virtual Option Option { get; set; }
=======
        public Attempt Attempt { get; set; }
        public Question Question { get; set; }
        public Option Option { get; set; }
>>>>>>> Create-Manage-Quizzes
    }
}
