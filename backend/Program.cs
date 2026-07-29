using ArgosHound.Api.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

const string frontendCorsPolicy = "Frontend";
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? ["http://localhost:5173", "http://127.0.0.1:5173"];

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IBuilderProfileStore, InMemoryBuilderProfileStore>();
builder.Services.AddSingleton<IProfileImportService, InMemoryProfileImportService>();
builder.Services.AddSingleton<ISourceDiscussionService, InMemorySourceDiscussionService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors(frontendCorsPolicy);

app.MapControllers();
app.MapHealthChecks("/api/health");

app.Run();
