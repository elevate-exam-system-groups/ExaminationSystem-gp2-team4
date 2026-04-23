using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Quizzes.DTOs;
using MediatR;

namespace Examination_System.Features.Quizzes.Commands;

public record PublishQuizCommand(Guid QuizId)
    : IRequest<ApiResponse<PublishQuizResult>>;


public class PublishQuizCommandHandler 
    : IRequestHandler<PublishQuizCommand, ApiResponse<PublishQuizResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public PublishQuizCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PublishQuizResult>> Handle(
        PublishQuizCommand request,
        CancellationToken cancellationToken)
    {
        var quizRepo = _unitOfWork.Repository<Quiz>();

        var quiz = quizRepo.Find(q => q.Id == request.QuizId).FirstOrDefault();

        if (quiz == null)
            return ApiResponse<PublishQuizResult>.Failure(ErrorCode.QuizNotFound);

        var validationError = ValidatePublishQuiz(quiz);
        if (validationError != ErrorCode.None)
            return ApiResponse<PublishQuizResult>.Failure(validationError);

        quiz.Status = "published";
        quizRepo.Update(quiz);

        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<PublishQuizResult>.Success(new PublishQuizResult
        {
            QuizId = quiz.Id,
            Status = quiz.Status
        });
    }
    private ErrorCode ValidatePublishQuiz(Quiz quiz)
    {
        // Already published
        if (quiz.Status == "published")
            return ErrorCode.Conflict;

        // Must have questions
        var questions = _unitOfWork.Repository<Question>()
            .Find(q => q.QuizId == quiz.Id)
            .ToList();

        if (!questions.Any())
            return ErrorCode.UnprocessableEntity;

        // Each question must have options
        var optionRepo = _unitOfWork.Repository<Option>();

        var hasInvalidQuestion = questions.Any(q =>
            !optionRepo.Find(o => o.QuestionId == q.Id).Any());

        if (hasInvalidQuestion)
            return ErrorCode.InvalidQuestion;

        return ErrorCode.None;
    }
}