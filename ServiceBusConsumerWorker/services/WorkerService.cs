namespace ServiceBusConsumerWorker.services;

public class WorkerService : BackgroundService
{
    private readonly QueueService _queueService;
    private readonly ILogger<WorkerService> _logger;

    public WorkerService(QueueService queueService, ILogger<WorkerService> logger)
    {
        _queueService = queueService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("QueueBackgroundService is starting.");

        // Start listening to the queue
        await _queueService.QueueMessageRegisterHandler(stoppingToken);

        _logger.LogInformation("QueueBackgroundService is stopping.");
    }
}