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
                                  event_id,
                                  user_email,
                                  started_at,
                                  finished_at,
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
                EventId: reader.GetInt64("event_id"),
                StartedAt: reader.GetFieldValue<DateTimeOffset>("started_at"),
                FinishedAt: reader.GetFieldValue<DateTimeOffset>("finished_at"),
                UserEmail: reader.GetString("user_email"),
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
                           insert into applications (event_id, user_email, started_at, finished_at, title, activity, description, outline, created_at, application_state)
                           values (:eventId, :userEmail, :startedAt, :finishedAt, :title, :activity, :description, :outline, :createdAt, :applicationState)
                           returning id;
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("eventId", application.EventId)
            .AddParameter("startedAt", application.StartedAt)
            .AddParameter("finishedAt", application.FinishedAt)
            .AddParameter("userEmail", application.UserEmail)
            .AddParameter("activity", application.Activity)
            .AddParameter("title", application.Title)
            .AddParameter("description", application.Description)
            .AddParameter("outline", application.Outline)
            .AddParameter("createdAt", DateTimeOffset.UtcNow)
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
                                  event_id,
                                  user_email,
                                  started_at,
                                  finished_at,
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
                EventId: reader.GetInt64("event_id"),
                Id: reader.GetInt64("id"),
                StartedAt: reader.GetFieldValue<DateTimeOffset>("started_at"),
                FinishedAt: reader.GetFieldValue<DateTimeOffset>("finished_at"),
                UserEmail: reader.GetString("user_email"),
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
                           started_at = :startedAt,
                           finished_at = :finishedAt,
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
            .AddParameter("startedAt", application.StartedAt)
            .AddParameter("finishedAt", application.FinishedAt)
            .AddParameter("title", application.Title)
            .AddParameter("description", application.Description)
            .AddParameter("outline", application.Outline);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<ApplicationModel?> GetDraftByUserEmailAsync(
        string userEmail,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           select id,
                                  event_id,
                                  user_email,
                                  started_at,
                                  finished_at,
                                  title,
                                  activity,
                                  description,
                                  outline,
                                  created_at,
                                  application_state
                           from applications where user_email = :userEmail and application_state = 'draft';
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("userEmail", userEmail);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        return reader.HasRows
            ? new ApplicationModel(
                EventId: reader.GetInt64("event_id"),
                Id: reader.GetInt64("id"),
                StartedAt: reader.GetFieldValue<DateTimeOffset>("started_at"),
                FinishedAt: reader.GetFieldValue<DateTimeOffset>("finished_at"),
                UserEmail: reader.GetString("user_email"),
                Activity: reader.IsDBNull(reader.GetOrdinal("activity")) ? null : reader.GetFieldValue<ActivityType>("activity"),
                Title: reader.IsDBNull(reader.GetOrdinal("title")) ? null : reader.GetString("title"),
                Description: reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                Outline: reader.IsDBNull(reader.GetOrdinal("outline")) ? null : reader.GetString("outline"),
                CreatedAt: reader.GetFieldValue<DateTimeOffset>("created_at"),
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