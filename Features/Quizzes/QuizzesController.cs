using Examination_System.Common.Exceptions;
using Examination_System.Common.Models;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Attempts.Commands;
using Examination_System.Features.Quizzes.Commands;
using Examination_System.Features.Quizzes.DTOs;
using Examination_System.Features.Quizzes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            string? diplomaId,
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

        [HttpPost]
        // [Authorize(Roles = "Admin")] // Temporarily disabled for testing
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequst request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var mockUser = await userManager.Users.FirstOrDefaultAsync();

                if (mockUser == null)
                {
                    throw new AppException("Invalid or missing user identity in token.", 401);
                }
            }

            var command = new CreateQuizCommand(request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return result.ErrorCode switch
                {
                    ErrorCode.DiplomaNotFound => NotFound(result),
                    ErrorCode.QuizTitleExists => Conflict(result),
                    ErrorCode.Forbidden => StatusCode(403, result),
                    _ => BadRequest(result)
                };
            }
            return Ok(result);
        }

        [HttpPut]
        // [Authorize(Roles = "Admin")] // Temporarily disabled for testing
        public async Task<IActionResult> UpdateQuiz([FromBody] UpdateQuizRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var mockUser = await userManager.Users.FirstOrDefaultAsync();

                if (mockUser == null)
                {
                    throw new AppException("Invalid or missing user identity in token.", 401);
                }
            }
            
            var command = new UpdateQuizCommand(request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return result.ErrorCode switch
                {
                    ErrorCode.QuizNotFound => NotFound(result),
                    ErrorCode.QuizTitleExists => Conflict(result),
                    ErrorCode.InvalidQuizData => BadRequest(result),
                    ErrorCode.Forbidden => StatusCode(403, result),
                    _ => BadRequest(result)
                };
            }

            return Ok(result);
        }

        [HttpPost("{id}/start")]
        // [Authorize] // Temporarily bypassed since JWT Scheme isn't active yet for testing
        public async Task<IActionResult> StartQuiz(Guid id)
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
                    throw new AppException("Invalid or missing user identity in token.", 401);
                }
            }

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
        
        [HttpGet("Total-Quizzes")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalQuizzes()
        {
            var result = await _mediator.Send(new GetTotalQuizzesQuery());

            return Ok(result);
        }
    }
}
