using System.Text;
using System.Text.Json;
using Microsoft.Azure.ServiceBus;
using ServiceBusProducerService.models;

namespace ServiceBusProducerService.services;

public class QueueService : IQueueService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<QueueService> _logger;

    public QueueService(IConfiguration configuration,ILogger<QueueService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendQueueMessageAsync<T>(T serviceMessageBus, string queueName)
    {
        try
        {
            var queueConfig = _configuration.GetSection("AzureServiceConfig").Get<AzureServiceBusConfig>();
            if (queueConfig == null)
            {
                _logger.LogError("Azure service Bus configuration not found");
                throw new ArgumentNullException(nameof(queueConfig), "Azure service Bus configuration not found");
            }
            var queueClient = new QueueClient(queueConfig.ServiceBusAddress, queueConfig.ServiceBusQueueName);
            string messageBody = JsonSerializer.Serialize(serviceMessageBus);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await queueClient.SendAsync(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

