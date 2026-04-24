using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Features.QuestionOptions.Queries
{
    public record GetAllCorrectOptions() : IRequest<IEnumerable<Option>>;

    public class GetAllCorrectOptionsHandler : IRequestHandler<GetAllCorrectOptions, IEnumerable<Option>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllCorrectOptionsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Option>> Handle(GetAllCorrectOptions request, CancellationToken cancellationToken)
        {
            var optionRepository = _unitOfWork.Repository<Option>();
            var correctOptions = await optionRepository.GetAll()
                .Where(o => o.IsCorrect)
                .ToListAsync();

            return correctOptions;
        }

    }
}
