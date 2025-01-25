using SourceKit.Generators.Builder.Annotations;

namespace SubmissionService.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public partial record GetExpiredDraftsQuery([RequiredValue] int PageSize, long Cursor);