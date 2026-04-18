namespace Examination_System.Common.Models.Identity
{
    public class EmailSettings
    {
        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
