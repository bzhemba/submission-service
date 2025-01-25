using SubmissionService.Application.Contracts.Applications.Operations;

namespace SubmissionService.Application.Contracts;

public interface IApplicationService
{
    Task<EditApplication.Result> EditAsync(
        EditApplication.Request request,
        CancellationToken cancellationToken);

    Task<SendApplication.Result> SendAsync(
        SendApplication.Request request,
        CancellationToken cancellationToken);

    Task<CreateApplication.Result> CreateAsync(
        CreateApplication.Request request,
        CancellationToken cancellationToken);

    Task<CancelApplication.Result> CancelAsync(
        CancelApplication.Request request,
        CancellationToken cancellationToken);

    Task<ApproveApplication.Result> ApproveAsync(
        ApproveApplication.Request request,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Models.Applications.ApplicationModel>> GetExpiredDraftsAsync(
        CancellationToken cancellationToken);
}