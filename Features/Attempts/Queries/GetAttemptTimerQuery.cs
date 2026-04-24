using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Answers.Queries;
using Examination_System.Features.Attempts.DTOs;
using Examination_System.Features.QuestionOptions.Queries;
using ExaminationSystem.API.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using System.Net.NetworkInformation;

namespace Examination_System.Features.Attempts.Queries
{
    
    public record GetAttemptTimerQuery(Guid AttemptId) : IRequest<ApiResponse<GetAttemptTimerResponse>>;

    public class GetAttemptTimerQueryHandler : IRequestHandler<GetAttemptTimerQuery, ApiResponse<GetAttemptTimerResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        // private readonly ICurrentUserService _currentUserService;

        public GetAttemptTimerQueryHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<ApiResponse<GetAttemptTimerResponse>> Handle(GetAttemptTimerQuery request, CancellationToken cancellationToken)
        {
            var attemptRepository = _unitOfWork.Repository<Attempt>();

            var attempt = await attemptRepository.GetAll()
                .Where(x => x.Id == request.AttemptId)
                .Select(a => new
                {
                    AttemptId = a.Id,
                    Status= a.Status,
                    StartTime = a.StartTime,
                    DurationMinutes=a.Quiz.DurationMinutes,
                }).FirstOrDefaultAsync();
                

            if (attempt is null)
                return ApiResponse<GetAttemptTimerResponse>.Failure(ErrorCode.AttemptNotFound);
       
            if (attempt.Status == "Submitted" || attempt.Status == "TimedOut")
                return ApiResponse<GetAttemptTimerResponse>.Failure(ErrorCode.AttemptClosed);
   

            var utcNow = DateTime.UtcNow;
            var deadline = attempt.StartTime.AddMinutes(attempt.DurationMinutes);

            if (utcNow >= deadline)
            {
              
                var attemptAnswersData = await _mediator.Send(new GetAttemptAnswersQuery(attempt.AttemptId));

                decimal score = 0;

                if (attemptAnswersData.Count() > 0)
                {
                 
                    var CorrectOptions = await _mediator.Send(new GetAllCorrectOptions()); 

                    var CorrectOptionsIds = CorrectOptions.Select(o => o.Id).ToList();


                    var CorrectOptionsCount = attemptAnswersData
                        .Count(x => CorrectOptionsIds.Contains(x.OptionId));

                    score = CorrectOptionsCount;
                }
                var UpdateAttempt = await attemptRepository.GetByIdAsync(attempt.AttemptId);

                UpdateAttempt.Status = "TimedOut";
                UpdateAttempt.SubmittedAt = utcNow;
                UpdateAttempt.Score = (int)score;

                attemptRepository.Update(UpdateAttempt);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<GetAttemptTimerResponse>.Failure(ErrorCode.AttemptExpired);
            }

            var secondsRemaining = (int)Math.Floor((deadline - utcNow).TotalSeconds);
            secondsRemaining = Math.Max(0, secondsRemaining);

            var response = new GetAttemptTimerResponse
            {
                SecondsRemaining = secondsRemaining
            };

            return ApiResponse<GetAttemptTimerResponse>.Success(response);
        }
    }
}
