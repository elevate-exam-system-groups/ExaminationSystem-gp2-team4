using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.StudentDashboard.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Features.StudentDashboard
{
    public record GetStudentDashboardQuery(Guid UserId) : IRequest<ApiResponse<StudentDashboardResponse>>;

    public class GetStudentDashboardQueryHandler : IRequestHandler<GetStudentDashboardQuery, ApiResponse<StudentDashboardResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetStudentDashboardQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<StudentDashboardResponse>> Handle(GetStudentDashboardQuery request, CancellationToken cancellationToken)
        {
            var attemptRepo = _unitOfWork.Repository<Attempt>();
            var quizRepo = _unitOfWork.Repository<Quiz>();
            var diplomaRepo = _unitOfWork.Repository<Diploma>();

            var userAttempts = await attemptRepo.GetAll()
                .Where(a => a.UserId == request.UserId)
                .Include(a => a.Quiz)
                .ThenInclude(q => q!.Diploma)
                .ToListAsync(cancellationToken);

            var enrolledDiplomaIds = userAttempts
                .Where(a => a.Quiz != null && a.Quiz.DiplomaId != Guid.Empty)
                .Select(a => a.Quiz!.DiplomaId)
                .Distinct()
                .ToList();

            var enrolledDiplomas = new List<EnrolledDiplomaDto>();

            foreach (var diplomaId in enrolledDiplomaIds)
            {
                var diploma = await diplomaRepo.GetByIdAsync(diplomaId);
                if (diploma == null) continue;

                var quizzesInDiploma = await quizRepo.GetAll()
                    .Where(q => q.DiplomaId == diplomaId)
                    .ToListAsync(cancellationToken);

                var quizIds = quizzesInDiploma.Select(q => q.Id).ToList();
                var completedQuizCount = userAttempts
                    .Where(a => quizIds.Contains(a.QuizId) && a.Status == "Completed")
                    .Select(a => a.QuizId)
                    .Distinct()
                    .Count();

                enrolledDiplomas.Add(new EnrolledDiplomaDto
                {
                    Id = diploma.Id,
                    Title = diploma.Title,
                    Description = diploma.Description,
                    CoverImageUrl = diploma.ImageUrl,
                    QuizCount = quizzesInDiploma.Count,
                    CompletedQuizCount = completedQuizCount
                });
            }

            var recentQuizAttempts = userAttempts
                .OrderByDescending(a => a.StartTime)
                .Take(10)
                .Select(a => new RecentQuizAttemptDto
                {
                    AttemptId = a.Id,
                    QuizId = a.QuizId,
                    QuizTitle = a.Quiz?.Title ?? string.Empty,
                    DiplomaTitle = a.Quiz?.Diploma?.Title ?? string.Empty,
                    Score = a.Score,
                    TotalQuestions = a.TotalQuestions,
                    Status = a.Status,
                    IsPassed = a.IsPassed,
                    StartedAt = a.StartTime,
                    SubmittedAt = a.SubmittedAt
                })
                .ToList();

            var completedAttempts = userAttempts.Where(a => a.Status != "InProgress").ToList();
            var totalQuizzesTaken = completedAttempts.Count;

            double avgScore = 0;
            double passRate = 0;

            if (totalQuizzesTaken > 0)
            {
                var attemptsWithQuestions = completedAttempts.Where(a => a.TotalQuestions > 0).ToList();
                if (attemptsWithQuestions.Any())
                {
                    avgScore = attemptsWithQuestions.Average(a => (double)a.Score / a.TotalQuestions * 100);
                }

                var passedCount = completedAttempts.Count(a => a.IsPassed);
                passRate = (double)passedCount / totalQuizzesTaken * 100;
            }

            var overallStats = new OverallStatsDto
            {
                TotalQuizzesTaken = totalQuizzesTaken,
                AvgScore = Math.Round(avgScore, 2),
                PassRate = Math.Round(passRate, 2)
            };

            var response = new StudentDashboardResponse
            {
                EnrolledDiplomas = enrolledDiplomas,
                RecentQuizAttempts = recentQuizAttempts,
                OverallStats = overallStats
            };

            return ApiResponse<StudentDashboardResponse>.Success(response);
        }
    }
}