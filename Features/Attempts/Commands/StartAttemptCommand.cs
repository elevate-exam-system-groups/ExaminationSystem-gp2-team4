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
    public record StartAttemptCommandResult(bool IsConflict, StartAttemptResponse Data);
    public record StartAttemptCommand(Guid QuizId, Guid UserId) : IRequest<StartAttemptCommandResult>;

    public class StartAttemptCommandHandler : IRequestHandler<StartAttemptCommand, StartAttemptCommandResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public StartAttemptCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StartAttemptCommandResult> Handle(StartAttemptCommand request, CancellationToken cancellationToken)
        {
            var quiz = await _unitOfWork.Repository<Quiz>().GetByIdAsync(request.QuizId);
            if (quiz == null)
            {
                throw new AppException("Quiz not found.", 404);
            }

            var attempts = await _unitOfWork.Repository<Attempt>()
                .FindAsync(a => a.UserId == request.UserId && a.QuizId == request.QuizId);

            var activeAttempt = attempts.FirstOrDefault(a => a.Status == "in_progress");
            Attempt currentAttempt;
            bool isConflict = false;

            if (activeAttempt != null)
            {
                currentAttempt = activeAttempt;
                isConflict = true;
            }
            else
            {
                if (attempts.Count() >= 3)
                {
                    throw new AppException("Maximum limit of 3 attempts reached for this quiz.", 403);
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

                await _unitOfWork.Repository<Attempt>().AddAsync(currentAttempt);
                await _unitOfWork.SaveChangesAsync();
            }

            var questions = await _unitOfWork.Repository<Question>().FindAsync(q => q.QuizId == request.QuizId);
            
            // Extract the list of question IDs to optimize the options query
            var questionIds = questions.Select(q => q.Id).ToList();
            var options = await _unitOfWork.Repository<Option>().FindAsync(o => questionIds.Contains(o.QuestionId));

            // Shuffle questions and options randomly
            var shuffledQuestions = questions.OrderBy(x => Guid.NewGuid()).ToList();

            var questionDtos = shuffledQuestions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Body = q.Body,
                Type = q.Type,
                Options = options.Where(o => o.QuestionId == q.Id)
                                 .OrderBy(x => Guid.NewGuid())
                                 .Select(o => new OptionDto
                                 {
                                     Id = o.Id,
                                     Body = o.Body
                                 }).ToList()
            }).ToList();

            return new StartAttemptCommandResult(isConflict, new StartAttemptResponse
            {
                AttemptId = currentAttempt.Id,
                QuizId = currentAttempt.QuizId,
                Status = currentAttempt.Status,
                StartTime = currentAttempt.StartTime,
                Questions = questionDtos
            });
        }
    }
}
