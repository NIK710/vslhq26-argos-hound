using ArgosHound.Api.Data;
using ArgosHound.Api.Models;
using ArgosHound.Api.Services;
using Xunit;

namespace ArgosHound.Api.Tests;

public sealed class OpportunityAnalysisValidatorTests
{
    private readonly OpportunityAnalysisValidator _validator = new();
    private readonly OpportunityAnalysisContext _context = new(
        DemoData.Builder,
        DemoData.Products,
        DemoSourceData.Discussions.Single(
            item => item.Id == DemoSourceData.DoomscrollingDiscussionId));

    [Fact]
    public void AcceptsValidProductAnalysis()
    {
        var analysis = _validator.ParseAndValidate(ValidProductJson, _context);

        Assert.Equal(OpportunityType.Product, analysis.OpportunityType);
        Assert.Equal(DemoData.Products[0].Id, analysis.ProductMatch?.ProductId);
    }

    [Fact]
    public void RejectsMalformedJson()
    {
        Assert.Throws<LlmAnalysisOutputException>(
            () => _validator.ParseAndValidate("{not-json}", _context));
    }

    [Fact]
    public void RejectsUnknownEvidenceReference()
    {
        var output = ValidProductJson.Replace(
            "comment_focus_reset",
            "fabricated_comment",
            StringComparison.Ordinal);

        Assert.Throws<LlmAnalysisOutputException>(
            () => _validator.ParseAndValidate(output, _context));
    }

    [Fact]
    public void RejectsUnknownProductId()
    {
        var output = ValidProductJson.Replace(
            DemoData.Products[0].Id.ToString(),
            Guid.NewGuid().ToString(),
            StringComparison.Ordinal);

        Assert.Throws<LlmAnalysisOutputException>(
            () => _validator.ParseAndValidate(output, _context));
    }

    [Fact]
    public void RejectsInventedProductCapability()
    {
        var output = ValidProductJson.Replace(
            "Interrupt infinite-scroll behavior",
            "Read the user's mind",
            StringComparison.Ordinal);

        Assert.Throws<LlmAnalysisOutputException>(
            () => _validator.ParseAndValidate(output, _context));
    }

    [Fact]
    public void RejectsProductMatchForBuilderOpportunity()
    {
        var output = ValidProductJson.Replace(
            "\"opportunityType\": \"PRODUCT\"",
            "\"opportunityType\": \"BUILDER\"",
            StringComparison.Ordinal);

        Assert.Throws<LlmAnalysisOutputException>(
            () => _validator.ParseAndValidate(output, _context));
    }

    private const string ValidProductJson =
        """
        {
          "problem": {
            "summary": "Students lose study time to doomscrolling.",
            "inferred": false
          },
          "topic": "Digital wellbeing",
          "sentiment": "NEGATIVE",
          "evidenceReferences": [
            "argos_demo_doomscrolling",
            "comment_focus_reset"
          ],
          "opportunityType": "PRODUCT",
          "productMatch": {
            "productId": "25c26703-61f5-4560-83df-a32908739b76",
            "productName": "ScrollGuard",
            "matchType": "DIRECT",
            "matchedCapabilities": [
              "Interrupt infinite-scroll behavior"
            ]
          },
          "limitations": [
            "The sample is small."
          ],
          "explanation": "ScrollGuard addresses the stated interruption need.",
          "suggestedAction": "Review the source and prepare a relevant public response.",
          "confidence": 0.8
        }
        """;
}
