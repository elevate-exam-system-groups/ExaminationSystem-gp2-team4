using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Answers.Commands;
using Examination_System.Features.Answers.Queries;
using Examination_System.Features.Attempts.DTOs;
using Examination_System.Features.QuestionOptions.Queries;
using Examination_System.Features.Questions.Queries;
using Examination_System.Features.Quizzes.Queries;
using ExaminationSystem.API.Common.Models;
using MediatR;

namespace Examination_System.Features.Attempts.Orchestrators
{
    public record SaveAnswerOrchestrator(Guid AttemptId, Guid QuestionId, Guid SelectedOptionId) : IRequest<ApiResponse<SaveAnswerResponse>>;

    public class SaveAnswerSaveAnswerOrchestratorHandler : IRequestHandler<SaveAnswerOrchestrator, ApiResponse<SaveAnswerResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public SaveAnswerSaveAnswerOrchestratorHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }
        public async Task<ApiResponse<SaveAnswerResponse>> Handle(SaveAnswerOrchestrator request, CancellationToken cancellationToken)
        {
            var attemptRepository = _unitOfWork.Repository<Attempt>();
            //var questionRepository = _unitOfWork.Repository<Question>();
            //var optionRepository = _unitOfWork.Repository<Option>();
            //var answerRepository = _unitOfWork.Repository<Answer>();
            //var quizRepository = _unitOfWork.Repository<Quiz>();


            var attempt = await attemptRepository.GetByIdAsync(request.AttemptId);

            if (attempt is null)
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.AttemptNotFound);

            if (attempt.Status == "Submitted" || attempt.Status == "TimedOut")
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.AttemptClosed);
            
            //var quiz = await quizRepository.GetByIdAsync(attempt.QuizId);
            var quiz = await _mediator.Send(new GetQuizByIdQuery(attempt.QuizId));

            if (quiz is null)
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.QuizNotFound);

            var utcNow = DateTime.UtcNow;
            var deadline = attempt.StartTime.AddMinutes(attempt.Quiz.DurationMinutes);

            if (utcNow >= deadline)
            {
                //var allAnswers = answerRepository.GetAll();

                //var attemptAnswersData = allAnswers
                //    .Where(x => x.AttemptId == attempt.Id)
                //    .Select(x => new
                //    {
                //        x.QuestionId,
                //        x.OptionId
                //    })
                //    .ToList();

                //decimal score = 0;

                //if (attemptAnswersData.Count > 0)
                //{
                //    var selectedOptionIds = attemptAnswersData
                //        .Select(x => x.OptionId)
                //        .Distinct()
                //        .ToList();

                //    var allOptions = optionRepository.GetAll();

                //    var correctOptionIds = allOptions
                //        .Where(x => selectedOptionIds.Contains(x.Id) && x.IsCorrect)
                //        .Select(x => x.Id)
                //        .ToList();

                //    var correctAnswersCount = attemptAnswersData
                //        .Count(x => correctOptionIds.Contains(x.OptionId));

                //    score = correctAnswersCount;
                //}

                //attempt.Status = "TimedOut";
                //attempt.SubmittedAt = utcNow;
                //attempt.Score = (int)score;

                //attemptRepository.Update(attempt);
                //await _unitOfWork.SaveChangesAsync();

                //return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.AttemptExpired);
                var attemptAnswersData = await _mediator.Send(new GetAttemptAnswersQuery(attempt.Id));

                decimal score = 0;

                if (attemptAnswersData.Count() > 0)
                {

                    var CorrectOptions = await _mediator.Send(new GetAllCorrectOptions());

                    var CorrectOptionsIds = CorrectOptions.Select(o => o.Id).ToList();


                    var CorrectOptionsCount = attemptAnswersData
                        .Count(x => CorrectOptionsIds.Contains(x.OptionId));

                    score = CorrectOptionsCount;
                }
                var UpdateAttempt = await attemptRepository.GetByIdAsync(attempt.Id);

                UpdateAttempt.Status = "TimedOut";
                UpdateAttempt.SubmittedAt = utcNow;
                UpdateAttempt.Score = (int)score;

                attemptRepository.Update(UpdateAttempt);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.AttemptExpired);
            }
            var question = await _mediator.Send(new GetQuestionByIdQuery(request.QuestionId));
            //var question = await questionRepository.GetByIdAsync(request.QuestionId);

            if (question is null)
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.InvalidQuestion);

            if (question.QuizId != attempt.QuizId)
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.InvalidQuestion);

            var option = await _mediator.Send(new GetOptionByIdQuery(request.SelectedOptionId));
            //var option = await optionRepository.GetByIdAsync(request.SelectedOptionId);

            if (option is null)
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.InvalidOption);

            if (option.QuestionId != request.QuestionId)
                return ApiResponse<SaveAnswerResponse>.Failure(ErrorCode.InvalidOption);

            var ExistingAnswer= await _mediator.Send(new GetExistingAnswerQuery(request.QuestionId, request.AttemptId));
            //var AllAnswers =  answerRepository.GetAll();

            //var ExistingAnswer = AllAnswers
            //    .FirstOrDefault(x =>
            //        x.AttemptId == request.AttemptId &&
            //        x.QuestionId == request.QuestionId);

            if (ExistingAnswer is null)
            {
                var newAnswer = new Answer
                {
                    Id = Guid.NewGuid(),
                    AttemptId = request.AttemptId,
                    QuestionId = request.QuestionId,
                    OptionId = request.SelectedOptionId
                };

              await _mediator.Send(new AddAnswerCommand(newAnswer));
            }
            else
            {
                ExistingAnswer.OptionId = request.SelectedOptionId;
                await _mediator.Send(new UpdateAnswerCommand(ExistingAnswer));
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
   

