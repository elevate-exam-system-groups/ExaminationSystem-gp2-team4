using Examination_System.Common.Models;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Admin.Queries.GetActiveUsersToday;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Examination_System.Features.Auth.Queries;

public class GetActiveUsersTodayQueryHandler
    : IRequestHandler<GetActiveUsersTodayQuery, ApiResponse<int>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMemoryCache _cache;

    private const string CacheKey = "active_users_today";

    public GetActiveUsersTodayQueryHandler(
        UserManager<ApplicationUser> userManager,
        IMemoryCache cache)
    {
        _userManager = userManager;
        _cache = cache;
    }

    public async Task<ApiResponse<int>> Handle(
        GetActiveUsersTodayQuery request,
        CancellationToken ct)
    {
        if (_cache.TryGetValue(CacheKey, out int cachedValue))
        {
            return ApiResponse<int>.Success(cachedValue);
        }

        var startOfDay = DateTime.UtcNow.Date;
        var endOfDay = startOfDay.AddDays(1);

        var activeUsersToday = await _userManager.Users
            .Where(u => u.LastLoginAt >= startOfDay &&
                        u.LastLoginAt < endOfDay)
            .CountAsync(ct);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        _cache.Set(CacheKey, activeUsersToday, cacheOptions);

        return ApiResponse<int>.Success(activeUsersToday);
    }
}