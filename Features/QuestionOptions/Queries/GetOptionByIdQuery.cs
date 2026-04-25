using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Features.Attempts.DTOs;
using MediatR;

namespace Examination_System.Features.QuestionOptions.Queries
{
    public record GetOptionByIdQuery(Guid OptionId) : IRequest<Option>;

    public class GetOptionByIdQueryHandler : IRequestHandler<GetOptionByIdQuery, Option>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetOptionByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Option> Handle(GetOptionByIdQuery request, CancellationToken cancellationToken)
        {
            var optionRepository = _unitOfWork.Repository<Option>();
            var option = await optionRepository.GetByIdAsync(request.OptionId);
            return option;
        }

    }
}
