using Examination_System.Common.Wrappers;
using Examination_System.Features.StudentDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Examination_System.Features.StudentDashboard
{
    [ApiController]
    [Route("api/student")]
    [Authorize(Policy = "StudentOnly")]
    public class StudentDashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new GetStudentDashboardQuery(userId));

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result.Data);
        }
    }
}