using SubmissionService.Application.Models.Applications;

namespace SubmissionService.Application.Contracts.Applications.Operations;

public static class EditApplication
{
    public readonly record struct Request(
        long ApplicationId,
        DateTimeOffset? StartedAt,
        DateTimeOffset? FinishedAt,
        ActivityType? Activity,
        string? Title,
        string? Description,
        string? Outline);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success : Result;

        public sealed record ApplicationNotFound : Result;

        public sealed record InvalidState(ApplicationState State) : Result;

        public sealed record MissingRequiredFields : Result;
    }
}