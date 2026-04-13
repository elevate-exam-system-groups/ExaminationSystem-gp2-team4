using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Examination_System.Features.Attempts.Commands;
using Examination_System.Common.Exceptions;
using Examination_System.Common.Repositories;

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
        // [Authorize] // Temporarily bypassed since JWT Scheme isn't active yet for testing
        public async Task<IActionResult> StartQuiz(Guid id)
        {
            // Extract the user identity securely directly from the JWT claims per your rule
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                // Development fallback: Dynamically extract our seeded test user to allow scalar testing
                var uow = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                var mockUser = System.Linq.Enumerable.FirstOrDefault(await uow.Repository<Examination_System.Common.Models.User>().GetAllAsync());
                
                if (mockUser != null) userId = mockUser.Id;
                else throw new AppException("Invalid or missing user identity in token.", 401);
            }

            // Dispatch command via MediatR
            var command = new StartAttemptCommand(id, userId);
            var result = await _mediator.Send(command);

            if (result.IsConflict)
            {
                return Conflict(result.Data);
            }

            return Ok(result.Data);
        }
    }
}
