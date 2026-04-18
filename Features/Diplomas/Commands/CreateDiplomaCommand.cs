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
    public record CreateDiplomaCommandResult(bool IsSuccess, int StatusCode, DiplomaResponse? Data, object? Errors);
    public record CreateDiplomaCommand(CreateDiplomaRequest Request) : IRequest<CreateDiplomaCommandResult>;

    public class CreateDiplomaCommandHandler : IRequestHandler<CreateDiplomaCommand, CreateDiplomaCommandResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDiplomaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateDiplomaCommandResult> Handle(CreateDiplomaCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Request.Title))
            {
                return new CreateDiplomaCommandResult(false, 422, null, new { title = "Title is required." });
            }

            var diploma = new Diploma
            {
                Id = Guid.NewGuid(),
                Title = request.Request.Title,
                Description = request.Request.Description ?? string.Empty,
                ImageUrl = request.Request.ImageUrl ?? string.Empty,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Diploma>().AddAsync(diploma);
            await _unitOfWork.SaveChangesAsync();

            var response = new DiplomaResponse
            {
                Id = diploma.Id,
                Title = diploma.Title,
                Description = diploma.Description,
                QuizCount = 0,
                StudentProgress = 0
            };

            return new CreateDiplomaCommandResult(true, 201, response, null);
        }
    }
}
