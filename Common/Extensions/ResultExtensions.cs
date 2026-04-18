using Examination_System.Common.Wrappers;
using Microsoft.AspNetCore.Mvc;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this ApiResponse<T> response)
    {
        if (response.IsSuccess)
            return new OkObjectResult(response);

        return response.ErrorCode switch
        {
            // Auth
            ErrorCode.InvalidCredentials => new UnauthorizedObjectResult(response),
            ErrorCode.UnauthorizedAccess => new UnauthorizedObjectResult(response),
            ErrorCode.EmailNotConfirmed => new UnauthorizedObjectResult(response),

            ErrorCode.UserNotFound => new NotFoundObjectResult(response),
            ErrorCode.NotFound => new NotFoundObjectResult(response),

            ErrorCode.ValidationError => new BadRequestObjectResult(response),
            ErrorCode.BadRequest => new BadRequestObjectResult(response),

            ErrorCode.Conflict => new ConflictObjectResult(response),

            ErrorCode.TooManyRequests => new ObjectResult(response)
            {
                StatusCode = 429
            },

            _ => new ObjectResult(response)
            {
                StatusCode = 500
            }
        };
    }
}