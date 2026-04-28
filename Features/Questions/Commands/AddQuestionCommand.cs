using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Questions.DTOs;
using Examination_System.Features.Quizzes.Queries;
using MediatR;

namespace Examination_System.Features.Questions.Commands
{
    public record AddQuestionCommand(AddQuestionDTO AddQuestionDTO) : IRequest<ApiResponse<bool>>;

    public class AddQuestionCommandHandler : IRequestHandler<AddQuestionCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        public AddQuestionCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }
        public async Task<ApiResponse<bool>> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
        {
            var QuestionRepository = _unitOfWork.Repository<Question>();

            if (request.AddQuestionDTO.Options.Count() <2)
                return ApiResponse<bool>.Failure(ErrorCode.OptionsLessThanTwo);

            if (!request.AddQuestionDTO.Options.Any(o => o.IsCorrect))
                return ApiResponse<bool>.Failure(ErrorCode.ThereIsNoCorrectOption);

            var quiz = await _mediator.Send(new GetQuizByIdQuery(request.AddQuestionDTO.QuizId));

            if (quiz is null)
                return ApiResponse<bool>.Failure(ErrorCode.QuizNotFound);

            var newQuestion = new Question
            {
                QuizId= request.AddQuestionDTO.QuizId,
                Body= request.AddQuestionDTO.Text,
                Options= request.AddQuestionDTO.Options.Select(o => new Option
                {
                    Body = o.Text,
                    IsCorrect = o.IsCorrect,
                    OrderIndex = o.OrderIndex
                }).ToList(),
                OrderIndex  = request.AddQuestionDTO.OrderIndex,
                Explanation = request.AddQuestionDTO.Explanation
            };

            QuestionRepository.Add(newQuestion);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true);
        }
    }
}
