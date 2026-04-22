using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Attempts.DTOs;
using ExaminationSystem.API.Common.Models;
using MediatR;

namespace Examination_System.Features.Attempts.Queries
{
    
    public record GetAttemptTimerQuery(Guid AttemptId) : IRequest<ApiResponse<GetAttemptTimerResponse>>;

    public class GetAttemptTimerQueryHandler : IRequestHandler<GetAttemptTimerQuery, ApiResponse<GetAttemptTimerResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        // private readonly ICurrentUserService _currentUserService;

        public GetAttemptTimerQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<GetAttemptTimerResponse>> Handle(GetAttemptTimerQuery request, CancellationToken cancellationToken)
        {
            var attemptRepository = _unitOfWork.Repository<Attempt>();
            var attempt = await attemptRepository.GetByIdAsync(request.AttemptId);

            if (attempt is null)
                return ApiResponse<GetAttemptTimerResponse>.Failure(ErrorCode.AttemptNotFound);

            /*
            var currentStudentId = _currentUserService.UserId;
            if (attempt.StudentId != currentStudentId)
                return ApiResponse<GetAttemptTimerResponse>.Failure(ErrorCode.Forbidden);
            */

            if (attempt.Status == "Submitted" || attempt.Status == "TimedOut")
                return ApiResponse<GetAttemptTimerResponse>.Failure(ErrorCode.AttemptClosed);

            var quizRepository = _unitOfWork.Repository<Quiz>();
            var quiz = await quizRepository.GetByIdAsync(attempt.QuizId);

            if (quiz is null)
                return ApiResponse<GetAttemptTimerResponse>.Failure(ErrorCode.QuizNotFound);

            var utcNow = DateTime.UtcNow;
            var deadline = attempt.StartTime.AddMinutes(attempt.Quiz.DurationMinutes);

            if (utcNow >= deadline)
            {
                var answerRepository = _unitOfWork.Repository<Answer>();
                var allAnswers =  answerRepository.GetAll();

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

                    var optionRepository = _unitOfWork.Repository<Option>();
                    var allOptions =  optionRepository.GetAll();

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

                return ApiResponse<GetAttemptTimerResponse>.Failure(ErrorCode.AttemptExpired);
            }

            var secondsRemaining = (int)Math.Floor((deadline - utcNow).TotalSeconds);
            secondsRemaining = Math.Max(0, secondsRemaining);

            var response = new GetAttemptTimerResponse
            {
                SecondsRemaining = secondsRemaining
            };

            return ApiResponse<GetAttemptTimerResponse>.Success(response);
        }
    }
}
