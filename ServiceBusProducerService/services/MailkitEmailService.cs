using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using ServiceBusProducerService.models;

namespace ServiceBusProducerService.services;

public class MailkitEmailService
{
    private readonly IConfiguration _configuration;

    public MailkitEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendEmail(EmailDto request)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailUsername").Value));
        email.To.Add(MailboxAddress.Parse(request.To));
        email.Subject = request.Subject;
        email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

        using var smtp = new SmtpClient();
        smtp.Connect(_configuration.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
        smtp.Authenticate(_configuration.GetSection("EmailUsername").Value, _configuration.GetSection("EmailPassword").Value);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}