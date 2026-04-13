using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Examination_System.Features.Attempts.Commands;
using Examination_System.Common.Exceptions;

namespace Examination_System.Features.Quizzes
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizzesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuizzesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{id}/start")]
        [Authorize]
        public async Task<IActionResult> StartQuiz(Guid id)
        {
            // Extract the user identity securely directly from the JWT claims per your rule
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new AppException("Invalid or missing user identity in token.", 401);
            }

            // Dispatch command via MediatR
            var command = new StartAttemptCommand(id, userId);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
