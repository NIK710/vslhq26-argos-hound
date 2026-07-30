using ArgosHound.Api.Data;
using ArgosHound.Api.Models;
using ArgosHound.Api.Services;
using Xunit;

namespace ArgosHound.Api.Tests;

public sealed class OpportunityScoringServiceTests
{
    private readonly OpportunityScoringService _service = new();
    private readonly OpportunityAnalysisContext _context = new(
        DemoData.Builder,
        DemoData.Products,
        DemoSourceData.Discussions.Single(
            item => item.Id == DemoSourceData.DoomscrollingDiscussionId));

    [Fact]
    public void CalculatesExpectedDeterministicFactors()
    {
        var score = _service.Calculate(CreateAnalysis(0.25m), _context);

        Assert.Equal(79, score.Value);
        Assert.Equal(
            ["evidenceStrength", "problemClarity", "productFit", "actionability", "uncertainty"],
            score.Factors.Select(factor => factor.Key));
    }

    [Fact]
    public void ModelConfidenceDoesNotChangeDeterministicScore()
    {
        var lowConfidence = _service.Calculate(CreateAnalysis(0.1m), _context);
        var highConfidence = _service.Calculate(CreateAnalysis(0.95m), _context);

        Assert.Equal(lowConfidence.Value, highConfidence.Value);
    }

    public static OpportunityAnalysis CreateAnalysis(decimal confidence) =>
        new()
        {
            Problem = new ProblemAnalysis
            {
                Summary = "Students lose study time to doomscrolling.",
                Inferred = false,
            },
            Topic = "Digital wellbeing",
            Sentiment = DiscussionSentiment.Negative,
            EvidenceReferences =
            [
                "argos_demo_doomscrolling",
                "comment_focus_reset",
                "comment_assignment_intent",
                "comment_summary_need",
            ],
            OpportunityType = OpportunityType.Product,
            ProductMatch = new ProductMatchAnalysis
            {
                ProductId = DemoData.Products[0].Id,
                ProductName = DemoData.Products[0].Name,
                MatchType = ProductMatchType.Direct,
                MatchedCapabilities =
                [
                    "Interrupt infinite-scroll behavior",
                    "Prompt users to return to a stated task",
                ],
            },
            Limitations =
            [
                "The sample is small.",
                "User preferences may vary.",
            ],
            Explanation = "The product directly addresses the cited need.",
            SuggestedAction = "Review the thread and test a relevant demo.",
            Confidence = confidence,
        };
}
