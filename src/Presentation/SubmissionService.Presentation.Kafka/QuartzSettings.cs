namespace SubmissionService.Presentation.Kafka;

public class QuartzSettings
{
    public string JobGroup { get; set; } = string.Empty;

    public string JobName { get; set; } = string.Empty;

    public string CronSchedule { get; set; } = string.Empty;
}