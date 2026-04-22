using Examination_System.Common.Models;
using Examination_System.Common.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Examination_System.Features.Auth.Queries;

public class GetUsersCountQueryHandler 
    : IRequestHandler<GetUsersCountQuery, ApiResponse<int>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMemoryCache _cache;

    private const string CacheKey = "users_count";

    public GetUsersCountQueryHandler(
        UserManager<ApplicationUser> userManager,
        IMemoryCache cache)
    {
        _userManager = userManager;
        _cache = cache;
    }

    public async Task<ApiResponse<int>> Handle(
        GetUsersCountQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Check cache
        if (_cache.TryGetValue(CacheKey, out ApiResponse<int> cachedCount))
        {
            return cachedCount;
        }

        // 2. Query DB
        var count = await _userManager.Users
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var response = ApiResponse<int>.Success(count);

        // 3. Save to cache
        _cache.Set(CacheKey, response, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        });

        return response;
    }
}