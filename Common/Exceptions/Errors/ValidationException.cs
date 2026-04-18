namespace Examination_System.Common.Exceptions.Errors
{
    public class ValidationException : BadRequestException
    {
        public required IEnumerable<string> Errors { get; set; }

        public ValidationException(string message = "Bad Request")
            : base(message)
        {

        }
    }
}
