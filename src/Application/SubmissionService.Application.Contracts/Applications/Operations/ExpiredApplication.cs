namespace SubmissionService.Application.Contracts.Applications.Operations;

public static class ExpiredApplication
{
    public readonly record struct Request();

    public abstract record Result
    {
        private Result() { }

        public sealed record Success : Result;

        public sealed record ApplicationsNotFound : Result;
    }
}