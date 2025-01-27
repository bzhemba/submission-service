using SourceKit.Generators.Builder.Annotations;
using SubmissionService.Application.Models.Applications;

namespace SubmissionService.Application.Abstractions.Persistence.Commands;

[GenerateBuilder]
public partial record CreateApplicationCommand(
    long EventId,
    string UserEmail,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    ActivityType? Activity,
    string? Title,
    string? Description,
    string? Outline,
    DateTimeOffset CreatedAt,
    ApplicationState State = ApplicationState.Draft);