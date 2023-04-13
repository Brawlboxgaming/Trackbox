using System.Net;
using System.Net.Mail;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("noreply.trackbox@gmail.com", "nkhzbcregqiwwolu")
        };

        var mail = new MailMessage(from: "noreply.trackbox@gmail.com", to: email, subject, message);
        mail.IsBodyHtml = true;

        return client.SendMailAsync(mail);
    }
}