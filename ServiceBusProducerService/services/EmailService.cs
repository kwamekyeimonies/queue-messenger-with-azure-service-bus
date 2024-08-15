using System.Net;
using System.Net.Mail;
using ServiceBusProducerService.models;

namespace ServiceBusProducerService.services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<EmailResponse> SendEmailAsync(EmailRequest emailRequest)
    {
        using (MailMessage mailMessage = new MailMessage())
        {
            mailMessage.From = new MailAddress(emailRequest.Sender);
            // Loop through the list of recipients and add each to the MailMessage
            foreach (var recipient in emailRequest.Receipient)
            {
                mailMessage.To.Add(new MailAddress(recipient));
            }
            mailMessage.Subject = emailRequest.Subject;
            mailMessage.Body = emailRequest.Body;

            var configurationKeys = _configuration.GetSection("EmailConfig").Get<EmailConfig>();
            if (configurationKeys == null)
            {
                _logger.LogError("no email credentials available");
                throw new ArgumentNullException(nameof(configurationKeys), "no email credentials available");
            }

            using (SmtpClient smtpClient = new SmtpClient(configurationKeys.EmailClient))
            {
                smtpClient.Host = configurationKeys.EmailClient;
                smtpClient.Port = configurationKeys.EmailServerPort; 
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(configurationKeys.Username, configurationKeys.Password);
                smtpClient.EnableSsl = configurationKeys.EnableSSL?.ToLower() == "true";

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    Console.WriteLine("Email Sent Successfully.");
                    return new EmailResponse { Status = "successful", Message = "Email sent successfully." };
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return new EmailResponse {  Status = "failed", Message = ex.Message };
                }
            }
        }
    }

}