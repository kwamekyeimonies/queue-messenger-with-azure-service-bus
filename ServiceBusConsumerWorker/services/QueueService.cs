using System.Text;
using System.Text.Json;
using Microsoft.Azure.ServiceBus;
using ServiceBusConsumerWorker.models;

namespace ServiceBusConsumerWorker.services;

public class QueueService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<QueueService> _logger;
    private QueueClient? _queueClient;


    public QueueService(IConfiguration configuration, ILogger<QueueService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        var queueConfig = _configuration.GetSection("AzureServiceConfig").Get<AzureServiceBusConfig>();
        if (queueConfig == null)
        {
            _logger.LogError("Azure service Bus configuration not found");
            throw new ArgumentNullException(nameof(queueConfig), "Azure service Bus configuration not found");
        }
        _queueClient = new QueueClient(queueConfig.ServiceBusAddress, queueConfig.ServiceBusQueueName);
    }

    public async Task QueueMessageRegisterHandler(CancellationToken cancellationToken)
    {
        var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
        {
            MaxConcurrentCalls = 1,
            AutoComplete = false
        };

        _queueClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);

        _logger.LogInformation("Queue client registered and listening for messages.");

        try
        {
            // Wait until the cancellation token is triggered
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Cancellation requested. Closing queue client.");
        }
        finally
        {
            await _queueClient.CloseAsync();
            _logger.LogInformation("Queue client closed.");
        }
    }

    private async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
    {
        try
        {
            var jsonStringPayload = Encoding.UTF8.GetString(message.Body);
            var orderRequest = JsonSerializer.Deserialize<OrderRequest>(jsonStringPayload);

            if (orderRequest != null)
            {
                _logger.LogInformation($"Received Order: {JsonSerializer.Serialize(orderRequest)}");

                // TODO: Add your business logic to process the orderRequest here.

                // After successful processing, complete the message.
                await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
                _logger.LogInformation("Message processed and completed.");
            }
            else
            {
                _logger.LogWarning("Received a null OrderRequest. Abandoning message.");
                await _queueClient.AbandonAsync(message.SystemProperties.LockToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message. Abandoning message.");
            await _queueClient.AbandonAsync(message.SystemProperties.LockToken);
        }
    }

    private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
        _logger.LogError(exceptionReceivedEventArgs.Exception, "Message handler encountered an exception.");
        var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
        _logger.LogDebug($"Exception context for troubleshooting:");
        _logger.LogDebug($"- Endpoint: {context.Endpoint}");
        _logger.LogDebug($"- Entity Path: {context.EntityPath}");
        _logger.LogDebug($"- Executing Action: {context.Action}");
        return Task.CompletedTask;
    }
}