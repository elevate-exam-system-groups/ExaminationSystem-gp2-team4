using System;

namespace Examination_System.Features.Diplomas.DTOs
{
    public class CreateDiplomaRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
