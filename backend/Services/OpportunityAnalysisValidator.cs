using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public sealed class OpportunityAnalysisValidator
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        Converters =
        {
            new JsonStringEnumConverter(
                JsonNamingPolicy.SnakeCaseUpper,
                allowIntegerValues: false),
        },
    };

    public OpportunityAnalysis ParseAndValidate(
        string output,
        OpportunityAnalysisContext context)
    {
        OpportunityAnalysis analysis;

        try
        {
            analysis = JsonSerializer.Deserialize<OpportunityAnalysis>(
                    output,
                    JsonOptions)
                ?? throw new JsonException("The response was JSON null.");
        }
        catch (JsonException exception)
        {
            throw new LlmAnalysisOutputException(
                "The model returned malformed structured analysis.",
                exception);
        }

        if (analysis.Problem is null)
        {
            Reject("problem is required.");
        }

        ValidateRequiredText(analysis.Problem.Summary, "problem.summary");
        ValidateRequiredText(analysis.Topic, "topic");
        ValidateRequiredText(analysis.Explanation, "explanation");
        ValidateRequiredText(analysis.SuggestedAction, "suggestedAction");
        ValidateNonEmptyTextList(analysis.Limitations, "limitations");

        if (analysis.Confidence is < 0 or > 1)
        {
            Reject("confidence must be between 0 and 1.");
        }

        var validEvidenceIds = context.Discussion.Comments
            .Select(comment => comment.ExternalId)
            .Append(context.Discussion.ExternalId)
            .ToHashSet(StringComparer.Ordinal);

        var evidenceReferences = RequireList(
            analysis.EvidenceReferences,
            "evidenceReferences");
        EnsureUnique(evidenceReferences, "evidenceReferences");
        foreach (var evidenceId in evidenceReferences)
        {
            ValidateRequiredText(evidenceId, "evidenceReferences");
            if (!validEvidenceIds.Contains(evidenceId))
            {
                Reject($"Unknown evidence reference '{evidenceId}'.");
            }
        }

        if (analysis.OpportunityType != OpportunityType.None
            && evidenceReferences.Count == 0)
        {
            Reject("Product and builder opportunities require source evidence.");
        }

        if (analysis.OpportunityType == OpportunityType.Product)
        {
            ValidateProductMatch(analysis.ProductMatch, context.Products);
        }
        else if (analysis.ProductMatch is not null)
        {
            Reject("productMatch must be null for BUILDER and NONE.");
        }

        return analysis;
    }

    private static void ValidateProductMatch(
        ProductMatchAnalysis? match,
        IReadOnlyList<Product> products)
    {
        if (match is null)
        {
            Reject("PRODUCT opportunities require productMatch.");
        }

        var product = products.SingleOrDefault(item => item.Id == match.ProductId);
        if (product is null)
        {
            Reject($"Unknown product ID '{match.ProductId}'.");
        }

        if (!string.Equals(match.ProductName, product.Name, StringComparison.Ordinal))
        {
            Reject("productName does not match the supplied product.");
        }

        var matchedCapabilities = RequireList(
            match.MatchedCapabilities,
            "productMatch.matchedCapabilities");
        ValidateNonEmptyTextList(
            matchedCapabilities,
            "productMatch.matchedCapabilities");
        EnsureUnique(
            matchedCapabilities,
            "productMatch.matchedCapabilities");

        var validCapabilities = product.Capabilities.ToHashSet(StringComparer.Ordinal);
        foreach (var capability in matchedCapabilities)
        {
            if (!validCapabilities.Contains(capability))
            {
                Reject($"Unknown capability '{capability}' for product '{product.Name}'.");
            }
        }
    }

    private static void ValidateNonEmptyTextList(
        IReadOnlyList<string>? values,
        string field)
    {
        if (values is null || values.Count == 0)
        {
            Reject($"{field} must contain at least one item.");
        }

        foreach (var value in values)
        {
            ValidateRequiredText(value, field);
        }
    }

    private static void EnsureUnique(
        IReadOnlyList<string>? values,
        string field)
    {
        if (values is null)
        {
            Reject($"{field} is required.");
        }

        if (values.Count != values.Distinct(StringComparer.Ordinal).Count())
        {
            Reject($"{field} must not contain duplicates.");
        }
    }

    private static IReadOnlyList<string> RequireList(
        IReadOnlyList<string>? values,
        string field)
    {
        if (values is null)
        {
            Reject($"{field} is required.");
        }

        return values;
    }

    private static void ValidateRequiredText(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Reject($"{field} is required.");
        }
    }

    [DoesNotReturn]
    private static void Reject(string detail) =>
        throw new LlmAnalysisOutputException(
            $"The model returned invalid structured analysis: {detail}");
}
