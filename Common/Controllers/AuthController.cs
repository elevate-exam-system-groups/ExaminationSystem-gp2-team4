using Examination_System.Features.Auth.Commands.Registeration;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Common.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand request)
        {
            var result = await _mediator.Send(request);

            if (result == null)
                return BadRequest();

            return Ok(result);
        }
    }
}