using Itmo.Dev.Platform.BackgroundTasks.Tasks.Metadata;

namespace SubmissionService.Application;
public sealed record SimpleTaskMetadata(string[] Values) : IBackgroundTaskMetadata;