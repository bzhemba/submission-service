using Microsoft.AspNetCore.Builder;
using SubmissionService.Presentation.Grpc.Controllers;

namespace SubmissionService.Presentation.Grpc.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UsePresentationGrpc(this IApplicationBuilder builder)
    {
        builder.UseEndpoints(routeBuilder =>
        {
            routeBuilder.MapGrpcService<ApplicationController>();
            routeBuilder.MapGrpcReflectionService();
        });

        return builder;
    }
}