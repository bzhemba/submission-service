#pragma warning disable CA1506
using Itmo.Dev.Platform.Common.Extensions;
using Itmo.Dev.Platform.MessagePersistence.Extensions;
using Itmo.Dev.Platform.MessagePersistence.Postgres.Extensions;
using Itmo.Dev.Platform.Observability;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;
using SubmissionService.Application.Extensions;
using SubmissionService.Infrastructure.Persistence.Extensions;
using SubmissionService.Presentation.Grpc.Extensions;
using SubmissionService.Presentation.Kafka.Extensions;
using SubmissionService.Presentation.Kafka.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddPlatform();
builder.AddPlatformObservability();
builder.Services.AddUtcDateTimeProvider();
builder.Services.AddSwaggerGen();
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddOptions<JsonSerializerSettings>()
    .Configure(options => options.NullValueHandling = NullValueHandling.Ignore);

builder.Services.AddSingleton<JsonSerializerSettings>(
    provider => provider.GetRequiredService<IOptions<JsonSerializerSettings>>().Value);

builder.Services.AddPlatformMessagePersistence(selector => selector
    .UsePostgresPersistence(postgres => postgres
        .ConfigureOptions(optionsBuilder => optionsBuilder
            .BindConfiguration("Infrastructure:MessagePersistence:Persistence"))));

builder.Services.AddQuartz();

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddHostedService<MessageProducerService>();

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