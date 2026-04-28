using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Questions.Queries;
using MediatR;

namespace Examination_System.Features.Questions.Commands
{
    public record DeleteQuestionCommand(Guid QuestionId):IRequest<ApiResponse<bool>>;

    public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand,ApiResponse<bool> >
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        public DeleteQuestionCommandHandler(IUnitOfWork unitOfWork, IMediator mediator  )
        {
            _unitOfWork = unitOfWork;
            _mediator=mediator;
        }
        public async Task<ApiResponse<bool>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            var questionRepository = _unitOfWork.Repository<Question>();

            var question = questionRepository.Find(q => q.Id == request.QuestionId).FirstOrDefault();

            if (question is null)
                return ApiResponse<bool>.Failure(ErrorCode.QuestionNotFound);


            questionRepository.Delete(question);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true);
        }
    }

}
