using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Quizzes.DTOs;
using MediatR;

namespace Examination_System.Features.Quizzes.Queries;


public record GetQuizByIdQuery(Guid Id) : IRequest<ApiResponse<QuizResponse>>;

public class GetQuizByIdQueryHandler : IRequestHandler<GetQuizByIdQuery, ApiResponse<QuizResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetQuizByIdQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<ApiResponse<QuizResponse>> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.Repository<Quiz>().GetByIdAsync(request.Id);
        if (quiz == null)
        {
            return  ApiResponse<QuizResponse>.Failure(ErrorCode.QuizNotFound);
        }

        var response = new QuizResponse
        {
            Id = quiz.Id,
            DiplomaId = quiz.DiplomaId,
            CreatedAt = quiz.CreatedAt,
            UpdatedAt = quiz.UpdatedAt,
            QuestionsCount = quiz.QuestionsCount,
            MaxAttempts = quiz.MaxAttempts,
            Title = quiz.Title,
            DurationMinutes = quiz.DurationMinutes,
            PassScore = quiz.PassScore,
            Status = quiz.Status
        };
        return ApiResponse<QuizResponse>.Success(response);       
    }
}