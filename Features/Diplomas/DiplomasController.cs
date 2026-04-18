using Examination_System.Common.Wrappers;
using Examination_System.Features.Diplomas.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Features.Diplomas
{
    //[Authorize(Roles = "Student")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DiplomasController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DiplomasController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDiplomas([FromQuery] int PageNum=1, [FromQuery] int ItemPerPage=5, [FromQuery] string? SearchItem=null)
        {
            var result = await _mediator.Send(new GetAllDiplomasQuery(PageNum, ItemPerPage, SearchItem));
           
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDiplomaById([FromQuery] string? DiplomaId)
        {
            var result = await _mediator.Send(new GetDiplomaByIdQuery(DiplomaId));

            return Ok(result);

        }
    }
}
