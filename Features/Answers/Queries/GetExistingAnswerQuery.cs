using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Features.Answers.Queries
{
    public record GetExistingAnswerQuery(Guid QuestionId, Guid AttemptId) : IRequest<Answer>;

    public class GetExistingAnswerQueryHandler : IRequestHandler<GetExistingAnswerQuery, Answer>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetExistingAnswerQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Answer> Handle(GetExistingAnswerQuery request, CancellationToken cancellationToken)
        {
            var answerRepository = _unitOfWork.Repository<Answer>();
            var existingAnswer = await answerRepository.GetAll()
                .FirstOrDefaultAsync(a => a.QuestionId == request.QuestionId && a.AttemptId == request.AttemptId);
            return existingAnswer;
        }
    }
}
