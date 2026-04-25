using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using MediatR;

namespace Examination_System.Features.Answers.Commands
{
    public record AddAnswerCommand(Answer Answer) : IRequest;

    public class AddAnswerCommandHandler : IRequestHandler<AddAnswerCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddAnswerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(AddAnswerCommand request, CancellationToken cancellationToken)
        {
            var answerRepository = _unitOfWork.Repository<Answer>();
             answerRepository.Add(request.Answer);
             await _unitOfWork.SaveChangesAsync();
        }
    }

}
