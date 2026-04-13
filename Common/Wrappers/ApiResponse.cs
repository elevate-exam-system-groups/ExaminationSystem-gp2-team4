using Microsoft.AspNetCore.Connections.Features;

namespace Examination_System.Common.Wrappers
{
    public record ApiResponse<TResult>(TResult Data,bool IsSuccess,ErrorCode ErrorCode)
    {
        public static ApiResponse<TResult> Success(TResult data) => new ApiResponse<TResult>(data,true,ErrorCode.None);
        public static ApiResponse<TResult> Failure(ErrorCode errorCode) => new ApiResponse<TResult>(default(TResult),false,errorCode);

    }
}
