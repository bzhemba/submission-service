using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace SubmissionService.Infrastructure.Persistence.Migrations;

[Migration(1, "initial")]
#pragma warning disable SA1649
public class Initial : SqlMigration
#pragma warning restore SA1649
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        create type application_state as enum
        (
            'draft',
            'pending_approval',
            'approved',
            'cancelled'
        );
        
        create type activity_type as enum
        (
            'lecture',
            'workshop',
            'discussion'
        );

        CREATE TABLE applications (
            id bigint primary key NOT NULL generated always as identity,
            user_id bigint NOT NULL,
            title TEXT,
            activity activity_type,
            description TEXT,
            outline TEXT,
            created_at timestamp with time zone ,
            application_state application_state
        );
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        drop table applications;
        drop type application_state;
        """;
}