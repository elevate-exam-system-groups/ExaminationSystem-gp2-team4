namespace Examination_System.Common.Exceptions.Errors
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string name, object key)
            : base($"{name} with ({key}) is not found")
        {

        }
    }
}
