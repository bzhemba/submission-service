using Itmo.Dev.Platform.Persistence.Postgres.Plugins;
using Npgsql;
using SubmissionService.Application.Models.Applications;

namespace SubmissionService.Infrastructure.Persistence.Plugins;

/// <summary>
///     Plugin for configuring NpgsqlDataSource's mappings
///     ie: enums, composite types
/// </summary>
public class MappingPlugin : IPostgresDataSourcePlugin
{
    public void Configure(NpgsqlDataSourceBuilder dataSource)
    {
        dataSource.MapEnum<ApplicationState>("application_state");
        dataSource.MapEnum<ActivityType>("activity_type");
    }
}