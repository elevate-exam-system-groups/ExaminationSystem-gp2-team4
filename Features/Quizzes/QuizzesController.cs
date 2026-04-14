using Examination_System.Features.Quizzes.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Features.Quizzes
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public QuizzesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizzesByDiplomaId(string? DiplomaId, int PageNum=1, int ItemPerPage=5, string? SearchValue=null)
        {   
            var result = await _mediator.Send(new GetQuizzesByDiplomaIdQuery(DiplomaId, PageNum, ItemPerPage, SearchValue));
            return Ok(result);
        }
    }
}
