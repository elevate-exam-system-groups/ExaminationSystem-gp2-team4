using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using MediatR;

namespace Examination_System.Features.Answers.Commands
{
    public record UpdateAnswerCommand(Answer Answer) : IRequest;
   
    public class UpdateAnswerCommandHandler : IRequestHandler<UpdateAnswerCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateAnswerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(UpdateAnswerCommand request, CancellationToken cancellationToken)
        {
            var answerRepository = _unitOfWork.Repository<Answer>();
             answerRepository.Update(request.Answer);
             await _unitOfWork.SaveChangesAsync();
        }
    }
}
