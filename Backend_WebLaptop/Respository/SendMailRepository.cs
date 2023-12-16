using Backend_WebLaptop.Configs;
using Backend_WebLaptop.IRespository;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.WebSockets;

namespace Backend_WebLaptop.Respository
{
    public class EmailSendRepository : IEmailSendRepository
    {
        readonly MailConfig _mailConfig;
        readonly SmtpClient SmtpClient;
        public EmailSendRepository(IOptions<MailConfig> mailconfig) {
            _mailConfig = mailconfig.Value;
            SmtpClient= new SmtpClient(_mailConfig.Host)
            {
                Port = _mailConfig.Port,
                EnableSsl = true,
                Credentials = new NetworkCredential(_mailConfig.Email, _mailConfig.Password)
            };
        }
        public async Task Sendmail(string to, string subject, string body)
        {
            //validate email
            var MailMessage = new MailMessage(_mailConfig.Email!, to, subject, body)
            {
                From = new MailAddress(_mailConfig.Email!, "DienTuViu Ecomerce Website")
                
            };

            try
            {
                await SmtpClient.SendMailAsync(MailMessage);
            }
            catch
            {
                throw new Exception("Dịch vụ không hoạt động");
            }
              
        }
    }
}
