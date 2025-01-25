using SubmissionService.Application.Abstractions.Persistence;
using SubmissionService.Application.Abstractions.Persistence.Repositories;

namespace SubmissionService.Infrastructure.Persistence;

public class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(IApplicationsRepository applications)
    {
        Applications = applications;
    }

    public IApplicationsRepository Applications { get; }
}