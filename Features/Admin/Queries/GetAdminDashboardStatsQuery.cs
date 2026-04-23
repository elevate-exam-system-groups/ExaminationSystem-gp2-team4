using Examination_System.Common.Wrappers;
using Examination_System.Features.Admin.AdminResponse;
using Examination_System.Features.Admin.Queries.GetActiveUsersToday;
using Examination_System.Features.Attempts.Queries;
using Examination_System.Features.Auth.Queries;
using Examination_System.Features.Quizzes.Queries;
using MediatR;

namespace Examination_System.Features.Admin.Queries;

public record GetAdminDashboardStatsQuery()
    : IRequest<ApiResponse<AdminDashboardStatsDto>>;

public class GetAdminDashboardStatsQueryHandler
    : IRequestHandler<GetAdminDashboardStatsQuery, ApiResponse<AdminDashboardStatsDto>>
{
    private readonly IMediator _mediator;

    public GetAdminDashboardStatsQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ApiResponse<AdminDashboardStatsDto>> Handle(
        GetAdminDashboardStatsQuery request,
        CancellationToken cancellationToken)
    {
        var totalUsersTask = _mediator.Send(new GetUsersCountQuery(), cancellationToken);
        var activeUsersTask = _mediator.Send(new GetActiveUsersTodayQuery(), cancellationToken);
        var totalQuizzesTask = _mediator.Send(new GetTotalQuizzesQuery(), cancellationToken);
        var totalAttemptsTask = _mediator.Send(new GetTotalAttemptsQuery(), cancellationToken);
        var avgPassRateTask = _mediator.Send(new GetAvgPassRateQuery(), cancellationToken);

      
        var result = new AdminDashboardStatsDto
        {
            TotalUsers = (await totalUsersTask).Data,
            ActiveUsersToday = (await activeUsersTask).Data,
            TotalQuizzes = (await totalQuizzesTask).Data,
            TotalAttempts = (await totalAttemptsTask).Data,
            AvgPassRate = (await avgPassRateTask).Data
        };

        return ApiResponse<AdminDashboardStatsDto>.Success(result);
    }
}