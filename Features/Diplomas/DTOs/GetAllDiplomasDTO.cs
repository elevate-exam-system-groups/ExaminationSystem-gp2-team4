namespace Examination_System.Features.Diplomas.DTOs
{
    public class GetAllDiplomasDTO
    {
        public List<DiplomaResponse> Diplomas { get; set; } = [];
        public int PageNum { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalCount { get; set; }

    }
}
