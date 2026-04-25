using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Admin.DTOs;
using Examination_System.Features.Diplomas.DTOs;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Examination_System.Features.Admin.Queries
{
    public record GetAllAttemptsQuery(int PageNum, int ItemsPerPage, Guid? QuizId, Guid? StudentId) : IRequest<ApiResponse<GetAllAttemptsDTO>>;

    public class GetAllAttemptsQueryHandler : IRequestHandler<GetAllAttemptsQuery, ApiResponse<GetAllAttemptsDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache; 
        public GetAllAttemptsQueryHandler(IUnitOfWork unitOfWork, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
        }

        public async Task<ApiResponse<GetAllAttemptsDTO>> Handle(GetAllAttemptsQuery request, CancellationToken cancellationToken)
        {

            var cacheKey = $"GetAllAttempts_{request.PageNum}_{request.ItemsPerPage}_{request.QuizId}_{request.StudentId}";
            if (_memoryCache.TryGetValue(cacheKey, out GetAllAttemptsDTO cachedResponse))
            {
                return ApiResponse<GetAllAttemptsDTO>.Success(cachedResponse);
            }

            var AttemptsRepository = _unitOfWork.Repository<Attempt>();
            var AllAttempts =  AttemptsRepository.GetAll();

            var PaginatedResult = PaginationProcess(AllAttempts, request.PageNum, request.ItemsPerPage, request.QuizId, request.StudentId);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            _memoryCache.Set(cacheKey, PaginatedResult, cacheEntryOptions);

            return ApiResponse<GetAllAttemptsDTO>.Success(PaginatedResult);
        }

        private GetAllAttemptsDTO PaginationProcess(IQueryable<Attempt> Attempts, int PageNum, int ItemsPerPage,Guid? QuizId, Guid? StudentId)
        {
            return new GetAllAttemptsDTO()
            {
                Attempts = Attempts.Where(a => (QuizId == null || a.QuizId == QuizId) && (StudentId == null || a.UserId == StudentId))
                               .Skip((PageNum - 1) * ItemsPerPage)
                               .Take(ItemsPerPage)
                               .Select(a => new AttemptDTO
                               {
                                   AttemptId = a.Id,
                                   StudentId = a.UserId,
                                   StudentName = a.User.FullName,
                                   QuizTitle = a.Quiz.Title,
                                   Score = a.Score,
                                   Status = a.Status,
                                   SubmittedAt = a.SubmittedAt ?? DateTime.MinValue,
                               }).ToList(),
                PageNum = PageNum,
                ItemsPerPage = ItemsPerPage,
                TotalCount=Attempts.Count()
            };
        }
    }
}
