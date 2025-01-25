using SourceKit.Generators.Builder.Annotations;
using SubmissionService.Application.Models.Applications;

namespace SubmissionService.Application.Abstractions.Persistence.Commands;

[GenerateBuilder]
public partial record CreateApplicationCommand(
    long UserId,
    ActivityType? Activity,
    string? Title,
    string? Description,
    string? Outline,
    DateTime CreatedAt,
    ApplicationState State = ApplicationState.Draft);