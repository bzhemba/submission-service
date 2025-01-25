using SubmissionService.Application.Abstractions.Persistence;
using SubmissionService.Application.Abstractions.Persistence.Commands;
using SubmissionService.Application.Abstractions.Persistence.Queries;
using SubmissionService.Application.Contracts;
using SubmissionService.Application.Contracts.Applications.Operations;
using SubmissionService.Application.Models.Applications;

namespace SubmissionService.Application.Application;

internal class ApplicationService : IApplicationService
{
    private readonly IPersistenceContext _context;

    public ApplicationService(
        IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<CreateApplication.Result> CreateAsync(
        CreateApplication.Request request,
        CancellationToken cancellationToken)
    {
        ApplicationModel? application = await _context.Applications
            .GetDraftByUserIdAsync(request.UserId, cancellationToken);

        if (application is not null)
            return new CreateApplication.Result.DraftAlreadyExists();

        if (request.Activity == null &&
            string.IsNullOrWhiteSpace(request.Title) &&
            string.IsNullOrWhiteSpace(request.Description) &&
            string.IsNullOrWhiteSpace(request.Outline))
        {
            return new CreateApplication.Result.MissingRequiredFields();
        }

        var command = new CreateApplicationCommand(
            request.UserId,
            request.Activity,
            request.Title,
            request.Description,
            request.Outline,
            DateTime.UtcNow);

        long applicationId = await _context.Applications.CreateAsync(command, cancellationToken);

        return new CreateApplication.Result.Success(applicationId);
    }

    public async Task<SendApplication.Result> SendAsync(
        SendApplication.Request request,
        CancellationToken cancellationToken)
    {
        var applicationQuery = new GetApplicationByIdQuery(request.ApplicationId);

        ApplicationModel? application = await _context.Applications
            .GetByIdAsync(applicationQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (application is null)
            return new SendApplication.Result.ApplicationNotFound();

        await _context.Applications.ChangeStatusAsync(
            request.ApplicationId,
            ApplicationState.PendingApproval,
            cancellationToken);

        return new SendApplication.Result.Success();
    }

    public async Task<CancelApplication.Result> CancelAsync(
        CancelApplication.Request request,
        CancellationToken cancellationToken)
    {
        var applicationQuery = new GetApplicationByIdQuery(request.ApplicationId);

        ApplicationModel? application = await _context.Applications
            .GetByIdAsync(applicationQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (application is null)
            return new CancelApplication.Result.ApplicationNotFound();

        if (application.State is not ApplicationState.Draft)
            return new CancelApplication.Result.InvalidState(application.State);

        await _context.Applications.ChangeStatusAsync(
            request.ApplicationId,
            ApplicationState.Cancelled,
            cancellationToken);

        return new CancelApplication.Result.Success();
    }

    public async Task<ApproveApplication.Result> ApproveAsync(
        ApproveApplication.Request request,
        CancellationToken cancellationToken)
    {
        var applicationQuery = new GetApplicationByIdQuery(request.ApplicationId);

        Models.Applications.ApplicationModel? application = await _context.Applications
            .GetByIdAsync(applicationQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (application is null)
            return new ApproveApplication.Result.ApplicationNotFound();

        await _context.Applications.ChangeStatusAsync(
            request.ApplicationId,
            ApplicationState.Cancelled,
            cancellationToken);

        return new ApproveApplication.Result.Success();
    }

    public async Task<IReadOnlyCollection<ApplicationModel>> GetExpiredDraftsAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Applications.GetExpiredDraftsAsync(cancellationToken).ToArrayAsync(cancellationToken);
    }

    public async Task<EditApplication.Result> EditAsync(
        EditApplication.Request request,
        CancellationToken cancellationToken)
    {
        var applicationQuery = new GetApplicationByIdQuery(request.ApplicationId);

        ApplicationModel? application = await _context.Applications
            .GetByIdAsync(applicationQuery, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (application is null)
            return new EditApplication.Result.ApplicationNotFound();

        if (application.State is not ApplicationState.Draft)
            return new EditApplication.Result.InvalidState(application.State);

        if (request.Activity == null &&
            string.IsNullOrWhiteSpace(request.Title) &&
            string.IsNullOrWhiteSpace(request.Description) &&
            string.IsNullOrWhiteSpace(request.Outline))
        {
            return new EditApplication.Result.MissingRequiredFields();
        }

        var command = UpdateApplicationCommand.Build(builder => builder
            .WithId(request.ApplicationId)
            .WithActivity(request.Activity ?? application.Activity)
            .WithTitle(request.Title ?? application.Title)
            .WithDescription(request.Description ?? application.Description)
            .WithOutline(request.Outline ?? application.Outline));

        await _context.Applications.UpdateAsync(
            command,
            cancellationToken);

        return new EditApplication.Result.Success();
    }
}