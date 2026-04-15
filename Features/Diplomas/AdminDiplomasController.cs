using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Examination_System.Features.Diplomas.Commands;
using Examination_System.Features.Diplomas.DTOs;

namespace Examination_System.Features.Diplomas
{
    [ApiController]
    [Route("api/admin/diplomas")]
    public class AdminDiplomasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminDiplomasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiploma([FromBody] CreateDiplomaRequest request)
        {
            var result = await _mediator.Send(new CreateDiplomaCommand(request));

            if (!result.IsSuccess)
            {
                if (result.StatusCode == 422)
                {
                    return UnprocessableEntity(result.Errors);
                }
                return StatusCode(result.StatusCode, result.Errors);
            }

            return StatusCode(201, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiploma(Guid id, [FromBody] UpdateDiplomaRequest request)
        {
            var result = await _mediator.Send(new UpdateDiplomaCommand(id, request));

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiploma(Guid id)
        {
            var result = await _mediator.Send(new DeleteDiplomaCommand(id));

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}
