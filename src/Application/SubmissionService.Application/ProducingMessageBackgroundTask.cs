using Itmo.Dev.Platform.BackgroundTasks.Tasks;
using Itmo.Dev.Platform.BackgroundTasks.Tasks.Errors;
using Itmo.Dev.Platform.BackgroundTasks.Tasks.ExecutionMetadata;

namespace SubmissionService.Application;

public class ProducingMessageBackgroundTask : IBackgroundTask<
    SimpleTaskMetadata,
    EmptyExecutionMetadata,
    SimpleTaskResult,
    EmptyError>
{
    public static string Name => nameof(ProducingMessageBackgroundTask);

    public Task<BackgroundTaskExecutionResult<SimpleTaskResult, EmptyError>> ExecuteAsync(
        BackgroundTaskExecutionContext<SimpleTaskMetadata, EmptyExecutionMetadata> executionContext,
        CancellationToken cancellationToken)
    {
        string result = string.Join(", ", executionContext.Metadata.Values);

        return Task.FromResult<BackgroundTaskExecutionResult<SimpleTaskResult, EmptyError>>(
            BackgroundTaskExecutionResult.Success.WithResult(new SimpleTaskResult(result)).ForEmptyError());
    }
}