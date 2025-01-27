using SourceKit.Generators.Builder.Annotations;
using SubmissionService.Application.Models.Applications;

namespace SubmissionService.Application.Abstractions.Persistence.Commands;

[GenerateBuilder]
public partial record UpdateApplicationCommand(
    long Id,
    DateTimeOffset? StartedAt,
    DateTimeOffset? FinishedAt,
    ActivityType? Activity,
    string? Title,
    string? Description,
    string? Outline);