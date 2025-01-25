using ApplicationService;

namespace HttpGateway.Services;

public class ApplicationGrpcService
{
    private readonly ApplicationService.ApplicationService.ApplicationServiceClient _applicationServiceClient;

    public ApplicationGrpcService(
        ApplicationService.ApplicationService.ApplicationServiceClient applicationServiceClient)
    {
        _applicationServiceClient = applicationServiceClient;
    }

    public async Task<CreateApplicationResponse> CreateApplicationAsync(CreateApplicationRequest request, CancellationToken cancellationToken)
    {
        var grpcRequest = new CreateApplicationRequest()
        {
            UserId = request.UserId,
            Activity = request.Activity,
            Title = request.Title,
            Description = request.Description,
            Outline = request.Outline,
        };

        return await _applicationServiceClient.CreateApplicationAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task CancelApplicationAsync(long applicationId, CancellationToken cancellationToken)
    {
        var grpcRequest = new CancelApplicationRequest()
        {
            ApplicationId = applicationId,
        };

        await _applicationServiceClient.CancelApplicationAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task EditApplicationAsync(EditApplicationRequest request, CancellationToken cancellationToken)
    {
        var grpcRequest = new EditApplicationRequest()
        {
            ApplicationId = request.ApplicationId,
            Activity = request.Activity,
            Title = request.Title,
            Description = request.Description,
            Outline = request.Outline,
        };

        await _applicationServiceClient.EditApplicationAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task SendApplicationAsync(long applicationId, CancellationToken cancellationToken)
    {
        var grpcRequest = new SendApplicationRequest()
        {
            ApplicationId = applicationId,
        };

        await _applicationServiceClient.SendApplicationAsync(grpcRequest, cancellationToken: cancellationToken);
    }
}