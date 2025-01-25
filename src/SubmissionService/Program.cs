#pragma warning disable CA1506
using Itmo.Dev.Platform.Common.Extensions;
using Itmo.Dev.Platform.MessagePersistence.Extensions;
using Itmo.Dev.Platform.MessagePersistence.Postgres.Extensions;
using Itmo.Dev.Platform.Observability;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SubmissionService.Application.Extensions;
using SubmissionService.Infrastructure.Persistence.Extensions;
using SubmissionService.Presentation.Grpc.Extensions;
using SubmissionService.Presentation.Http.Extensions;
using SubmissionService.Presentation.Kafka.Extensions;
using SubmissionService.Presentation.Kafka.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddPlatform();
builder.AddPlatformObservability();
builder.Services.AddUtcDateTimeProvider();
builder.Services
    .AddControllers()
    .AddNewtonsoftJson()
    .AddPresentationHttp();

// Null value ignore is needed to correctly deserialize oneof messages in inbox/outbox
builder.Services.AddOptions<JsonSerializerSettings>()
    .Configure(options => options.NullValueHandling = NullValueHandling.Ignore);

builder.Services.AddSingleton<JsonSerializerSettings>(
    provider => provider.GetRequiredService<IOptions<JsonSerializerSettings>>().Value);

// Used as inbox and outbox infrastructure
builder.Services.AddPlatformMessagePersistence(selector => selector
    .UsePostgresPersistence(postgres => postgres
        .ConfigureOptions(optionsBuilder => optionsBuilder
            .BindConfiguration("Infrastructure:MessagePersistence:Persistence"))));

builder.Services.AddHostedService<BatchMessageConsumerService>();
builder.Services
    .AddApplication()
    .AddInfrastructurePersistence()
    .AddPresentationGrpc()
    .AddPresentationKafka(builder.Configuration);

WebApplication app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();

app.UsePlatformObservability();

app.UsePresentationGrpc();
app.MapControllers();

await app.RunAsync();
#pragma warning restore CA1506
