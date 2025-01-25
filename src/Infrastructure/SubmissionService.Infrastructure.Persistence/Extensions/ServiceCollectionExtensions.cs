using Itmo.Dev.Platform.Persistence.Abstractions.Extensions;
using Itmo.Dev.Platform.Persistence.Postgres.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SubmissionService.Application.Abstractions.Persistence;
using SubmissionService.Application.Abstractions.Persistence.Repositories;
using SubmissionService.Infrastructure.Persistence.Plugins;
using SubmissionService.Infrastructure.Persistence.Repositories;

namespace SubmissionService.Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection collection)
    {
        collection.AddPlatformPersistence(persistence => persistence
            .UsePostgres(postgres => postgres
                .WithConnectionOptions(builder => builder.BindConfiguration("Infrastructure:Persistence:Postgres"))
                .WithMigrationsFrom(typeof(IAssemblyMarker).Assembly)
                .WithDataSourcePlugin<MappingPlugin>()));

        collection.AddScoped<IPersistenceContext, PersistenceContext>();

        collection.AddScoped<IApplicationsRepository, ApplicationRepository>();

        return collection;
    }
}