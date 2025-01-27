using Google.Protobuf.WellKnownTypes;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.Kafka.Producer;
using Microsoft.Extensions.Logging;
using Quartz;
using SubmissionService.Application.Contracts;
using SubmissionService.Kafka.Contracts;

namespace SubmissionService.Presentation.Kafka.Services.Jobs;

public class DraftProcessingJob : IJob
{
    private readonly ILogger<DraftProcessingJob> _logger;
    private readonly IApplicationService _applicationService;
    private readonly IKafkaMessageProducer<DraftNotificationKey, DraftNotificationValue> _producer;

    public DraftProcessingJob(
        ILogger<DraftProcessingJob> logger,
        IApplicationService applicationService,
        IKafkaMessageProducer<DraftNotificationKey, DraftNotificationValue> producer)
    {
        _logger = logger;
        _applicationService = applicationService;
        _producer = producer;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("ReportProcessingJob Started");
            IReadOnlyCollection<Application.Models.Applications.ApplicationModel> expiredReports =
                await _applicationService.GetExpiredDraftsAsync(context.CancellationToken);

            foreach (Application.Models.Applications.ApplicationModel report in expiredReports)
            {
                var key = new DraftNotificationKey { ApplicationId = report.Id };

                var value = new DraftNotificationValue
                {
                    ApplicationId = report.Id,
                    UserEmail = report.UserEmail,
                    CreatedAt = report.CreatedAt.ToTimestamp(),
                };
                var message = new KafkaProducerMessage<DraftNotificationKey, DraftNotificationValue>(key, value);
                _logger.LogInformation("PRODUCING MESSAGE");

                await _producer.ProduceAsync(message, context.CancellationToken);
            }

            _logger.LogInformation("ReportProcessingJob Finished");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while processing reports in job");
        }
    }
}