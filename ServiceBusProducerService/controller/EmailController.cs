using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using ServiceBusProducerService.models;
using ServiceBusProducerService.services;

namespace ServiceBusProducerService.controller;

public class EmailModel
{
    public string FromEmail { get; set; } = string.Empty;
    public string ToEmails { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

[ApiController]
[Route("api/email")]
public class EmailController : ControllerBase
{
    private readonly EmailService _emailService;
    private readonly MailkitEmailService _mailkitEmailService;

    public EmailController(EmailService emailService,MailkitEmailService mailkitEmailService)
    {
        _emailService = emailService;
        _mailkitEmailService = mailkitEmailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmailMessage([FromBody] EmailRequest emailRequest)
    {
        try
        {
            var response = await _emailService.SendEmailAsync(emailRequest);
            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [HttpPost("/mailkit/send")]
    public async Task<IActionResult> SendEmailWithMailKit([FromBody] EmailDto emailRequest)
    {
        try
        {
             _mailkitEmailService.SendEmail(emailRequest);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [HttpPost("SendEmail")]
    public ActionResult SendEmail(EmailModel emailData)
    {
        var message = new MailMessage()
        {
            From = new MailAddress("teacodelab@gmail.com"),
            Subject = emailData.Subject,
            IsBodyHtml = true,
            Body = $"""
                    <html>
                        <body>
                            <h3>{emailData.Body}</h3>
                        </body>
                    </html>
                    """
        };
        foreach(var toEmail in emailData.ToEmails.Split(";"))
        {
            message.To.Add(new MailAddress("tenkorangd5@gmail.com"));
        }
        // aefq gyty hubo mfcx
        using(SmtpClient smtpClient= new SmtpClient("smtp.gmail.com",587)
              {
                  Port = 587,
                  Credentials = new NetworkCredential("mailkitdev", "aefq gyty hubo mfcx"),
                  EnableSsl = true,
              })
        

        smtpClient.Send(message);

        return Ok("Email Sent!");
    }
    
}