using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Features.QuestionOptions.Queries
{
    public record GetOptionsByQuestionIdQurey(Guid QuestionId) : IRequest<List<Option>>;

    public class GetOptionsByQuestionIdQureyHandler : IRequestHandler<GetOptionsByQuestionIdQurey, List<Option>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetOptionsByQuestionIdQureyHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<Option>> Handle(GetOptionsByQuestionIdQurey request, CancellationToken cancellationToken)
        {
            var optionRepository = _unitOfWork.Repository<Option>();
            var options = optionRepository.Find(o => o.QuestionId == request.QuestionId).ToList();
            return options;
        }
    }
}
