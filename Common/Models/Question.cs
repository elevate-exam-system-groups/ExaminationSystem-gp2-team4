using System;
using Examination_System.Common.Models;

namespace ExaminationSystem.API.Common.Models
{
    public class Question : BaseEntity
    {
        public Guid QuizId { get; set; }
        public string Body { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        
    }
}
