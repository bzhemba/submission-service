using Microsoft.Extensions.Logging;
using Quartz;

namespace SubmissionService.Presentation.Kafka;

public class NotificationJob : IJob
{
    private readonly ILogger<NotificationJob> _logger;

    public NotificationJob(ILogger<NotificationJob> logger)
    {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("🔔 Alarm! It's 7:00 AM. Wake up!");
        return Task.CompletedTask;
    }
}