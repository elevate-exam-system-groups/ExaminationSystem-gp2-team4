using Examination_System.Common.Models;
using Examination_System.Common.Wrappers;
using ExaminationSystem.API.Common.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Examination_System.Features.Attempts.Queries;

public record GetAvgPassRateQuery() : IRequest<ApiResponse<double>>;

public class GetAvgPassRateQueryHandler
    : IRequestHandler<GetAvgPassRateQuery, ApiResponse<double>>
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    private const string CacheKey = "avg_pass_rate";

    public GetAvgPassRateQueryHandler(
        AppDbContext context,
        IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<ApiResponse<double>> Handle(
        GetAvgPassRateQuery request,
        CancellationToken cancellationToken)
    {
        
        if (_cache.TryGetValue(CacheKey, out double cachedRate))
        {
            return ApiResponse<double>.Success(cachedRate);
        }

        var totalAttempts = await _context.Attempts
            .CountAsync(cancellationToken);

        if (totalAttempts == 0)
        {
            _cache.Set(CacheKey, 0.0, TimeSpan.FromMinutes(5));
            return ApiResponse<double>.Success(0);
        }

        var passedAttempts = await _context.Attempts
            .CountAsync(a => a.IsPassed, cancellationToken);

        var avgPassRate = (double)passedAttempts / totalAttempts * 100;

        _cache.Set(CacheKey, avgPassRate, TimeSpan.FromMinutes(5));

        return ApiResponse<double>.Success(avgPassRate);
    }
}