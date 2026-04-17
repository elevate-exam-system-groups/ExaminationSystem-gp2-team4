using System;
using Examination_System.Common.Models;

namespace Examination_System.Common.Models
{
    public class Option : BaseEntity
    {
        public Guid QuestionId { get; set; }
        public string Body { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int OrderIndex { get; set; }
        public Question Question { get; set; }
    }
}
