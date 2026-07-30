using ArgosHound.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArgosHound.Api.Services;

public sealed class DiscoveryService(
    IBuilderProfileStore builderProfileStore,
    IProductCatalog productCatalog,
    ISourceDiscussionService sourceDiscussionService,
    ILlmAnalysisProvider analysisProvider,
    IOpportunityScoringService scoringService,
    IOpportunityRepository opportunityRepository) : IDiscoveryService
{
    public async Task<Opportunity> DiscoverAsync(
        Guid discussionId,
        CancellationToken cancellationToken = default)
    {
        var existing = await opportunityRepository.GetByDiscussionAsync(
            discussionId,
            cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var builder = builderProfileStore.Get();
        var context = new OpportunityAnalysisContext(
            builder,
            productCatalog.GetForBuilder(builder.Id),
            sourceDiscussionService.Get(discussionId));
        var analysis = await analysisProvider.AnalyzeAsync(
            context,
            cancellationToken);
        var score = scoringService.Calculate(analysis, context);

        var opportunity = new Opportunity
        {
            Id = Guid.NewGuid(),
            DiscussionId = discussionId,
            Type = analysis.OpportunityType,
            ProductMatchType = analysis.ProductMatch?.MatchType,
            Problem = analysis.Problem.Summary,
            ProblemInferred = analysis.Problem.Inferred,
            Topic = analysis.Topic,
            Sentiment = analysis.Sentiment,
            MatchedProductId = analysis.ProductMatch?.ProductId,
            MatchedProductName = analysis.ProductMatch?.ProductName,
            MatchedCapabilities =
                analysis.ProductMatch?.MatchedCapabilities ?? [],
            BuilderSubtype = analysis.BuilderMatch?.Subtype,
            MatchedSkills = analysis.BuilderMatch?.MatchedSkills ?? [],
            AdvancedGoals = analysis.BuilderMatch?.AdvancedGoals ?? [],
            EffortEstimate = analysis.BuilderMatch?.EffortEstimate,
            NextSteps = analysis.BuilderMatch?.NextSteps ?? [],
            Limitations = analysis.Limitations,
            EvidenceReferences = analysis.EvidenceReferences,
            Explanation = analysis.Explanation,
            SuggestedAction = analysis.SuggestedAction,
            Confidence = analysis.Confidence,
            Score = score.Value,
            ScoreFactors = score.Factors,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        try
        {
            return await opportunityRepository.AddAsync(
                opportunity,
                cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            var concurrentResult =
                await opportunityRepository.GetByDiscussionAsync(
                    discussionId,
                    cancellationToken);
            return concurrentResult
                ?? throw new InvalidOperationException(
                    "Unable to persist the discovered opportunity.",
                    exception);
        }
    }
}
