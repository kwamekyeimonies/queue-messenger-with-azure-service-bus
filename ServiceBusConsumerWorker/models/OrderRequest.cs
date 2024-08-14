namespace ServiceBusConsumerWorker.models;

public class OrderRequest
{
    public Guid? OrderId { get; set; }
    public string? OrderName { get; set; }
    public Guid? UserId { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserEmail { get; set; }
    public List<string>? Products { get; set; }
}