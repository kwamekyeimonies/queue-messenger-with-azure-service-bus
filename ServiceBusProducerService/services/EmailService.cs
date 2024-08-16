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
        var configurationKeys = _configuration.GetSection("EmailConfig").Get<EmailConfig>();
        if (configurationKeys == null)
        {
            _logger.LogError("No email credentials available.");
            throw new ArgumentNullException(nameof(configurationKeys), "No email credentials available.");
        }
        
        using (MailMessage mailMessage = new MailMessage())
        {
            mailMessage.From = new MailAddress("tenkorangd5@gmail.com");
            mailMessage.To.Add(new MailAddress("kwamekyeimonies@gmail.com"));
            mailMessage.Subject = emailRequest.Subject;
            mailMessage.Body = emailRequest.Body;

            using (var smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("tenkorangd5@gmail.com", "vbuukfczmmlnaotv");
                smtpClient.EnableSsl = true; 
                Console.WriteLine($"client credentials ${smtpClient.Credentials}");
                
                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    _logger.LogInformation("Email sent successfully.");
                    return new EmailResponse { Status = "successful", Message = "Email sent successfully." };
                }
                catch (SmtpException smtpEx)
                {
                    _logger.LogError(smtpEx, $"SMTP error occurred: {smtpEx.StatusCode} - {smtpEx.Message}");
                    return new EmailResponse { Status = "failed", Message = smtpEx.Message };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while sending email.");
                    return new EmailResponse { Status = "failed", Message = ex.Message };
                }
            }
        }
    }
}
