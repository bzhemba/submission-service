using Microsoft.Extensions.DependencyInjection;
using SubmissionService.Application.Application;
using SubmissionService.Application.Contracts;

namespace SubmissionService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<IApplicationService, ApplicationService>();

        return collection;
    }
}