using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using MediatR;

namespace Examination_System.Features.QuestionOptions.Commands
{
    public record AddOptionCommand(Option Option) : IRequest<bool>;

    public class AddOptionCommandHandler : IRequestHandler<AddOptionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddOptionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(AddOptionCommand request, CancellationToken cancellationToken)
        {
            var optionRepository = _unitOfWork.Repository<Option>();
             optionRepository.Add(request.Option);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }


}
