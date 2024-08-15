using Microsoft.AspNetCore.Mvc;
using ServiceBusProducerService.models;
using ServiceBusProducerService.services;

namespace ServiceBusProducerService.controller;

[ApiController]
[Route("api/email")]
public class EmailController : ControllerBase
{
    private readonly EmailService _emailService;

    public EmailController(EmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("/send")]
    public async Task<IActionResult> SendEmailMessage([FromBody] EmailRequest emailRequest)
    {
        try
        {
            var response = _emailService.SendEmailAsync(emailRequest);
            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
}