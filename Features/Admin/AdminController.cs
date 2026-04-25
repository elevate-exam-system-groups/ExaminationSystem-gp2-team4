using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Examination_System.Features.Admin.Queries;
using MediatR;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var result = await _mediator.Send(new GetAdminDashboardStatsQuery());
        return Ok(result);
    }
}