using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace courses.Infrastructure.SenderMessages;

public interface ISenderMessages
{
    void SendMessage(string mail, string subject, string body);
}

public class SenderMessages: ISenderMessages
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromEmail;

    public SenderMessages(IOptions<SmtpSettings> smtpSettings)
    {
        var settings = smtpSettings.Value;

        _smtpClient = new SmtpClient(settings.Server, settings.Port)
        {
            Credentials = new NetworkCredential(
                settings.Username, 
                settings.Password),
            EnableSsl = settings.EnableSsl
        };
        
        _fromEmail = settings.FromEmail;
    }

    public void SendMessage(string mail, string subject, string body)
    {
        using (var mailMessage = new MailMessage())
        {
            mailMessage.From = new MailAddress(_fromEmail);
            mailMessage.To.Add(mail);
            mailMessage.Subject = subject;
            mailMessage.Body = body;

            try
            {
                _smtpClient.Send(mailMessage);
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Exception: {smtpEx.Message}");
                Console.WriteLine($"Inner Exception: {smtpEx.InnerException?.Message}");
                Console.WriteLine($"Status Code: {smtpEx.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
            }


        }
    }

}