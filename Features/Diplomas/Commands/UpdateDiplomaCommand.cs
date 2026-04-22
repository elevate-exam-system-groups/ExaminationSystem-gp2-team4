using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Features.Diplomas.DTOs;
using Examination_System.Common.Exceptions;

namespace Examination_System.Features.Diplomas.Commands
{
    public record UpdateDiplomaCommandResult(bool IsSuccess, int StatusCode, DiplomaResponse? Data, string? Message);
    public record UpdateDiplomaCommand(Guid DiplomaId, UpdateDiplomaRequest Request) : IRequest<UpdateDiplomaCommandResult>;

    public class UpdateDiplomaCommandHandler : IRequestHandler<UpdateDiplomaCommand, UpdateDiplomaCommandResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDiplomaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateDiplomaCommandResult> Handle(UpdateDiplomaCommand request, CancellationToken cancellationToken)
        {
            var diploma = await _unitOfWork.Repository<Diploma>().GetByIdAsync(request.DiplomaId);
            if (diploma == null || diploma.DeletedAt != null)
            {
                return new UpdateDiplomaCommandResult(false, 404, null, "Diploma not found.");
            }
            

            diploma.Title = request.Request.Title ?? diploma.Title;
            diploma.Description = request.Request.Description ?? diploma.Description;
            diploma.ImageUrl = request.Request.ImageUrl ?? diploma.ImageUrl;
            diploma.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            var quizzes =  _unitOfWork.Repository<Quiz>().Find(q => q.DiplomaId == diploma.Id);

            var response = new DiplomaResponse
            {
                Id = diploma.Id,
                Title = diploma.Title,
                Description = diploma.Description,
                QuizCount = quizzes.Count(),
                StudentProgress = 0
            };

            return new UpdateDiplomaCommandResult(true, 200, response, null);
        }
    }
}
