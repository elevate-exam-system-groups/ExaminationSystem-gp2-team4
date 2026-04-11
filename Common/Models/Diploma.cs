namespace Examination_System.Common.Models
{
    public class Diploma : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string ImageUrl { get; set; }
    }
}
