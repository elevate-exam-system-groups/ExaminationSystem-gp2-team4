using Examination_System.Common.Wrappers;
using MediatR;

namespace Examination_System.Features.Admin.Queries.GetActiveUsersToday
{
    public record GetActiveUsersTodayQuery
        : IRequest<ApiResponse<int>>;
}
