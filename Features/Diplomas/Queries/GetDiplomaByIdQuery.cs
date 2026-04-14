using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Diplomas.DTOs;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Examination_System.Features.Diplomas.Queries
{
    public record GetDiplomaByIdQuery(string DiplomaId) : IRequest<ApiResponse<DiplomaResponse>>;
    public class GetDiplomaByIdQueryHandler : IRequestHandler<GetDiplomaByIdQuery, ApiResponse<DiplomaResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        public GetDiplomaByIdQueryHandler(IUnitOfWork unitOfWork, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
        }
        public async Task<ApiResponse<DiplomaResponse>> Handle(GetDiplomaByIdQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.DiplomaId))
                return ApiResponse<DiplomaResponse>.Failure(ErrorCode.NoParamSent);

            Guid DiplomaId;
            if (!Guid.TryParse(request.DiplomaId, out DiplomaId))
                return ApiResponse<DiplomaResponse>.Failure(ErrorCode.InvaildDiplomaId);

            var cacheKey = $"GetDiplomaById_{request.DiplomaId}";
            if (_memoryCache.TryGetValue(cacheKey, out DiplomaResponse cachedResponse))
            {
                return ApiResponse<DiplomaResponse>.Success(cachedResponse);
            }

            var DiplomaRepository = _unitOfWork.Repository<Diploma>();
            var diploma = await DiplomaRepository.GetByIdAsync(DiplomaId);
            if (diploma is null)
                return ApiResponse<DiplomaResponse>.Failure(ErrorCode.DiplomaNotFound);

            if (!diploma.IsActive)
                return ApiResponse<DiplomaResponse>.Failure(ErrorCode.DiplomaIsNotActive);

            var diplomaResponse = new DiplomaResponse
            {
                Id = diploma.Id,
                Title = diploma.Title,
                Description = diploma.Description,
                QuizCount = diploma.Quizzes.Count(),
                StudentProgress = diploma.Quizzes.Any() ? ((decimal)diploma.Quizzes.Sum(q => q.PassScore) / (diploma.Quizzes.Count() * 100)) * 100 : 0
            };

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
            _memoryCache.Set(cacheKey, diplomaResponse, cacheEntryOptions);

            return ApiResponse<DiplomaResponse>.Success(diplomaResponse);
        }
    }
}
