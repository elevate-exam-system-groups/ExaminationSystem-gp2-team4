using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Diplomas.DTOs;
using Examination_System.Features.Diplomas.Queries;
using Examination_System.Features.Quizzes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;

namespace Examination_System.Features.Quizzes.Queries
{
    public record GetQuizzesByDiplomaIdQuery(string DiplomaId,int PageNum,int ItemPerPage,string? SearchValue) : IRequest<ApiResponse<GetQuizzesResponse>>;

    public class GetQuizzesByDiplomaIdQueryHandler : IRequestHandler<GetQuizzesByDiplomaIdQuery, ApiResponse<GetQuizzesResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        private readonly IMediator _mediator;
        public GetQuizzesByDiplomaIdQueryHandler(IUnitOfWork unitOfWork, IMemoryCache memoryCache, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
            _mediator = mediator;
        }
        public async Task<ApiResponse<GetQuizzesResponse>> Handle(GetQuizzesByDiplomaIdQuery request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrEmpty(request.DiplomaId))
                return ApiResponse<GetQuizzesResponse>.Failure(ErrorCode.NoParamSent);

            Guid DiplomaId;
            if (!Guid.TryParse(request.DiplomaId, out DiplomaId))
                return ApiResponse<GetQuizzesResponse>.Failure(ErrorCode.InvaildDiplomaId);

            var ActiveDiploma = await _mediator.Send(new GetDiplomaByIdQuery(request.DiplomaId));

            if(ActiveDiploma.ErrorCode == ErrorCode.DiplomaIsNotActive)
                return ApiResponse<GetQuizzesResponse>.Failure(ErrorCode.DiplomaIsNotActive);

            var _quizRepository = _unitOfWork.Repository<Quiz>();

            var quizzes =  _quizRepository.GetAll();
            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                quizzes = quizzes.Where(d => d.Title.Contains(request.SearchValue));
                    
                var DiplomaQuizzesResponse = PaginationProcess(quizzes, DiplomaId, request.PageNum, request.ItemPerPage);
                return ApiResponse<GetQuizzesResponse>.Success(DiplomaQuizzesResponse);
            }
            else
            {
                var DiplomaQuizzesResponse = PaginationProcess(quizzes, DiplomaId, request.PageNum, request.ItemPerPage);   
                return ApiResponse<GetQuizzesResponse>.Success(DiplomaQuizzesResponse);
            }           
        }
        private GetQuizzesResponse PaginationProcess(IQueryable<Quiz> quizzes,Guid DiplomaId, int pageNum, int itemPerPage)
        {
            var DiplomaQuizzes = quizzes.Where(q => q.DiplomaId == DiplomaId)
                .Skip((pageNum-1) * itemPerPage).Take(itemPerPage);

            var response = new GetQuizzesResponse
            {
                Quizzes = DiplomaQuizzes.Select(q => new QuizResponse
                {
                    Id = q.Id,
                    QuestionsCount  =q.Questions.Count(),
                    MaxAttempts=q.Attempts.Count(),
                    Title = q.Title,
                    DurationMinutes = q.DurationMinutes,
                    PassScore = q.PassScore,
                    Status = q.Status
                }).ToList(),
                TotalCount = quizzes.Count(),
                ItemsPerPage=itemPerPage,
                PageNum=pageNum
            };
            return response;
        }
    }
}
