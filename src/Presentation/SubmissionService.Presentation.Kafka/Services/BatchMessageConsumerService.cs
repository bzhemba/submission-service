using Google.Protobuf.WellKnownTypes;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.Kafka.Producer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SubmissionService.Application.Contracts;
using SubmissionService.Kafka.Contracts;

namespace SubmissionService.Presentation.Kafka.Services;

public class BatchMessageConsumerService : BackgroundService
{
    private readonly IApplicationService _applicationService;
    private readonly IKafkaMessageProducer<DraftNotificationKey, DraftNotificationValue> _producer;
    private readonly ILogger<BatchMessageConsumerService> _logger;

    public BatchMessageConsumerService(
        IKafkaMessageProducer<DraftNotificationKey, DraftNotificationValue> producer,
        IApplicationService applicationService,
        ILogger<BatchMessageConsumerService> logger)
    {
        _producer = producer;
        _applicationService = applicationService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                IReadOnlyCollection<Application.Models.Applications.ApplicationModel> expiredReports =
                    await _applicationService.GetExpiredDraftsAsync(stoppingToken);

                foreach (Application.Models.Applications.ApplicationModel report in expiredReports)
                {
                    var key = new DraftNotificationKey { ApplicationId = report.Id };

                    var value = new DraftNotificationValue
                    {
                        ApplicationId = report.Id,
                        UserId = report.UserId,
                        CreatedAt = report.CreatedAt.ToTimestamp(),
                    };
                    var message = new KafkaProducerMessage<DraftNotificationKey, DraftNotificationValue>(key, value);
                    _logger.LogInformation("PRODUCING MESSAGE");

                    await _producer.ProduceAsync(message, stoppingToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while processing reports");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}