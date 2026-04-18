using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Examination_System.Common.Models;
using ExaminationSystem.API.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Exceptions;

namespace Examination_System.Features.Diplomas.Commands
{
    public record DeleteDiplomaCommandResult(bool IsSuccess, int StatusCode, string? Message);
    public record DeleteDiplomaCommand(Guid DiplomaId) : IRequest<DeleteDiplomaCommandResult>;

    public class DeleteDiplomaCommandHandler : IRequestHandler<DeleteDiplomaCommand, DeleteDiplomaCommandResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDiplomaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DeleteDiplomaCommandResult> Handle(DeleteDiplomaCommand request, CancellationToken cancellationToken)
        {
            var diploma = await _unitOfWork.Repository<Diploma>().GetByIdAsync(request.DiplomaId);
            if (diploma == null || diploma.DeletedAt != null)
            {
                return new DeleteDiplomaCommandResult(false, 404, "Diploma not found.");
            }

            var quizzes = await _unitOfWork.Repository<Quiz>().FindAsync(q => q.DiplomaId == request.DiplomaId);
            var quizIds = quizzes.Select(q => q.Id).ToList();

            var hasEnrollments = await _unitOfWork.Repository<Attempt>().FindAsync(a => quizIds.Contains(a.QuizId));

            if (hasEnrollments.Any())
            {
                return new DeleteDiplomaCommandResult(false, 409, "Cannot delete diploma with active student enrollments.");
            }

            diploma.DeletedAt = DateTime.UtcNow;
            diploma.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new DeleteDiplomaCommandResult(true, 200, "Diploma deleted successfully.");
        }
    }
}
