namespace Examination_System.Features.Quizzes.DTOs;

public record CreateQuizResult(bool IsSuccess,int StatusCode,QuizResponse? Data
                            ,object? Errors,string? Message);