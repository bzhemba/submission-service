using SubmissionService.Application.Models.Applications;

namespace SubmissionService.Application.Contracts.Applications.Operations;

public static class CreateApplication
{
    public readonly record struct Request(
        long EventId,
        string UserEmail,
        DateTimeOffset StartedAt,
        DateTimeOffset FinishedAt,
        ActivityType? Activity,
        string? Title,
        string? Description,
        string Outline);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success(long ApplicationId) : Result;

        public sealed record DraftAlreadyExists : Result;

        public sealed record MissingRequiredFields : Result;
    }
}