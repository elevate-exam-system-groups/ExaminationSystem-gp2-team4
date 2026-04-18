using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Quizzes.DTOs;
using Examination_System.Features.Quizzes.Queries;
using MediatR;

namespace Examination_System.Features.Quizzes.Commands;

public record UpdateQuizCommand(UpdateQuizRequest Request) : IRequest<ApiResponse<UpdateQuizResponse>>;

public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand, ApiResponse<UpdateQuizResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateQuizCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<ApiResponse<UpdateQuizResponse>> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.Repository<Quiz>().GetByIdAsync(request.Request.Id);
        if (quiz == null)
        {
            return ApiResponse<UpdateQuizResponse>.Failure(ErrorCode.QuizNotFound);
        }

        // Apply updates if provided in request
        if (!string.IsNullOrWhiteSpace(request.Request.Title) && request.Request.Title != quiz.Title)
        {
            var existingQuiz = await _unitOfWork.Repository<Quiz>()
                .FindAsync(q => q.DiplomaId == quiz.DiplomaId 
                             && q.Title == request.Request.Title 
                             && q.Id != quiz.Id);
            
            if (existingQuiz.Any())
            {
                return ApiResponse<UpdateQuizResponse>.Failure(ErrorCode.QuizTitleExists);
            }
            quiz.Title = request.Request.Title;
        }

        if (request.Request.Instructions != null)
        {
            quiz.Instructions = request.Request.Instructions;
        }

        if (request.Request.PassScore.HasValue)
        {
            if (request.Request.PassScore < 0 || request.Request.PassScore > 100)
            {
                return ApiResponse<UpdateQuizResponse>.Failure(ErrorCode.InvalidQuizData);
            }
            quiz.PassScore = request.Request.PassScore.Value;
        }

        if (request.Request.MaxAttempts.HasValue)
        {
            if (request.Request.MaxAttempts < 1 || request.Request.MaxAttempts > 10)
            {
                return ApiResponse<UpdateQuizResponse>.Failure(ErrorCode.InvalidQuizData);
            }
            quiz.MaxAttempts = request.Request.MaxAttempts.Value;
        }

        if (request.Request.DurationMinutes.HasValue)
        {
            if (request.Request.DurationMinutes <= 0 || request.Request.DurationMinutes > 480)
            {
                return ApiResponse<UpdateQuizResponse>.Failure(ErrorCode.InvalidQuizData);
            }
            quiz.DurationMinutes = request.Request.DurationMinutes.Value;
        }

        quiz.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        var response = new UpdateQuizResponse
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Instructions = quiz.Instructions,
            PassScore = quiz.PassScore,
            MaxAttempts = quiz.MaxAttempts,
            DurationMinutes = quiz.DurationMinutes,
            QuestionsCount = quiz.QuestionsCount
        };
        return ApiResponse<UpdateQuizResponse>.Success(response);       
    }
}