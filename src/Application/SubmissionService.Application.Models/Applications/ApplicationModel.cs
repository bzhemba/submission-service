namespace SubmissionService.Application.Models.Applications;

public sealed record ApplicationModel(
    long Id,
    long EventId,
    string UserEmail,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    ActivityType? Activity,
    string? Title,
    string? Description,
    string? Outline,
    DateTimeOffset CreatedAt,
    ApplicationState State);