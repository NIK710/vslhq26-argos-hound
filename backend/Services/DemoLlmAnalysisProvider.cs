using ArgosHound.Api.Data;
using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public sealed class DemoLlmAnalysisProvider : ILlmAnalysisProvider
{
    public Task<OpportunityAnalysis> AnalyzeAsync(
        OpportunityAnalysisContext context,
        CancellationToken cancellationToken = default)
    {
        var analysis = context.Discussion.Id switch
        {
            var id when id == DemoSourceData.DoomscrollingDiscussionId =>
                ProductOpportunity(context),
            var id when id == DemoSourceData.ChessClubDiscussionId =>
                BuilderOpportunity(context),
            _ => NoOpportunity(context),
        };
        return Task.FromResult(analysis);
    }

    private static OpportunityAnalysis ProductOpportunity(
        OpportunityAnalysisContext context)
    {
        var product = context.Products.Single(x => x.Name == "ScrollGuard");
        return new()
        {
            Problem = new ProblemAnalysis
            {
                Summary = "Students lose study time to habitual scrolling.",
                Inferred = false,
            },
            Topic = "Digital wellbeing and study focus",
            Sentiment = DiscussionSentiment.Negative,
            EvidenceReferences =
                [context.Discussion.ExternalId, "comment_focus_reset"],
            OpportunityType = OpportunityType.Product,
            ProductMatch = new ProductMatchAnalysis
            {
                ProductId = product.Id,
                ProductName = product.Name,
                MatchType = ProductMatchType.Direct,
                MatchedCapabilities =
                    ["Interrupt infinite-scroll behavior"],
            },
            Limitations =
                ["The discussion does not confirm willingness to install an extension."],
            Explanation =
                "ScrollGuard directly addresses the requested lightweight interruption.",
            SuggestedAction =
                "Review the thread and prepare a helpful, non-promotional response.",
            Confidence = 0.9m,
        };
    }

    private static OpportunityAnalysis BuilderOpportunity(
        OpportunityAnalysisContext context) =>
        new()
        {
            Problem = new ProblemAnalysis
            {
                Summary =
                    "The chess club is outgrowing spreadsheets for attendance, pairings, and volunteer coordination.",
                Inferred = false,
            },
            Topic = "Local chess club coordination",
            Sentiment = DiscussionSentiment.Negative,
            EvidenceReferences =
                [context.Discussion.ExternalId, "comment_pairings", "comment_volunteers"],
            OpportunityType = OpportunityType.Builder,
            BuilderMatch = new BuilderMatchAnalysis
            {
                Subtype = BuilderOpportunitySubtype.CommunityService,
                MatchedSkills = context.Builder.CurrentSkills
                    .Where(x => x is "C#" or "React").ToArray(),
                AdvancedGoals =
                    [context.Builder.LearningGoals.First()],
                EffortEstimate = "Prototype in one to four weeks",
                NextSteps =
                    ["Interview club organizers", "Prototype a lightweight check-in flow"],
            },
            Limitations =
                ["Organizer requirements and data-handling expectations are not confirmed."],
            Explanation =
                "The builder can apply existing application skills while gaining experience with real community users.",
            SuggestedAction =
                "Interview organizers before deciding whether to prototype.",
            Confidence = 0.84m,
        };

    private static OpportunityAnalysis NoOpportunity(
        OpportunityAnalysisContext context) =>
        new()
        {
            Problem = new ProblemAnalysis
            {
                Summary = "No credible unmet need was identified.",
                Inferred = false,
            },
            Topic = "Mechanical keyboard showcase",
            Sentiment = DiscussionSentiment.Positive,
            EvidenceReferences = [context.Discussion.ExternalId],
            OpportunityType = OpportunityType.None,
            Limitations = ["The post shares completed work rather than requesting help."],
            Explanation =
                "The discussion has no meaningful product or builder fit.",
            SuggestedAction = "Do not pursue this discussion.",
            Confidence = 0.95m,
        };
}
