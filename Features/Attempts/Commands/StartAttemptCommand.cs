using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Attempts.DTOs;
using ExaminationSystem.API.Common.Models;

namespace Examination_System.Features.Attempts.Commands
{
    public record StartAttemptCommand(Guid QuizId, Guid UserId)
        : IRequest<ApiResponse<StartAttemptResponse>>;

    public class StartAttemptCommandHandler :
        IRequestHandler<StartAttemptCommand, ApiResponse<StartAttemptResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public StartAttemptCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<StartAttemptResponse>> Handle(
            StartAttemptCommand request,
            CancellationToken cancellationToken)
        {
            var quiz = await _unitOfWork.Repository<Quiz>()
                .GetByIdAsync(request.QuizId);

            if (quiz == null)
                return ApiResponse<StartAttemptResponse>.Failure(ErrorCode.QuizNotFound);

            var attempts = (await _unitOfWork.Repository<Attempt>()
                .FindAsync(a => a.UserId == request.UserId && a.QuizId == request.QuizId))
                .ToList();

            var activeAttempt = attempts.FirstOrDefault(a => a.Status == "in_progress");
            Attempt currentAttempt;

            if (activeAttempt != null)
            {
                currentAttempt = activeAttempt;
            }
            else
            {
                if (attempts.Count >= 3)
                {
                    return ApiResponse<StartAttemptResponse>
                        .Failure(ErrorCode.AttemptLimitReached);
                }

                currentAttempt = new Attempt
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    QuizId = request.QuizId,
                    Status = "in_progress",
                    StartTime = DateTime.UtcNow,
                    TotalQuestions = quiz.QuestionsCount,
                    Score = 0
                };

                await _unitOfWork.Repository<Attempt>()
                    .AddAsync(currentAttempt);

                await _unitOfWork.SaveChangesAsync();
            }

            var questions = (await _unitOfWork.Repository<Question>()
                .FindAsync(q => q.QuizId == request.QuizId))
                .ToList();

            var questionIds = questions.Select(q => q.Id).ToList();

            var options = (await _unitOfWork.Repository<Option>()
                .FindAsync(o => questionIds.Contains(o.QuestionId)))
                .ToList();

            var shuffledQuestions = questions
                .OrderBy(x => Guid.NewGuid())
                .ToList();

            var questionDtos = shuffledQuestions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Body = q.Body,
                Type = q.Type,
                Options = options
                    .Where(o => o.QuestionId == q.Id)
                    .OrderBy(x => Guid.NewGuid())
                    .Select(o => new OptionDto
                    {
                        Id = o.Id,
                        Body = o.Body
                    })
                    .ToList()
            }).ToList();

            var response = new StartAttemptResponse
            {
                AttemptId = currentAttempt.Id,
                QuizId = currentAttempt.QuizId,
                Status = currentAttempt.Status,
                StartTime = currentAttempt.StartTime,
                Questions = questionDtos
            };

            if (activeAttempt != null)
                return ApiResponse<StartAttemptResponse>
                    .Failure(ErrorCode.AttemptInProgress);

            return ApiResponse<StartAttemptResponse>.Success(response);
        }
    }
}