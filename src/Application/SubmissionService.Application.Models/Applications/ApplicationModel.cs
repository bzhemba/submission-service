namespace SubmissionService.Application.Models.Applications;

public sealed record ApplicationModel(
    long Id,
    long UserId,
    ActivityType? Activity,
    string? Title,
    string? Description,
    string? Outline,
    DateTimeOffset CreatedAt,
    ApplicationState State);