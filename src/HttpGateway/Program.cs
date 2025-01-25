using HttpGateway;
using HttpGateway.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ExceptionFormattingMiddleware>();
builder.Services.AddServices();
builder.Services.AddLogging();
builder.Services.ConfigureGrpcApplicationServiceClient();
builder.Services.ConfigureSwaggerGen();

builder.Services.AddControllers();

WebApplication app = builder.Build();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ExceptionFormattingMiddleware>();

app.MapControllers();

app.Run();