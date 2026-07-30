using System.Text.Json;
using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public sealed class OpportunityAnalysisPromptBuilder(
    IHostEnvironment environment) : IOpportunityAnalysisPromptBuilder
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly string _templatePath = Path.Combine(
        environment.ContentRootPath,
        "Prompts",
        "opportunity-analysis-request.md");
    private readonly string _schemaPath = Path.Combine(
        environment.ContentRootPath,
        "Schemas",
        "opportunity-analysis.schema.json");

    public async Task<string> BuildAsync(
        OpportunityAnalysisContext context,
        CancellationToken cancellationToken = default)
    {
        var template = await File.ReadAllTextAsync(
            _templatePath,
            cancellationToken);
        var schema = await File.ReadAllTextAsync(
            _schemaPath,
            cancellationToken);

        var input = new
        {
            builder = new
            {
                id = context.Builder.Id,
                context.Builder.Name,
                context.Builder.CurrentSkills,
                context.Builder.LearningGoals,
                context.Builder.Interests,
                context.Builder.PreferredOpportunityTypes,
                context.Builder.Location,
                context.Builder.EffortPreferences,
            },
            products = context.Products.Select(product => new
            {
                id = product.Id,
                product.Name,
                product.Description,
                product.Capabilities,
                product.TargetUsers,
            }),
            source = new
            {
                context.Discussion.Platform,
                context.Discussion.ExternalId,
                context.Discussion.Community,
                context.Discussion.Title,
                context.Discussion.Body,
                comments = context.Discussion.Comments.Select(comment => new
                {
                    comment.ExternalId,
                    comment.ParentExternalId,
                    comment.Body,
                }),
            },
        };

        return template
            .Replace(
                "{{OUTPUT_SCHEMA_JSON}}",
                schema,
                StringComparison.Ordinal)
            .Replace(
                "{{UNTRUSTED_INPUT_JSON}}",
                JsonSerializer.Serialize(input, JsonOptions),
                StringComparison.Ordinal);
    }
}
