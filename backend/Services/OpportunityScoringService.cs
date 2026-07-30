using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public sealed class OpportunityScoringService : IOpportunityScoringService
{
    public OpportunityScore Calculate(
        OpportunityAnalysis analysis,
        OpportunityAnalysisContext context)
    {
        if (analysis.OpportunityType == OpportunityType.None)
        {
            return new OpportunityScore(
                0,
                [
                    new(
                        "personalizedFit",
                        "Personalized fit",
                        0,
                        "The validated analysis found no product or builder opportunity."),
                ]);
        }

        var evidencePoints = Math.Min(
            20,
            analysis.EvidenceReferences.Count * 5);
        var clarityPoints = analysis.Problem.Inferred ? 8 : 15;
        var fitFactor = CalculateFit(analysis, context);
        const int actionabilityPoints = 15;
        var limitationPenalty = -Math.Min(
            15,
            analysis.Limitations.Count * 3);

        OpportunityScoreFactor[] factors =
        [
            new(
                "evidenceStrength",
                "Evidence strength",
                evidencePoints,
                $"{analysis.EvidenceReferences.Count} validated source reference(s)."),
            new(
                "problemClarity",
                "Problem clarity",
                clarityPoints,
                analysis.Problem.Inferred
                    ? "The problem is inferred rather than explicitly requested."
                    : "The source explicitly describes the problem or need."),
            fitFactor,
            new(
                "actionability",
                "Actionability",
                actionabilityPoints,
                "The analysis includes a concrete, reviewable next action."),
            new(
                "uncertainty",
                "Uncertainty and limitations",
                limitationPenalty,
                $"{analysis.Limitations.Count} limitation(s) reduce the score."),
        ];

        return new OpportunityScore(
            Math.Clamp(factors.Sum(factor => factor.Points), 0, 100),
            factors);
    }

    private static OpportunityScoreFactor CalculateFit(
        OpportunityAnalysis analysis,
        OpportunityAnalysisContext context)
    {
        if (analysis.OpportunityType == OpportunityType.Product)
        {
            var points = analysis.ProductMatch!.MatchType switch
            {
                ProductMatchType.Direct => 35,
                ProductMatchType.Adjacent => 25,
                ProductMatchType.SmallExtension => 18,
                _ => 0,
            };

            return new OpportunityScoreFactor(
                "productFit",
                "Product fit",
                points,
                $"{analysis.ProductMatch.MatchType} match to "
                + $"{analysis.ProductMatch.MatchedCapabilities.Count} validated capability reference(s).");
        }

        var profileSignals = context.Builder.CurrentSkills.Count
            + context.Builder.LearningGoals.Count
            + context.Builder.Interests.Count;
        var builderPoints = Math.Min(30, 20 + profileSignals / 3);

        return new OpportunityScoreFactor(
            "builderFit",
            "Builder fit",
            builderPoints,
            "The validated builder opportunity is supported by the active profile's "
            + "skills, goals, and interests.");
    }
}
