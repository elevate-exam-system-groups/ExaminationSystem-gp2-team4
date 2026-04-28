using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using MediatR;

namespace Examination_System.Features.QuestionOptions.Commands
{
    public record DeleteOptionCommand(Guid OptionId) : IRequest<bool>;

    public class DeleteOptionCommandHandler : IRequestHandler<DeleteOptionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteOptionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteOptionCommand request, CancellationToken cancellationToken)
        {
            var optionRepository = _unitOfWork.Repository<Option>();
            var option = await optionRepository.GetByIdAsync(request.OptionId);
            if (option is null)
                return false;
            optionRepository.Delete(option);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
