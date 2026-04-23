using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Quizzes.DTOs;
using MediatR;

namespace Examination_System.Features.Quizzes.Commands;

public record UnpublishQuizCommand(Guid QuizId)
    : IRequest<ApiResponse<PublishQuizResult>>;


public class UnpublishQuizCommandHandler
    : IRequestHandler<UnpublishQuizCommand, ApiResponse<PublishQuizResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnpublishQuizCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PublishQuizResult>> Handle(
        UnpublishQuizCommand request,
        CancellationToken cancellationToken)
    {
        var quizRepo = _unitOfWork.Repository<Quiz>();

        // 1. Get Quiz
        var quiz = quizRepo.Find(q => q.Id == request.QuizId).FirstOrDefault();

        if (quiz == null)
            return ApiResponse<PublishQuizResult>.Failure(ErrorCode.QuizNotFound);

        // 2. Must be published already
        if (quiz.Status != "published")
            return ApiResponse<PublishQuizResult>.Failure(ErrorCode.Conflict);

        // 3. Check active attempts (InProgress)
        var hasActiveAttempts = _unitOfWork.Repository<Attempt>()
            .Find(a => a.QuizId == quiz.Id && a.Status == "InProgress")
            .Any();

        if (hasActiveAttempts)
            return ApiResponse<PublishQuizResult>.Failure(ErrorCode.Conflict);

        // 4. Unpublish (back to draft)
        quiz.Status = "draft";
        quizRepo.Update(quiz);

        await _unitOfWork.SaveChangesAsync();

        // 5. Response
        return ApiResponse<PublishQuizResult>.Success(new PublishQuizResult
        {
            QuizId = quiz.Id,
            Status = quiz.Status
        });
    }
}