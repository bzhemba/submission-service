using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SubmissionService.Presentation.Grpc;

internal sealed class ExceptionInterceptor : Interceptor
{
    private readonly ILogger<ExceptionInterceptor> _logger;

    public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException rpcException)
        {
            throw rpcException.StatusCode switch
            {
                StatusCode.InvalidArgument => new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.NotFound => new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.Unavailable => new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.Aborted => new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.OK => throw new RpcException(new Status(StatusCode.OK, "Request was successful.")),
                StatusCode.Cancelled => throw new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.Unknown => throw new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.DeadlineExceeded => throw new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.AlreadyExists => throw new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.PermissionDenied => throw new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.Unauthenticated => throw new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.ResourceExhausted => throw new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.FailedPrecondition => throw new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.OutOfRange => throw new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.Unimplemented => throw new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.Internal => throw new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                StatusCode.DataLoss => throw new RpcException(new Status(rpcException.StatusCode, rpcException.Status.Detail)),
                _ => rpcException,

            };
        }
        catch (Exception exception)
        {
            HttpContext httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            _logger.LogError(exception, "Unhandled exception");

            throw new RpcException(new Status(StatusCode.Internal, "Internal server error. See logs for details."));
        }
    }
}