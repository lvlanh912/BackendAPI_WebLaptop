namespace Backend_WebLaptop.IRespository
{
    public interface IEmailSendRepository
    {
        public Task Sendmail(string to, string subject, string content);
    }
}
