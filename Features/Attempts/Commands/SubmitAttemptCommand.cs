using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Examination_System.Common.Models;
using ExaminationSystem.API.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Features.Attempts.DTOs;
using Examination_System.Common.Exceptions;
using Examination_System.Common.Wrappers;

namespace Examination_System.Features.Attempts.Commands
{
public record SubmitAttemptCommand(Guid AttemptId, Guid UserId) : IRequest<ApiResponse<SubmitAttemptResponse>>;

    public class SubmitAttemptCommandHandler : IRequestHandler<SubmitAttemptCommand, ApiResponse<SubmitAttemptResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmitAttemptCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<SubmitAttemptResponse>> Handle(SubmitAttemptCommand request, CancellationToken cancellationToken)
        {
            var attempt = await _unitOfWork.Repository<Attempt>().GetByIdAsync(request.AttemptId);
if (attempt == null)
            {
                return ApiResponse<SubmitAttemptResponse>.Failure(ErrorCode.AttemptNotFound);
            }

            if (attempt.UserId != request.UserId)
            {
                return ApiResponse<SubmitAttemptResponse>.Failure(ErrorCode.Forbidden);
            }

            if (attempt.Status == "submitted")

            {
                var quiz = await _unitOfWork.Repository<Quiz>().GetByIdAsync(attempt.QuizId);
                if (quiz == null)
                {
                    return ApiResponse<SubmitAttemptResponse>.Failure(ErrorCode.QuizNotFound);
                }
                var existingResult = new SubmitAttemptResponse
                {
                    AttemptId = attempt.Id,
                    Score = attempt.Score,
                    Passed = attempt.IsPassed
                };
                return ApiResponse<SubmitAttemptResponse>.Success(existingResult);
            }

            if (attempt.Status == "timed_out")
            {
                var quiz = await _unitOfWork.Repository<Quiz>().GetByIdAsync(attempt.QuizId);
                if (quiz == null)
                {
                    throw new AppException("Quiz not found.", 404);
                }
                var existingResult = new SubmitAttemptResponse
                {
                    AttemptId = attempt.Id,
                    Score = attempt.Score,
                    Passed = attempt.Score >= quiz.PassScore
                };
                return ApiResponse<SubmitAttemptResponse>.Success(existingResult);
            }

            var quizForDuration = await _unitOfWork.Repository<Quiz>().GetByIdAsync(attempt.QuizId);
            if (quizForDuration == null)
            {
                return ApiResponse<SubmitAttemptResponse>.Failure(ErrorCode.QuizNotFound);
            }

            var deadline = attempt.StartTime.AddMinutes(quizForDuration.DurationMinutes);
            bool isTimedOut = DateTime.UtcNow > deadline;

            var answers = _unitOfWork.Repository<Answer>().Find(a => a.AttemptId == attempt.Id);
            int correctAnswers = answers.Count(a => a.IsCorrect);
            int totalQuestions = attempt.TotalQuestions;
            int score = totalQuestions > 0 ? (correctAnswers * 100) / totalQuestions : 0;
            bool passed = score >= quizForDuration.PassScore;

            attempt.Score = score;
            attempt.IsPassed = passed; 
            attempt.SubmittedAt = DateTime.UtcNow;
            attempt.Status = isTimedOut ? "timed_out" : "submitted";
            attempt.UpdatedAt = DateTime.UtcNow;

await _unitOfWork.SaveChangesAsync();

            var result = new SubmitAttemptResponse
            {
                AttemptId = attempt.Id,
                Score = attempt.Score,
                Passed = attempt.IsPassed
            };
            return ApiResponse<SubmitAttemptResponse>.Success(result);
        }
    }
}
