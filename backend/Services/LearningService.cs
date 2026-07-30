using System.Text.Json;
using ArgosHound.Api.Contracts;
using ArgosHound.Api.Data;
using ArgosHound.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArgosHound.Api.Services;

public sealed class LearningService(
    ArgosHoundDbContext dbContext,
    ISourceDiscussionService sourceDiscussions)
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public async Task<LearningSummaryResponse> GetSummaryAsync(CancellationToken token)
    {
        var opportunities = await dbContext.Opportunities.AsNoTracking().ToListAsync(token);
        var decisions = await dbContext.BuilderDecisions.AsNoTracking().ToListAsync(token);
        var outcomes = await dbContext.Outcomes.AsNoTracking().ToListAsync(token);
        var rows = opportunities.Select(opportunity =>
        {
            var source = sourceDiscussions.Get(opportunity.DiscussionId);
            return new HistoryRow(
                opportunity.Id, source.Platform, source.Community, opportunity.Topic,
                opportunity.MatchedProductName ?? "No product", opportunity.Type,
                decisions.Count(x => x.OpportunityId == opportunity.Id),
                outcomes.Count(x => x.OpportunityId == opportunity.Id));
        }).ToArray();

        return new(
            Aggregate(rows, x => x.Source),
            Aggregate(rows, x => x.Community),
            Aggregate(rows, x => x.Topic),
            Aggregate(rows, x => x.Product),
            Aggregate(rows, x => x.Type));
    }

    public async Task RescoreAsync(Guid opportunityId, CancellationToken token)
    {
        var target = await dbContext.Opportunities.SingleOrDefaultAsync(
            x => x.Id == opportunityId, token);
        if (target is null) return;

        var relatedIds = await FindRelatedIdsAsync(target, token);
        var decisions = await dbContext.BuilderDecisions.AsNoTracking()
            .Where(x => relatedIds.Contains(x.OpportunityId)).ToListAsync(token);
        var outcomes = await dbContext.Outcomes.AsNoTracking()
            .Where(x => relatedIds.Contains(x.OpportunityId)).ToListAsync(token);
        var points = decisions.Sum(x => Enum.Parse<BuilderDecisionType>(x.DecisionType) switch
        {
            BuilderDecisionType.Saved => OpportunityScoreWeights.SavedDecision,
            BuilderDecisionType.Dismissed => OpportunityScoreWeights.DismissedDecision,
            BuilderDecisionType.Pursued => OpportunityScoreWeights.PursuedDecision,
            _ => 0,
        }) + outcomes.Sum(x => Enum.Parse<OutcomeType>(x.OutcomeType) switch
        {
            OutcomeType.LearningValue or OutcomeType.PrototypeCompleted =>
                OpportunityScoreWeights.LearningOutcome,
            OutcomeType.Portfolio or OutcomeType.Collaboration or OutcomeType.Interview
                or OutcomeType.Contract => OpportunityScoreWeights.CareerOutcome,
            _ => 0,
        });
        points = Math.Clamp(
            points, -OpportunityScoreWeights.HistoryMaximum,
            OpportunityScoreWeights.HistoryMaximum);

        var factors = JsonSerializer.Deserialize<OpportunityScoreFactor[]>(
            target.ScoreFactorsJson, JsonOptions) ?? [];
        factors = factors.Where(x => x.Key != "relevantHistory")
            .Append(new OpportunityScoreFactor(
                "relevantHistory", "Relevant history", points,
                $"{decisions.Count} decision(s) and {outcomes.Count} reported learning/career outcome(s) "
                + "across matching community, topic, product, or opportunity type."))
            .ToArray();
        target.ScoreFactorsJson = JsonSerializer.Serialize(factors, JsonOptions);
        target.Score = Math.Clamp(factors.Sum(x => x.Points), 0, 100);
        await dbContext.SaveChangesAsync(token);
    }

    private async Task<Guid[]> FindRelatedIdsAsync(
        OpportunityEntity target, CancellationToken token)
    {
        var targetCommunity = sourceDiscussions.Get(target.DiscussionId).Community;
        var opportunities = await dbContext.Opportunities.AsNoTracking().ToListAsync(token);
        return opportunities.Where(x =>
                x.Id == target.Id
                || x.Topic == target.Topic
                || x.Type == target.Type
                || (target.MatchedProductId is not null
                    && x.MatchedProductId == target.MatchedProductId)
                || sourceDiscussions.Get(x.DiscussionId).Community == targetCommunity)
            .Select(x => x.Id).ToArray();
    }

    private static IReadOnlyList<LearningAggregate> Aggregate(
        IEnumerable<HistoryRow> rows, Func<HistoryRow, string> key) =>
        rows.GroupBy(key, StringComparer.OrdinalIgnoreCase)
            .Select(group => new LearningAggregate(
                group.Key, group.Count(), group.Sum(x => x.Decisions),
                group.Sum(x => x.Outcomes)))
            .OrderByDescending(x => x.Decisions + x.Outcomes)
            .ThenBy(x => x.Value, StringComparer.OrdinalIgnoreCase).ToArray();

    private sealed record HistoryRow(
        Guid Id, string Source, string Community, string Topic, string Product,
        string Type, int Decisions, int Outcomes);
}
