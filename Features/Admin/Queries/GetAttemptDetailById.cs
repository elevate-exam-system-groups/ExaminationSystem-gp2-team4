using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Admin.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Features.Admin.Queries
{
    public record GetAttemptDetailById(Guid AttemptId) : IRequest<ApiResponse<AttemptDetailDTO>>;
    
    public class GetAttemptDetailByIdHandler : IRequestHandler<GetAttemptDetailById, ApiResponse<AttemptDetailDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAttemptDetailByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<AttemptDetailDTO>> Handle(GetAttemptDetailById request, CancellationToken cancellationToken)
        {
            var AttemptRepository = _unitOfWork.Repository<Attempt>();
            var attempt =  AttemptRepository.Find(a => a.Id == request.AttemptId)
                .Select(a=>new AttemptDetailDTO()
                {
                    UserId = a.UserId,
                    QuizId = a.QuizId,
                    Status = a.Status,
                    StartTime = a.StartTime,
                    SubmittedAt = a.SubmittedAt,
                    TotalQuestions = a.Quiz.Questions.Count(),
                    Score = a.Score,
                    IsPassed = a.IsPassed,
                    QuestionsData = a.Quiz.Questions.Select( q => new AttemptQuestionDataDTO
                    {
                        QuestionText = q.Body,
                        AnswerText =  a.Answers.Select(an=>an.Option.Body).FirstOrDefault(),        
                        IsCorrect = a.Answers.Select(an => an.Option.IsCorrect).FirstOrDefault()
                    }).ToList()
                }).FirstOrDefault();

            return ApiResponse<AttemptDetailDTO>.Success(attempt);
        }
    }

}
