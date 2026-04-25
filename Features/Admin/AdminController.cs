using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Examination_System.Features.Admin.Queries;
using MediatR;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var result = await _mediator.Send(new GetAdminDashboardStatsQuery());
        return Ok(result);
    }
    [HttpGet("attempts")]
    public async Task<IActionResult> GetAllAttempts([FromQuery] int pageNum = 1, [FromQuery] int itemsPerPage = 20, [FromQuery] Guid? quizId = null, [FromQuery] Guid? studentId = null)
    {
        var result = await _mediator.Send(new GetAllAttemptsQuery(pageNum, itemsPerPage, quizId, studentId));
        return Ok(result);
    }

    [HttpGet("attempt/{attemptId}")]
    public async Task<IActionResult> GetAttemptDetailById([FromRoute]Guid attemptId)
    {
        var result = await _mediator.Send(new GetAttemptDetailById(attemptId));
        return Ok(result);
    }

}