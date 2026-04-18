using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Attempts.DTOs;
using ExaminationSystem.API.Common.Models;
using MediatR;

namespace Examination_System.Features.Attempts.Commands
{
    public record SaveAnswerCommand(Guid AttemptId, Guid QuestionId, Guid SelectedOptionId) : IRequest<ApiResponse<SaveAnswerResponse>>;

    public class SaveAnswerCommandHandler : IRequestHandler<SaveAnswerCommand, ApiResponse<SaveAnswerResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaveAnswerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<SaveAnswerResponse>> Handle(SaveAnswerCommand request, CancellationToken cancellationToken)
        {
            var attemptRepository = _unitOfWork.Repository<Attempt>();
            var questionRepository = _unitOfWork.Repository<Question>();
            var optionRepository = _unitOfWork.Repository<Option>();
            var answerRepository = _unitOfWork.Repository<Answer>();
            var quizRepository = _unitOfWork.Repository<Quiz>();


            var attempt = await attemptRepository.GetByIdAsync(request.AttemptId);

            if (attempt is null)
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.AttemptNotFound);

            /*
            var currentStudentId = _currentUserService.UserId;
            if (attempt.StudentId != currentStudentId)
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.Forbidden);
            */

            if (attempt.Status == "Submitted" || attempt.Status == "TimedOut")
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.AttemptClosed);

            var quiz = await quizRepository.GetByIdAsync(attempt.QuizId);

            if (quiz is null)
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.QuizNotFound);

            var utcNow = DateTime.UtcNow;
            var deadline = attempt.StartTime.AddMinutes(attempt.Quiz.DurationMinutes);

            if (utcNow >= deadline)
            {
                var allAnswers = await answerRepository.GetAllAsync();

                var attemptAnswersData = allAnswers
                    .Where(x => x.AttemptId == attempt.Id)
                    .Select(x => new
                    {
                        x.QuestionId,
                        x.OptionId
                    })
                    .ToList();

                decimal score = 0;

                if (attemptAnswersData.Count > 0)
                {
                    var selectedOptionIds = attemptAnswersData
                        .Select(x => x.OptionId)
                        .Distinct()
                        .ToList();

                    var allOptions = await optionRepository.GetAllAsync();

                    var correctOptionIds = allOptions
                        .Where(x => selectedOptionIds.Contains(x.Id) && x.IsCorrect)
                        .Select(x => x.Id)
                        .ToList();

                    var correctAnswersCount = attemptAnswersData
                        .Count(x => correctOptionIds.Contains(x.OptionId));

                    score = correctAnswersCount;
                }

                attempt.Status = "TimedOut";
                attempt.SubmittedAt = utcNow;
                attempt.Score = (int)score;

                attemptRepository.Update(attempt);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.AttemptExpired);
            }

            var question = await questionRepository.GetByIdAsync(request.QuestionId);

            if (question is null)
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.InvalidQuestion);

            if (question.QuizId != attempt.QuizId)
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.InvalidQuestion);

            var option = await optionRepository.GetByIdAsync(request.SelectedOptionId);

            if (option is null)
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.InvalidOption);

            if (option.QuestionId != request.QuestionId)
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.InvalidOption);

            var allExistingAnswers = await answerRepository.GetAllAsync();

            var existingAnswer = allExistingAnswers
                .FirstOrDefault(x =>
                    x.AttemptId == request.AttemptId &&
                    x.QuestionId == request.QuestionId);

            if (existingAnswer is null)
            {
                var newAnswer = new Answer
                {
                    Id = Guid.NewGuid(),
                    AttemptId = request.AttemptId,
                    QuestionId = request.QuestionId,
                    OptionId = request.SelectedOptionId
                };

<<<<<<< HEAD
                 answerRepository.AddAsync(newAnswer);
=======
                 answerRepository.Add(newAnswer);
>>>>>>> origin/Test
            }
            else
            {
                existingAnswer.OptionId = request.SelectedOptionId;
                answerRepository.Update(existingAnswer);
            }

            await _unitOfWork.SaveChangesAsync();

            var response = new SaveAnswerResponse
            {
                Saved = true
            };

            return ApiResponse<SaveAnswerResponse>.Success(response);
        }

    }
}
   

