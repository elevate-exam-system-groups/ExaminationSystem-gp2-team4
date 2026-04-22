using Examination_System.Common.Wrappers;
using MediatR;

namespace Examination_System.Features.Auth.Queries;

public class GetUsersCountQuery : IRequest<ApiResponse<int>>;