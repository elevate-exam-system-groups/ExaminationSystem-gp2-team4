using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Features.Attempts.Queries
{
    public record GetAttemptByIdQuery(Guid AttemptId) : IRequest<Attempt>;

    public class GetAttemptByIdQueryHandler : IRequestHandler<GetAttemptByIdQuery, Attempt>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAttemptByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Attempt> Handle(GetAttemptByIdQuery request, CancellationToken cancellationToken)
        {
            var attemptRepository = _unitOfWork.Repository<Attempt>();
            var attempt = await attemptRepository.GetAll()
                .Where(x => x.Id == request.AttemptId)
                .FirstOrDefaultAsync();
            return attempt;
        }
    }



}
