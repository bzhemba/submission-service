using HttpGateway.Services;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace HttpGateway;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureGrpcApplicationServiceClient(this IServiceCollection services)
    {
        services.AddOptions<GrpcOptions>().BindConfiguration("GrpcOptions");

        services.AddGrpcClient<ApplicationService.ApplicationService.ApplicationServiceClient>((serviceProvider, o) =>
        {
            GrpcOptions settings = serviceProvider.GetRequiredService<IOptions<GrpcOptions>>().Value;
            o.Address = new Uri(settings.ApplicationServiceUri ??
                                throw new ArgumentNullException(settings.ApplicationServiceUri, "Empty configuration for application GrpcClient"));
        });

        return services;
    }

    public static IServiceCollection ConfigureSwaggerGen(this IServiceCollection services)
    {
        return services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "gRPC HTTP Gateway", Version = "v1" });
        });
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ApplicationGrpcService>();

        return services;
    }
}