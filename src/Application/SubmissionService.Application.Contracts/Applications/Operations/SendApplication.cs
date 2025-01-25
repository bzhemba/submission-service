namespace SubmissionService.Application.Contracts.Applications.Operations;

public static class SendApplication
{
    public readonly record struct Request(long ApplicationId);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success : Result;

        public sealed record ApplicationNotFound : Result;
    }
}