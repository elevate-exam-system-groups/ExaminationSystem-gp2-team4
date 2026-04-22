using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Diplomas.DTOs;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection.Metadata.Ecma335;

namespace Examination_System.Features.Diplomas.Queries
{
    public record GetAllDiplomasQuery(int PageNum, int ItemsPerPage, string? SearchValue) : IRequest<ApiResponse<GetAllDiplomasResponse>>;

    public class GetAllDiplomasQueryHandler : IRequestHandler<GetAllDiplomasQuery, ApiResponse<GetAllDiplomasResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        public GetAllDiplomasQueryHandler(IUnitOfWork unitOfWork,IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
        }

        public async Task<ApiResponse<GetAllDiplomasResponse>> Handle(GetAllDiplomasQuery request, CancellationToken cancellationToken)
        {

            var cacheKey = $"GetAllDiplomas_{request.PageNum}_{request.ItemsPerPage}_{request.SearchValue}";
            if (_memoryCache.TryGetValue(cacheKey, out GetAllDiplomasResponse cachedResponse))
            {
                return ApiResponse<GetAllDiplomasResponse>.Success(cachedResponse);
            }

            var DiplomaRepository = _unitOfWork.Repository<Diploma>();
            var diplomas =  DiplomaRepository.GetAll();

            if (diplomas is null || !diplomas.Any())
                return ApiResponse<GetAllDiplomasResponse>.Failure(ErrorCode.EmptyDiplomaArray);

            var ActiveDiplomas = diplomas.Where(d => d.IsActive).ToList();

            if(!ActiveDiplomas.Any())
                return ApiResponse<GetAllDiplomasResponse>.Failure(ErrorCode.EmptyDiplomaArray);

            if (!string.IsNullOrWhiteSpace(request.SearchValue))
            {
                ActiveDiplomas = ActiveDiplomas.Where(d => d.Title.Contains(request.SearchValue, StringComparison.OrdinalIgnoreCase)).ToList();
                var diplomaResponses = new GetAllDiplomasResponse()
                {
                    Diplomas = ActiveDiplomas.Skip((request.PageNum - 1) * request.ItemsPerPage).Take(request.ItemsPerPage)
                   .Select(d => new DiplomaResponse
                   {
                       Id = d.Id,
                       Title = d.Title,
                       Description = d.Description,
                       QuizCount = d.Quizzes.Count(),
                       StudentProgress =d.Quizzes.Any() ? ((decimal)d.Quizzes.Sum(q => q.PassScore)/(d.Quizzes.Count()*100)) *100 : 0
                   }).ToList(),
                    PageNum = request.PageNum,
                    ItemsPerPage = request.ItemsPerPage,
                    TotalCount = ActiveDiplomas.Count()

                };
                var options = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                _memoryCache.Set(cacheKey, diplomaResponses, options);

                return ApiResponse<GetAllDiplomasResponse>.Success(diplomaResponses);
            }
            else
            {
                var diplomaResponses = new GetAllDiplomasResponse()
                {
                    Diplomas = ActiveDiplomas.Skip((request.PageNum - 1) * request.ItemsPerPage).Take(request.ItemsPerPage)
                    .Select(d => new DiplomaResponse
                    {
                        Id = d.Id,
                        Title = d.Title,
                        Description = d.Description,
                        QuizCount = d.Quizzes.Count(),
                        StudentProgress =d.Quizzes.Any() ? ((decimal)d.Quizzes.Sum(q => q.PassScore)/(d.Quizzes.Count()*100)) *100 : 0
                    }).ToList(),
                    PageNum = request.PageNum,
                    ItemsPerPage = request.ItemsPerPage,
                    TotalCount = ActiveDiplomas.Count()
                };
                var options = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                _memoryCache.Set(cacheKey, diplomaResponses, options);

                return ApiResponse<GetAllDiplomasResponse>.Success(diplomaResponses);
            }
        }
    }
}
