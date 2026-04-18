using Examination_System.Common.Exceptions;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Attempts.Commands;
using Examination_System.Features.Quizzes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

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

        [HttpGet]
        public async Task<IActionResult> GetQuizzesByDiplomaId(
            string diplomaId,
            int pageNum = 1,
            int itemPerPage = 5,
            string? searchValue = null)
        {
            if (string.IsNullOrWhiteSpace(diplomaId))
                throw new AppException("DiplomaId is required.", 400);

            var result = await _mediator.Send(
                new GetQuizzesByDiplomaIdQuery(diplomaId, pageNum, itemPerPage, searchValue));

            return Ok(result);
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartQuiz(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdStr, out var userId))
                throw new AppException("Invalid user id.", 401);

            var result = await _mediator.Send(new StartAttemptCommand(id, userId));

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

            return Ok(result);
        }
    }
}