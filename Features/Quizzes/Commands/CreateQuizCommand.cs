using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Quizzes.DTOs;
using MediatR;

namespace Examination_System.Features.Quizzes.Commands;

public record CreateQuizCommand(CreateQuizRequst Request) : IRequest<ApiResponse<CreateQuizResult>>;

public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand,ApiResponse<CreateQuizResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateQuizCommandHandler(IUnitOfWork unitOfWork) =>  _unitOfWork = unitOfWork;
    
    public async Task<ApiResponse<CreateQuizResult>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        var diplomaResult = await  _unitOfWork.Repository<Diploma>().GetByIdAsync(request.Request.DiplomaId);

        if (diplomaResult == null)
        {
            return ApiResponse<CreateQuizResult>.Failure(ErrorCode.DiplomaNotFound);
        }

        var existingQuiz = await _unitOfWork.Repository<Quiz>()
            .FindAsync(q => q.DiplomaId == request.Request.DiplomaId 
                         && q.Title == request.Request.Title);
        
        if (existingQuiz.Any())
        {
            return ApiResponse<CreateQuizResult>.Failure(ErrorCode.QuizTitleExists);
        }

        var quiz = new Quiz
        {
            DiplomaId = request.Request.DiplomaId,
            Title = request.Request.Title,
            DurationMinutes = request.Request.DurationMinutes,
            PassScore = request.Request.PassScore,
            MaxAttempts = request.Request.MaxAttempts,
            Instructions = request.Request.Instructions,
            Status = "draft",
            QuestionsCount = request.Request.QuestionsCount
        };
        _unitOfWork.Repository<Quiz>().Add(quiz);
        await _unitOfWork.SaveChangesAsync();
        var response = new CreateQuizResult
        {
            Id = quiz.Id,
            DiplomaId = quiz.DiplomaId,
            Status = quiz.Status,
            CreatedAt = quiz.CreatedAt,
            TotalQuestions = quiz.QuestionsCount,
            PassScore = quiz.PassScore
        };
        return ApiResponse<CreateQuizResult>.Success(response);
    }

    
}