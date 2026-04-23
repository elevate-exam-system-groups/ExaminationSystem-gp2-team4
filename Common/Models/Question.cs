using System;
using Examination_System.Common.Models;

namespace Examination_System.Common.Models
{
    public class Question : BaseEntity
    {
        public Guid QuizId { get; set; }
        public string Body { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public Quiz? Quiz { get; set; }
        public string? Explanation { get; set; } // Explain Result of Question

        public ICollection<Option> Options { get; set; } = new List<Option>();
    }
}
