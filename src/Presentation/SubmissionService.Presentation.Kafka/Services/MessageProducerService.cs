using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using SubmissionService.Presentation.Kafka.Services.Jobs;

namespace SubmissionService.Presentation.Kafka.Services;

public class MessageProducerService : BackgroundService
{
    private readonly ISchedulerFactory _schedulerFactory;

    public MessageProducerService(
        ILogger<MessageProducerService> logger,
        ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.Start(cancellationToken);

        IJobDetail job = JobBuilder.Create<DraftProcessingJob>()
            .WithIdentity("draftProcessingJob", "draftProcessorGroup")
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("draftProcessingTrigger", "draftProcessorGroup")
            .WithCronSchedule("0 0/5 * * * ?")
            .Build();
        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}