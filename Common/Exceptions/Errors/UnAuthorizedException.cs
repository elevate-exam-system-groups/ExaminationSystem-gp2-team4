namespace Examination_System.Common.Exceptions.Errors
{
    public class UnAuthorizedException : ApplicationException
    {
        public UnAuthorizedException(string message)
            : base(message)
        {

        }
    }
}
