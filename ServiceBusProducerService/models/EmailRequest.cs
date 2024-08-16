namespace ServiceBusProducerService.models;

public class EmailRequest
{
    public List<string> Receipient { get; set; }
    public string? Body { get; set; }
    public string? Subject { get; set; }
    public string? Sender { get; set; }
}
// vsns jqou ckce jslh