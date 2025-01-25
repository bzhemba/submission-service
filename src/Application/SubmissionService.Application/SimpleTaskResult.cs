using Itmo.Dev.Platform.BackgroundTasks.Tasks.Results;

namespace SubmissionService.Application;

public sealed record SimpleTaskResult(string Result) : IBackgroundTaskResult;