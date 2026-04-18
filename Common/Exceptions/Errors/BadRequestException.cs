namespace Examination_System.Common.Exceptions.Errors
{
    public class BadRequestException : ApplicationException
    {
        public BadRequestException(string? message = null)
            : base(message)
        {

        }
    }
}
