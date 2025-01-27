using ApplicationService;
using Grpc.Core;
using SubmissionService.Application.Contracts;
using SubmissionService.Application.Contracts.Applications.Operations;
using ActivityType = SubmissionService.Application.Models.Applications.ActivityType;

namespace SubmissionService.Presentation.Grpc.Controllers;

public class ApplicationController : ApplicationService.ApplicationService.ApplicationServiceBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    public override async Task<CreateApplicationResponse> CreateApplication(
        CreateApplicationRequest request,
        ServerCallContext context)
    {
        var applicationCommand = new CreateApplication.Request(
            request.EventId,
            request.UserEmail,
            request.StartedAt.ToDateTimeOffset(),
            request.FinishedAt.ToDateTimeOffset(),
            Convert(request.Activity),
            request.Title,
            request.Description,
            request.Outline);
        CreateApplication.Result response
            = await _applicationService.CreateAsync(applicationCommand, context.CancellationToken);
        return response switch
        {
            Application.Contracts.Applications.Operations.CreateApplication.Result.DraftAlreadyExists => throw new
                RpcException(
                    new Status(StatusCode.AlreadyExists, $"Draft already exists.")),
            Application.Contracts.Applications.Operations.CreateApplication.Result.MissingRequiredFields => throw new
                RpcException(
                    new Status(StatusCode.InvalidArgument, "Missing required fields.")),
            CreateApplication.Result.Success success =>
                new CreateApplicationResponse { ApplicationId = success.ApplicationId },
            _ => throw new RpcException(new Status(StatusCode.Cancelled, "Unable to create application.")),
        };
    }

    public override async Task<SendApplicationResponse> SendApplication(
        SendApplicationRequest request,
        ServerCallContext context)
    {
        var applicationCommand = new SendApplication.Request(request.ApplicationId);
        SendApplication.Result response
            = await _applicationService.SendAsync(applicationCommand, context.CancellationToken);
        return response switch
        {
            Application.Contracts.Applications.Operations.SendApplication.Result.ApplicationNotFound => throw new
                RpcException(
                    new Status(StatusCode.NotFound, $"Application not found.")),
            Application.Contracts.Applications.Operations.SendApplication.Result.MissingRequiredFields => throw new
                RpcException(
                    new Status(StatusCode.InvalidArgument, "Missing required fields.")),
            Application.Contracts.Applications.Operations.SendApplication.Result.Success =>
                new SendApplicationResponse(),
            _ => throw new RpcException(new Status(StatusCode.Cancelled, "Unable to send application.")),
        };
    }

    public override async Task<EditApplicationResponse> EditApplication(
        EditApplicationRequest request,
        ServerCallContext context)
    {
        var applicationCommand = new EditApplication.Request(
            request.ApplicationId,
            request.StartedAt.ToDateTimeOffset(),
            request.FinishedAt.ToDateTimeOffset(),
            Convert(request.Activity),
            request.Title,
            request.Description,
            request.Outline);
        EditApplication.Result response
            = await _applicationService.EditAsync(applicationCommand, context.CancellationToken);
        return response switch
        {
            Application.Contracts.Applications.Operations.EditApplication.Result.InvalidState => throw new
                RpcException(
                    new Status(StatusCode.Unavailable, $"Editing application available only in 'Draft' state.")),
            Application.Contracts.Applications.Operations.EditApplication.Result.MissingRequiredFields => throw new
                RpcException(
                    new Status(StatusCode.InvalidArgument, "Missing required fields.")),
            Application.Contracts.Applications.Operations.EditApplication.Result.Success =>
                new EditApplicationResponse(),
            Application.Contracts.Applications.Operations.EditApplication.Result.ApplicationNotFound => throw new
                RpcException(
                    new Status(StatusCode.NotFound, $"Application not found.")),
            _ => throw new RpcException(new Status(StatusCode.Cancelled, "Unable to edit application.")),
        };
    }

    public override async Task<CancelApplicationResponse> CancelApplication(
        CancelApplicationRequest request,
        ServerCallContext context)
    {
        var applicationCommand = new CancelApplication.Request(
            request.ApplicationId);
        CancelApplication.Result response
            = await _applicationService.CancelAsync(applicationCommand, context.CancellationToken);
        return response switch
        {
            Application.Contracts.Applications.Operations.CancelApplication.Result.Success =>
                new CancelApplicationResponse(),
            Application.Contracts.Applications.Operations.CancelApplication.Result.InvalidState => throw new
                RpcException(
                    new Status(StatusCode.Unavailable, $"Canceling application available only in 'Draft' state.")),
            Application.Contracts.Applications.Operations.CancelApplication.Result.ApplicationNotFound => throw new
                RpcException(
                    new Status(StatusCode.NotFound, $"Application not found.")),
            _ => throw new RpcException(new Status(StatusCode.Cancelled, "Unable to edit application.")),
        };
    }

    public override async Task<ApproveApplicationResponse> ApproveApplication(
        ApproveApplicationRequest request,
        ServerCallContext context)
    {
        var applicationCommand = new ApproveApplication.Request(
            request.ApplicationId);
        ApproveApplication.Result response
            = await _applicationService.ApproveAsync(applicationCommand, context.CancellationToken);
        return response switch
        {
            Application.Contracts.Applications.Operations.ApproveApplication.Result.Success =>
                new ApproveApplicationResponse(),
            Application.Contracts.Applications.Operations.ApproveApplication.Result.InvalidState => throw new
                RpcException(
                    new Status(StatusCode.Unavailable, $"Canceling application available only in 'Draft' state.")),
            Application.Contracts.Applications.Operations.ApproveApplication.Result.ApplicationNotFound => throw new
                RpcException(
                    new Status(StatusCode.NotFound, $"Application not found.")),
            _ => throw new RpcException(new Status(StatusCode.Cancelled, "Unable to edit application.")),
        };
    }

    private ActivityType? Convert(ApplicationService.ActivityType? type)
    {
        return type switch
        {
            ApplicationService.ActivityType.Unspecified => null,
            ApplicationService.ActivityType.Lecture => ActivityType.Lecture,
            ApplicationService.ActivityType.Workshop => ActivityType.Workshop,
            ApplicationService.ActivityType.Discussion => ActivityType.Discussion,
            _ => null,
        };
    }
}