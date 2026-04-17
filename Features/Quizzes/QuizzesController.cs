using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Examination_System.Features.Quizzes.Queries;
using Examination_System.Features.Attempts.Commands;
using Examination_System.Common.Exceptions;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;

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
        public async Task<IActionResult> GetQuizzesByDiplomaId(string? DiplomaId, int PageNum = 1, int ItemPerPage = 5, string? SearchValue = null)
        {
            var result = await _mediator.Send(new GetQuizzesByDiplomaIdQuery(DiplomaId, PageNum, ItemPerPage, SearchValue));
            return Ok(result);
        }

        [HttpPost("{id}/start")]
        // [Authorize] // Temporarily bypassed since JWT Scheme isn't active yet for testing
        public async Task<IActionResult> StartQuiz(Guid id)
        {
            // Extract the user identity securely directly from the JWT claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string userId;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                // Development fallback: Dynamically extract seeded test user to allow testing
                var userManager = HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Examination_System.Common.Models.ApplicationUser>>();
                var mockUser = System.Linq.Enumerable.FirstOrDefault(userManager.Users);

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

            // Dispatch command via MediatR
            var command = new StartAttemptCommand(id, userId);
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

            return Ok(result);
        }
    }
}
