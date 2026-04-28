using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.QuestionOptions.Commands;
using Examination_System.Features.QuestionOptions.Queries;
using Examination_System.Features.Questions.ViewModels;
using Examination_System.Features.Quizzes.Queries;
using MediatR;

namespace Examination_System.Features.Questions.Orchestrators
{
    public record UpdateQuestionOrchestrator(UpdateQuestionDTO UpdateQuestionDTO) : IRequest<ApiResponse<bool>>;

    public class UpdateQuestionOrchestratorHandler : IRequestHandler<UpdateQuestionOrchestrator, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        public UpdateQuestionOrchestratorHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }
        public async Task<ApiResponse<bool>> Handle(UpdateQuestionOrchestrator request, CancellationToken cancellationToken)
        {
            var quiz = await _mediator.Send(new GetQuizByIdQuery(request.UpdateQuestionDTO.QuizId));
            if (quiz is null)
                return ApiResponse<bool>.Failure(ErrorCode.QuizNotFound);

            var questionRepository = _unitOfWork.Repository<Question>();

            var question = questionRepository.Find(q => q.Id==request.UpdateQuestionDTO.QuestionId).FirstOrDefault();

            if (question is null)
                return ApiResponse<bool>.Failure(ErrorCode.QuestionNotFound);

            question.Body = request.UpdateQuestionDTO.Text;
            question.OrderIndex = request.UpdateQuestionDTO.OrderIndex;
            question.Explanation= request.UpdateQuestionDTO.Explanation;

            if (request.UpdateQuestionDTO.Options.Count()<2)
                return ApiResponse<bool>.Failure(ErrorCode.OptionsLessThanTwo);

            if (request.UpdateQuestionDTO.Options.Count(o => o.IsCorrect)!=1)
                return ApiResponse<bool>.Failure(ErrorCode.ThereIsNoCorrectOption);

            var oldOptions = await _mediator.Send(new GetOptionsByQuestionIdQurey(request.UpdateQuestionDTO.QuestionId));

            var sentOptionsId =request.UpdateQuestionDTO.Options.Select(x => x.Id).ToList();

            foreach (var option in oldOptions)
            {
                if (!sentOptionsId.Contains(option.Id))
                {
                    await _mediator.Send(new DeleteOptionCommand(option.Id));
                }   
            }

            foreach (var option in request.UpdateQuestionDTO.Options)
            {
                if (option.Id is null)
                {
                    var newOption = new Option
                    {
                        Body = option.Text,
                        IsCorrect = option.IsCorrect,
                        OrderIndex = option.OrderIndex,
                        QuestionId = question.Id
                    };
                    var reusult = await _mediator.Send(new AddOptionCommand(newOption)); 
                }
                else
                {
                    var optionToUpdate = oldOptions.FirstOrDefault(o => o.Id == option.Id);
                    if (optionToUpdate is not null)
                    {
                        optionToUpdate.Body = option.Text;
                        optionToUpdate.IsCorrect = option.IsCorrect;
                        optionToUpdate.OrderIndex = option.OrderIndex;
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<bool>.Success(true);
        }
    }
}
