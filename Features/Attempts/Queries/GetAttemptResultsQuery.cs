using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Common.Wrappers;
using Examination_System.Features.Attempts.DTOs;
using Examination_System.Features.Attempts.Queries.GetAttemptResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Features.Attempts.Queries.GetAttemptResults;

public record GetAttemptResultsQuery(Guid AttemptId) : IRequest<ApiResponse<AttemptResultsResponse>>;
public class GetAttemptResultsQueryHandler(
    IUnitOfWork unitOfWork,
    IHttpContextAccessor httpContextAccessor,
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<GetAttemptResultsQuery, ApiResponse<AttemptResultsResponse>>
{
    public async Task<ApiResponse<AttemptResultsResponse>> Handle(
        GetAttemptResultsQuery request,
        CancellationToken ct)
    {
        // 1. Get current user
        var email = httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(email))
            return ApiResponse<AttemptResultsResponse>.Failure(ErrorCode.Unauthorized);

        var currentUser = await userManager.FindByEmailAsync(email);
        if (currentUser is null)
            return ApiResponse<AttemptResultsResponse>.Failure(ErrorCode.UserNotFound);

        var roles = await userManager.GetRolesAsync(currentUser);
        var isAdmin = roles.Contains("Admin");

        // 2. Get attempt with needed data
        var attempt = await unitOfWork.Repository<Attempt>()
            .Find(a => a.Id == request.AttemptId)
            .Include(a => a.Answers)
                .ThenInclude(a => a.Question)
            .Include(a => a.Answers)
                .ThenInclude(a => a.Option)
            .FirstOrDefaultAsync(ct);

        if (attempt is null)
            return ApiResponse<AttemptResultsResponse>.Failure(ErrorCode.NotFound);

        // 3. Authorization
        var isOwner = attempt.UserId == currentUser.Id;
        if (!isOwner && !isAdmin)
            return ApiResponse<AttemptResultsResponse>.Failure(ErrorCode.Forbidden);

        // 4. Status validation
        if (attempt.Status == "in_progress")
            return ApiResponse<AttemptResultsResponse>.Failure(ErrorCode.Forbidden);

        // 5. Get correct options
        var questionIds = attempt.Answers.Select(a => a.QuestionId).ToList();

        var correctOptions = await unitOfWork.Repository<Option>()
            .Find(o => questionIds.Contains(o.QuestionId) && o.IsCorrect)
            .ToListAsync(ct);

        var correctOptionMap = correctOptions.ToDictionary(o => o.QuestionId, o => o);

        // 6. Mapping
        var perQuestion = attempt.Answers.Select(a =>
        {
            correctOptionMap.TryGetValue(a.QuestionId, out var correctOption);

            return new PerQuestionDto
            {
                QuestionId = a.QuestionId,
                StudentAnswer = a.Option?.Body,          // إجابة الطالب
                CorrectAnswer = correctOption?.Body,
                IsCorrect = a.IsCorrect,
                Explanation = a.Question.Explanation
            };
        }).ToList();

        var total = perQuestion.Count;
        var correctCount = perQuestion.Count(q => q.IsCorrect);

        var result = new AttemptResultsResponse
        {
            Score = attempt.Score,
            Passed = attempt.IsPassed,
            TotalQuestions = total,
            CorrectCount = correctCount,
            PerQuestion = perQuestion
        };

        return ApiResponse<AttemptResultsResponse>.Success(result);
    }
}