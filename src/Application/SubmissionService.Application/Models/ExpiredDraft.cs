using SubmissionService.Application.Models.Applications;

namespace SubmissionService.Application.Models;

public sealed record ExpiredDraft(
    long Id,
    long UserId,
    ActivityType? Activity,
    string? Title,
    string? Description,
    string? Outline,
    DateTimeOffset CreatedAt,
    ApplicationState State);