namespace StudentMvcApp.Services
{
    public interface IEmailSender
    {
        void send(string to, string subject, string body);
    }
}
