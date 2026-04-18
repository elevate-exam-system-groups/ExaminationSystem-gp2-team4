using Examination_System.Common.Exceptions.Errors;
using Examination_System.Common.Wrappers;
using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Examination_System.Common.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = ex switch
            {
                NotFoundException => (HttpStatusCode.NotFound, ex.Message),

                //ValidationException => (HttpStatusCode.BadRequest, ex.Message),

                UnAuthorizedException => (HttpStatusCode.Unauthorized, ex.Message),

                BadRequestException => (HttpStatusCode.BadRequest, ex.Message),

                _ => (HttpStatusCode.InternalServerError,
                     "An unexpected error occurred")
            };

            context.Response.StatusCode = (int)statusCode;
            var response = ApiResponse<string>.Failure(ErrorCode.InternalServerError);




            return context.Response.WriteAsync(
                JsonSerializer.Serialize(response,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    })
            );
        }
    }
}

