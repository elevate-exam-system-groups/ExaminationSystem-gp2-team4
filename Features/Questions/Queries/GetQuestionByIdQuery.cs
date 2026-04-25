using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Features.Attempts.DTOs;
using MediatR;

namespace Examination_System.Features.Questions.Queries
{
    public record GetQuestionByIdQuery (Guid QuestionId) : IRequest<QuestionDto>;

    public class GetQuestionByIdQueryHandler : IRequestHandler<GetQuestionByIdQuery, QuestionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetQuestionByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<QuestionDto> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
        {
            var questionRepository = _unitOfWork.Repository<Question>();
            var question = await questionRepository.GetByIdAsync(request.QuestionId);
            if (question is null)
                return null;
            var questionDto = new QuestionDto
            {
                Id = question.Id,
                Body = question.Body,
                Type = question.Type,
                QuizId = question.QuizId,
            };
            return questionDto;
        }
    }
}   