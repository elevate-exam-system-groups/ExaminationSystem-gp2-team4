namespace Examination_System.Features.Admin.DTOs
{
    public class GetAllAttemptsDTO
    {
        public List<AttemptDTO> Attempts { get; set; }
        public int PageNum { get; set; }   
        public int ItemsPerPage { get; set; }
        public int TotalCount { get; set; } 

    }
}
