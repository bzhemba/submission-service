using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using SubmissionService.Application.Abstractions.Persistence.Commands;
using SubmissionService.Application.Abstractions.Persistence.Queries;
using SubmissionService.Application.Abstractions.Persistence.Repositories;
using SubmissionService.Application.Models.Applications;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace SubmissionService.Infrastructure.Persistence.Repositories;

internal class ApplicationRepository : IApplicationsRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public ApplicationRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async IAsyncEnumerable<ApplicationModel> GetByIdAsync(
        GetApplicationByIdQuery byIdQuery,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           select id,
                                  user_id,
                                  title,
                                  activity,
                                  description,
                                  outline,
                                  created_at,
                                  application_state
                           from applications
                           where 
                              id = :id
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("id", byIdQuery.ApplicationId);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new ApplicationModel(
                Id: reader.GetInt64("id"),
                UserId: reader.GetInt64("user_id"),
                Activity: reader.IsDBNull(reader.GetOrdinal("activity")) ? null : reader.GetFieldValue<ActivityType>("activity"),
                Title: reader.IsDBNull(reader.GetOrdinal("title")) ? null : reader.GetString("title"),
                Description: reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                Outline: reader.IsDBNull(reader.GetOrdinal("outline")) ? null : reader.GetString("outline"),
                CreatedAt: reader.GetFieldValue<DateTimeOffset>("created_at"),
                State: reader.GetFieldValue<ApplicationState>("application_state"));
        }
    }

    public async Task<long> CreateAsync(CreateApplicationCommand application, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into applications (user_id, title, activity, description, outline, created_at, application_state)
                           values (:userId, :title, :activity, :description, :outline, :createdAt, :applicationState)
                           returning id;
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("userId", application.UserId)
            .AddParameter("activity", application.Activity)
            .AddParameter("title", application.Title)
            .AddParameter("description", application.Description)
            .AddParameter("outline", application.Outline)
            .AddParameter("createdAt", DateTime.Now)
            .AddParameter("applicationState", application.State);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);
        long id = reader.GetInt64("id");

        return id;
    }

    public async IAsyncEnumerable<ApplicationModel> GetExpiredDraftsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           select id,
                                  user_id,
                                  title,
                                  activity,
                                  description,
                                  outline,
                                  created_at,
                                  application_state
                           from applications
                           where 
                               application_state = 'draft' 
                               and created_at <= current_date - interval '2' day;
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new ApplicationModel(
                Id: reader.GetInt64("id"),
                UserId: reader.GetInt64("user_id"),
                Activity: reader.IsDBNull(reader.GetOrdinal("activity")) ? null : reader.GetFieldValue<ActivityType>("activity"),
                Title: reader.IsDBNull(reader.GetOrdinal("title")) ? null : reader.GetString("title"),
                Description: reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                Outline: reader.IsDBNull(reader.GetOrdinal("outline")) ? null : reader.GetString("outline"),
                CreatedAt: reader.GetFieldValue<DateTimeOffset>("created_at"),
                State: reader.GetFieldValue<ApplicationState>("application_state"));
        }
    }

    public async Task UpdateAsync(
        UpdateApplicationCommand application,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           update applications set 
                           title = :title,
                           activity = :activity,
                           description = :description,
                           outline = :outline
                           where id = :id;
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("id", application.Id)
            .AddParameter("activity", application.Activity)
            .AddParameter("title", application.Title)
            .AddParameter("description", application.Description)
            .AddParameter("outline", application.Outline);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<ApplicationModel?> GetDraftByUserIdAsync(
        long userId,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           select id,
                                  user_id,
                                  title,
                                  activity,
                                  description,
                                  outline,
                                  created_at,
                                  application_state
                           from applications where user_id = :userId and application_state = 'draft';
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("userId", userId);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        return reader.HasRows
            ? new ApplicationModel(
                Id: reader.GetInt64("id"),
                UserId: reader.GetInt64("user_id"),
                Activity: reader.IsDBNull(reader.GetOrdinal("activity")) ? null : reader.GetFieldValue<ActivityType>("activity"),
                Title: reader.IsDBNull(reader.GetOrdinal("title")) ? null : reader.GetString("title"),
                Description: reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                Outline: reader.IsDBNull(reader.GetOrdinal("outline")) ? null : reader.GetString("outline"),
                CreatedAt: reader.GetDateTime("created_at"),
                State: reader.GetFieldValue<ApplicationState>("application_state"))
            : null;
    }

    public async Task ChangeStatusAsync(
        long applicationId,
        ApplicationState newState,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           update applications set application_state = :newState
                           where id = :id;
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);
        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("id", applicationId)
            .AddParameter("newState", newState);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}