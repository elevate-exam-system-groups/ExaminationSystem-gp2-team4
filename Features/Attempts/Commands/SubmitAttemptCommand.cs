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

namespace Examination_System.Features.Attempts.Commands
{
    public record SubmitAttemptCommandResult(bool IsConflict, bool IsNotFound, bool IsForbidden, SubmitAttemptResponse? Data, string? Message);
    public record SubmitAttemptCommand(Guid AttemptId, Guid UserId) : IRequest<SubmitAttemptCommandResult>;

    public class SubmitAttemptCommandHandler : IRequestHandler<SubmitAttemptCommand, SubmitAttemptCommandResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmitAttemptCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SubmitAttemptCommandResult> Handle(SubmitAttemptCommand request, CancellationToken cancellationToken)
        {
            var attempt = await _unitOfWork.Repository<Attempt>().GetByIdAsync(request.AttemptId);
            if (attempt == null)
            {
                return new SubmitAttemptCommandResult(false, true, false, null, "Attempt not found.");
            }

            if (attempt.UserId != request.UserId)
            {
                return new SubmitAttemptCommandResult(false, false, true, null, "You do not own this attempt.");
            }

            if (attempt.Status == "submitted")
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
                return new SubmitAttemptCommandResult(true, false, false, existingResult, "Attempt already submitted.");
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
                return new SubmitAttemptCommandResult(true, false, false, existingResult, "Attempt already timed out.");
            }

            var quizForDuration = await _unitOfWork.Repository<Quiz>().GetByIdAsync(attempt.QuizId);
            if (quizForDuration == null)
            {
                throw new AppException("Quiz not found.", 404);
            }

            var deadline = attempt.StartTime.AddMinutes(quizForDuration.DurationMinutes);
            bool isTimedOut = DateTime.UtcNow > deadline;

            var answers = await _unitOfWork.Repository<Answer>().FindAsync(a => a.AttemptId == attempt.Id);
            int correctAnswers = answers.Count(a => a.IsCorrect);
            int totalQuestions = attempt.TotalQuestions;
            int score = totalQuestions > 0 ? (correctAnswers * 100) / totalQuestions : 0;
            bool passed = score >= quizForDuration.PassScore;

            attempt.Score = score;
            attempt.SubmittedAt = DateTime.UtcNow;
            attempt.Status = isTimedOut ? "timed_out" : "submitted";
            attempt.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new SubmitAttemptCommandResult(false, false, false, new SubmitAttemptResponse
            {
                AttemptId = attempt.Id,
                Score = score,
                Passed = passed
            }, null);
        }
    }
}
