using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Answers.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Features.Answers.Queries
{
    public record GetAttemptAnswersQuery(Guid AttemptId):IRequest<IEnumerable<GetAttemptAnswersDTO>>;

    public class GetAttemptAnswersQueryHandler : IRequestHandler<GetAttemptAnswersQuery, IEnumerable<GetAttemptAnswersDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAttemptAnswersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<GetAttemptAnswersDTO>> Handle(GetAttemptAnswersQuery request, CancellationToken cancellationToken)
        {
            var answerRepository = _unitOfWork.Repository<Answer>();
            var attemptAnswersData = await answerRepository.GetAll()
                .Where(x => x.AttemptId == request.AttemptId)
                .Select(x => new GetAttemptAnswersDTO
                {
                    QuestionId = x.QuestionId,
                    OptionId = x.OptionId
                }).ToListAsync();
            return attemptAnswersData;
        }
    }

}
