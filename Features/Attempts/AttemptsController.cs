using Examination_System.Common.Exceptions;
using Examination_System.Common.Exceptions.Errors;
using Examination_System.Common.Models;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Attempts.Commands;
using Examination_System.Features.Attempts.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Examination_System.Features.Attempts
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttemptsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttemptsController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpPost("{id}/submit")]
        public async Task<IActionResult> SubmitAttempt(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                throw new UnAuthorizedException("Unauthorized request.");

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
        public async Task<IActionResult> SaveAnswer(
            [FromQuery] Guid attemptId,
            [FromQuery] Guid questionId,
            [FromQuery] Guid selectedOptionId)
        {
            var result = await _mediator.Send(
                new SaveAnswerCommand(attemptId, questionId, selectedOptionId));

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
    }
}