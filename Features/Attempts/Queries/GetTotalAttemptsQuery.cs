using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Examination_System.Features.Attempts.Queries;

public record GetTotalAttemptsQuery() : IRequest<ApiResponse<int>>;

public class GetTotalAttemptsQueryHandler 
    : IRequestHandler<GetTotalAttemptsQuery, ApiResponse<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;

    private const string CacheKey = "total_attempts";

    public GetTotalAttemptsQueryHandler(
        IUnitOfWork unitOfWork,
        IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<ApiResponse<int>> Handle(
        GetTotalAttemptsQuery request,
        CancellationToken cancellationToken)
    {
        var totalAttempts = await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

            var repo = _unitOfWork.Repository<Attempt>();

            return await repo.CountAsync();
        });

        return ApiResponse<int>.Success(
            totalAttempts
        );
    }
}

