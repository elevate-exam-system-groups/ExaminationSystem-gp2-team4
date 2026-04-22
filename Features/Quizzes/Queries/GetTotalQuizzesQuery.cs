using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Examination_System.Features.Quizzes.Queries;

public record GetTotalQuizzesQuery() : IRequest<ApiResponse<int>>;

public class GetTotalQuizzesQueryHandler
    : IRequestHandler<GetTotalQuizzesQuery, ApiResponse<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;

    private const string CacheKey = "total_quizzes";

    public GetTotalQuizzesQueryHandler(
        IUnitOfWork unitOfWork,
        IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<ApiResponse<int>> Handle(
        GetTotalQuizzesQuery request,
        CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(CacheKey, out int cachedTotal))
        {
            return ApiResponse<int>.Success(cachedTotal);
        }

        var total = _unitOfWork.Repository<Quiz>()
            .GetAll()
            .Count();

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        _cache.Set(CacheKey, total, cacheOptions);

        return ApiResponse<int>.Success(total);
    }
}