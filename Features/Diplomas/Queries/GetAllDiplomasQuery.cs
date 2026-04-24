using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Diplomas.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection.Metadata.Ecma335;

namespace Examination_System.Features.Diplomas.Queries
{
    public record GetAllDiplomasQuery(int PageNum, int ItemsPerPage, string? SearchValue) : IRequest<ApiResponse<GetAllDiplomasDTO>>;

    public class GetAllDiplomasQueryHandler : IRequestHandler<GetAllDiplomasQuery, ApiResponse<GetAllDiplomasDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        private readonly IMediator _mediator;
        public GetAllDiplomasQueryHandler(IUnitOfWork unitOfWork, IMemoryCache memoryCache, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
            _mediator = mediator;
        }

        public async Task<ApiResponse<GetAllDiplomasDTO>> Handle(GetAllDiplomasQuery request, CancellationToken cancellationToken)
        {

            var cacheKey = $"GetAllDiplomas_{request.PageNum}_{request.ItemsPerPage}_{request.SearchValue}";
            if (_memoryCache.TryGetValue(cacheKey, out GetAllDiplomasDTO cachedResponse))
            {
                return ApiResponse<GetAllDiplomasDTO>.Success(cachedResponse);
            }

            var DiplomaRepository = _unitOfWork.Repository<Diploma>();
            var ActiveDiplomas = await _mediator.Send(new GetActiveDiplomasQuery());

            if (ActiveDiplomas is null || !ActiveDiplomas.Any())
                return ApiResponse<GetAllDiplomasDTO>.Failure(ErrorCode.EmptyDiplomaArray);

            if (!string.IsNullOrWhiteSpace(request.SearchValue))
            {
                ActiveDiplomas =  ActiveDiplomas.Where(d => d.Title.Contains(request.SearchValue));
                
                var diplomaResponses = PaginationProcess(ActiveDiplomas, request.PageNum, request.ItemsPerPage);

                var options = MemoryCacheEntryOptionsProcess();
                _memoryCache.Set(cacheKey, diplomaResponses, options);

                return ApiResponse<GetAllDiplomasDTO>.Success(diplomaResponses);
            }
            else
            {
                var diplomaResponses = PaginationProcess(ActiveDiplomas, request.PageNum, request.ItemsPerPage);

                var options = MemoryCacheEntryOptionsProcess();
                _memoryCache.Set(cacheKey, diplomaResponses, options);

                return ApiResponse<GetAllDiplomasDTO>.Success(diplomaResponses);
            }
        }
        private GetAllDiplomasDTO PaginationProcess(IQueryable<Diploma> ActiveDiplomas, int PageNum, int ItemsPerPage)
        {
            return new GetAllDiplomasDTO()
            {
                Diplomas = ActiveDiplomas.Skip((PageNum - 1) * ItemsPerPage).Take(ItemsPerPage)
                   .Select(d => new DiplomaResponse
                   {
                       Id = d.Id,
                       Title = d.Title,
                       Description = d.Description,
                       QuizCount = d.Quizzes.Count(),
                       StudentProgress =d.Quizzes.Any() ? ((decimal)d.Quizzes.Sum(q => q.PassScore)/(d.Quizzes.Count()*100)) *100 : 0
                   }).ToList(),
                PageNum = PageNum,
                ItemsPerPage = ItemsPerPage,
                TotalCount = ActiveDiplomas.Count()
            };
        }
        private MemoryCacheEntryOptions MemoryCacheEntryOptionsProcess()
        {
            return new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        }
    }     
}
