namespace ServiceBusProducerService.models;

public class EmailConfig
{
    public string? EmailClient { get; set; }
    public int EmailServerPort { get; set; }
    public string? EnableSSL { get; set; } 
    public string? Username { get; set; }
    public string? Password { get; set; }
    
}