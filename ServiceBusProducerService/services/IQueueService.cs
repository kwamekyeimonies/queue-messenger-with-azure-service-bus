namespace ServiceBusProducerService.services;

public interface IQueueService
{
    public Task SendQueueMessageAsync<T>(T serviceMessageBus, string queueName);
}