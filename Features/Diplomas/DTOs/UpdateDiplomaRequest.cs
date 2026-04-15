using System;

namespace Examination_System.Features.Diplomas.DTOs
{
    public class UpdateDiplomaRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
