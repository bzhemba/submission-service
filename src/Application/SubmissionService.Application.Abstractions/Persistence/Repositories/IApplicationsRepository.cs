using SubmissionService.Application.Abstractions.Persistence.Commands;
using SubmissionService.Application.Abstractions.Persistence.Queries;
using SubmissionService.Application.Models.Applications;

namespace SubmissionService.Application.Abstractions.Persistence.Repositories;

public interface IApplicationsRepository
{
    IAsyncEnumerable<ApplicationModel> GetByIdAsync(
        GetApplicationByIdQuery query,
        CancellationToken cancellationToken);

    Task<ApplicationModel?> GetDraftByUserIdAsync(long userId, CancellationToken cancellationToken);

    IAsyncEnumerable<ApplicationModel> GetExpiredDraftsAsync(CancellationToken cancellationToken);

    Task UpdateAsync(UpdateApplicationCommand application, CancellationToken cancellationToken);

    Task<long> CreateAsync(CreateApplicationCommand application, CancellationToken cancellationToken);

    Task ChangeStatusAsync(long applicationId, ApplicationState newState, CancellationToken cancellationToken);
}