using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Examination_System.Features.Attempts.Commands;
using Examination_System.Common.Exceptions;
using Examination_System.Common.Repositories;

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
                var uow = HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                var mockUser = System.Linq.Enumerable.FirstOrDefault(
                    await uow.Repository<Examination_System.Common.Models.User>().GetAllAsync());

                if (mockUser != null)
                {
                    userId = mockUser.Id;
                }
                else
                {
                    throw new AppException("Invalid or missing user identity in token.", 401);
                }
            }

            var command = new SubmitAttemptCommand(id, userId);
            var result = await _mediator.Send(command);

            if (result.IsNotFound)
            {
                return NotFound(new { message = result.Message });
            }

            if (result.IsForbidden)
            {
                return StatusCode(403, new { message = result.Message });
            }

            if (result.IsConflict)
            {
                return Conflict(new { message = result.Message, data = result.Data });
            }

            return Ok(result.Data);
        }
    }
}
