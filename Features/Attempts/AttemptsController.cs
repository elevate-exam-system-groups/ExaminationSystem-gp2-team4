using Examination_System.Common.Exceptions.Errors;
using Examination_System.Common.Models;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Attempts.Commands;
using Examination_System.Features.Attempts.Queries;
using ExaminationSystem.API.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Examination_System.Features.Attempts
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttemptsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AttemptsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{id}/submit")]
        public async Task<IActionResult> SubmitAttempt(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid userId;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out userId))
            {
                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var mockUser = await userManager.Users.FirstOrDefaultAsync();

                if (mockUser != null)
                {
                    userId = mockUser.Id;
                }
                else
                {
                    throw new UnAuthorizedException("Unauthorized request.");
                }
            }

            var command = new SubmitAttemptCommand(id, userId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return result.ErrorCode switch
                {
                    ErrorCode.AttemptNotFound => NotFound(result),
                    ErrorCode.Forbidden => StatusCode(403, result),
                    ErrorCode.AttemptClosed => Conflict(result),
                    ErrorCode.AttemptExpired => StatusCode(410, result),
                    _ => BadRequest(result)
                };
            }

            return Ok(result.Data);
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartAttempt([FromBody] StartAttemptCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return result.ErrorCode switch
                {
                    ErrorCode.QuizNotFound => NotFound(result),
                    ErrorCode.AttemptInProgress => Conflict(result),
                    ErrorCode.AttemptLimitReached => StatusCode(403, result),
                    _ => BadRequest(result)
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("timer")]
        public async Task<IActionResult> GetTimer([FromQuery] Guid attemptId)
        {
            var result = await _mediator.Send(new GetAttemptTimerQuery(attemptId));

            if (!result.IsSuccess)
            {
                return result.ErrorCode switch
                {
                    ErrorCode.AttemptNotFound => NotFound(result),
                    ErrorCode.Forbidden => StatusCode(403, result),
                    ErrorCode.AttemptClosed => Conflict(result),
                    ErrorCode.AttemptExpired => StatusCode(410, result),
                    _ => BadRequest(result)
                };
            }

            return Ok(result.Data);
        }

        [HttpPost("answer")]
        public async Task<IActionResult> SaveAnswer([FromQuery] Guid attemptId, [FromQuery] Guid questionId,
            [FromQuery] Guid selectedOptionId)
        {
            var result = await _mediator.Send(new SaveAnswerCommand(attemptId, questionId, selectedOptionId));

            if (!result.IsSuccess)
            {
                return result.ErrorCode switch
                {
                    ErrorCode.AttemptNotFound => NotFound(result),
                    ErrorCode.Forbidden => StatusCode(403, result),
                    ErrorCode.AttemptClosed => Conflict(result),
                    ErrorCode.AttemptExpired => StatusCode(410, result),
                    ErrorCode.InvalidQuestion => UnprocessableEntity(result),
                    ErrorCode.InvalidOption => UnprocessableEntity(result),
                    _ => BadRequest(result)
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("avg-pass-rate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<double>>> GetAvgPassRate(
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAvgPassRateQuery(), cancellationToken);
            return Ok(result);
        }
        
         [HttpGet("Total-Attempts")]
         [Authorize(Roles = "Admin")]
            public async Task<ActionResult<ApiResponse<int>>> GetTotalAttempts()
            {
                var result = await _mediator.Send(new GetTotalAttemptsQuery());
                return Ok(result);
            }
    }
}
