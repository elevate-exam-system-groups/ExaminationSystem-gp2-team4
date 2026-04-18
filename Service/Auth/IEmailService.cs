namespace Examination_System.Common.Service.Auth
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
