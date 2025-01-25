using SubmissionService.Application.Abstractions.Persistence.Repositories;

namespace SubmissionService.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    IApplicationsRepository Applications { get; }
}