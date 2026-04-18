using Examination_System.Common.Exceptions;
<<<<<<< HEAD
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
=======
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Attempts.Commands;
using Examination_System.Features.Attempts.Queries;
using ExaminationSystem.API.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Examination_System.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
>>>>>>> origin/Test

namespace Examination_System.Features.Attempts
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttemptsController : ControllerBase
    {
        private readonly IMediator _mediator;
<<<<<<< HEAD
        private readonly UserManager<ApplicationUser> _userManager;

        public AttemptsController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
=======

        public AttemptsController(IMediator mediator)
        {
            _mediator = mediator;
>>>>>>> origin/Test
        }

        [HttpPost("{id}/submit")]
        public async Task<IActionResult> SubmitAttempt(Guid id)
        {
<<<<<<< HEAD
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                throw new UnAuthorizedException("Unauthorized request.");
=======
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string userId;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var mockUser = await userManager.Users.FirstOrDefaultAsync();

                if (mockUser != null)
                {
                    userId = mockUser.Id;
                }
                else
                {
                    throw new AppException("Invalid or missing user identity in token.", 401);
                }
            }
            else
            {
                userId = userIdClaim;
            }
>>>>>>> origin/Test

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
<<<<<<< HEAD

            return Ok(result.Data);
=======
return Ok(result.Data);
>>>>>>> origin/Test
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

<<<<<<< HEAD
            return Ok(result.Data);
=======
            return Ok(result);
>>>>>>> origin/Test
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

<<<<<<< HEAD
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
=======
            return Ok(result);
        }

        [HttpPost("answer")]
        public async Task<IActionResult> SaveAnswer([FromQuery] Guid attemptId, [FromQuery] Guid questionId, [FromQuery] Guid selectedOptionId)
        {
    
            var result = await _mediator.Send(new SaveAnswerCommand(attemptId, questionId, selectedOptionId));
>>>>>>> origin/Test

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

<<<<<<< HEAD
            return Ok(result.Data);
        }
    }
}
=======
            return Ok(result);
        }
    }
}
>>>>>>> origin/Test
