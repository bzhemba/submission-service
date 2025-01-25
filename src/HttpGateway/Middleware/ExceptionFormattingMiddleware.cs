using Grpc.Core;

namespace HttpGateway.Middleware;

public class ExceptionFormattingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Aborted)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { Message = ex.Status.Detail });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { Message = ex.Status.Detail });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { Message = ex.Status.Detail });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { Message = ex.Status.Detail });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { Message = ex.Status.Detail });
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { Message = ex.Status.Detail });
        }
        catch (Exception e)
        {
            string message = $"""
                              Exception occured while processing request, type = {e.GetType().Name}, message = {e.Message}";
                              """;
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { Message = message });
        }
    }
}