using ArgosHound.Api.Configuration;
using ArgosHound.Api.Data;
using ArgosHound.Api.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string frontendCorsPolicy = "Frontend";
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? ["http://localhost:5173", "http://127.0.0.1:5173"];

builder.Services
    .AddOptions<AzureOpenAIOptions>()
    .Bind(builder.Configuration.GetSection(AzureOpenAIOptions.SectionName))
    .Validate(
        options =>
            Uri.TryCreate(options.Endpoint, UriKind.Absolute, out var endpoint)
            && endpoint.Scheme == Uri.UriSchemeHttps,
        "AzureOpenAI:Endpoint must be an absolute HTTPS URL.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.DeploymentName),
        "AzureOpenAI:DeploymentName is required.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.ApiKey),
        "AzureOpenAI:ApiKey is required.")
    .ValidateOnStart();

builder.Services
    .AddOptions<FoundryOptions>()
    .Bind(builder.Configuration.GetSection(FoundryOptions.SectionName))
    .Validate(
        options =>
            Uri.TryCreate(options.ProjectEndpoint, UriKind.Absolute, out var endpoint)
            && endpoint.Scheme == Uri.UriSchemeHttps,
        "Foundry:ProjectEndpoint must be an absolute HTTPS URL.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.AgentName),
        "Foundry:AgentName is required.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.AgentVersion),
        "Foundry:AgentVersion is required.")
    .Validate(
        options => options.RequestTimeoutSeconds is >= 5 and <= 120,
        "Foundry:RequestTimeoutSeconds must be between 5 and 120.")
    .Validate(
        options => options.MaxAttempts is >= 1 and <= 3,
        "Foundry:MaxAttempts must be between 1 and 3.")
    .ValidateOnStart();

builder.Services
    .AddOptions<CampaignOptions>()
    .Bind(builder.Configuration.GetSection(CampaignOptions.SectionName))
    .Validate(
        options =>
            Uri.TryCreate(
                options.PublicBaseUrl,
                UriKind.Absolute,
                out var publicBaseUrl)
            && (publicBaseUrl.Scheme == Uri.UriSchemeHttps
                || publicBaseUrl.Scheme == Uri.UriSchemeHttp),
        "Campaign:PublicBaseUrl must be an absolute HTTP or HTTPS URL.")
    .Validate(
        options =>
            options.AllowedDestinationHosts.Count > 0
            && options.AllowedDestinationHosts.All(
                host => Uri.CheckHostName(host) != UriHostNameType.Unknown),
        "Campaign:AllowedDestinationHosts must contain valid host names.")
    .ValidateOnStart();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });
builder.Services.AddHealthChecks();
builder.Services.AddDbContext<ArgosHoundDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("ArgosHound")
        ?? "Data Source=argoshound.db"));
builder.Services.AddSingleton<IBuilderProfileStore, InMemoryBuilderProfileStore>();
builder.Services.AddSingleton<IProductCatalog, DemoProductCatalog>();
builder.Services.AddSingleton<IProfileImportService, InMemoryProfileImportService>();
builder.Services.AddSingleton<ISourceDiscussionService, InMemorySourceDiscussionService>();
builder.Services.AddSingleton<IOpportunityAnalysisPromptBuilder, OpportunityAnalysisPromptBuilder>();
builder.Services.AddSingleton<OpportunityAnalysisValidator>();
builder.Services.AddSingleton<ILlmAnalysisProvider, FoundryLlmAnalysisProvider>();
builder.Services.AddSingleton<IOpportunityScoringService, OpportunityScoringService>();
builder.Services.AddScoped<IOpportunityRepository, OpportunityRepository>();
builder.Services.AddScoped<IDiscoveryService, DiscoveryService>();
builder.Services.AddScoped<IOpportunityReportService, OpportunityReportService>();
builder.Services.AddSingleton<ICampaignCodeService, CampaignCodeService>();
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
builder.Services.AddScoped<ICampaignLinkService, CampaignLinkService>();
builder.Services.AddSingleton<
    IFoundryAgentConnectivityService,
    FoundryAgentConnectivityService>();
builder.Services.AddHttpClient<
    IAzureOpenAIConnectivityService,
    AzureOpenAIConnectivityService>((services, client) =>
    {
        var options = services
            .GetRequiredService<IOptions<AzureOpenAIOptions>>()
            .Value;

        var configuredEndpoint = options.Endpoint.TrimEnd('/');
        var v1Endpoint = configuredEndpoint.EndsWith(
            "/openai/v1",
            StringComparison.OrdinalIgnoreCase)
            ? $"{configuredEndpoint}/"
            : $"{configuredEndpoint}/openai/v1/";

        client.BaseAddress = new Uri(v1Endpoint);
        client.DefaultRequestHeaders.Add("api-key", options.ApiKey);
        client.Timeout = TimeSpan.FromSeconds(30);
    });
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

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ArgosHoundDbContext>();
    ArgosHoundDatabaseInitializer.Initialize(dbContext);
}

app.UseCors(frontendCorsPolicy);

app.MapControllers();
app.MapHealthChecks("/api/health");

app.Run();
