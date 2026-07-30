using ArgosHound.Api.Contracts;
using ArgosHound.Api.Data;
using ArgosHound.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArgosHound.Api.Services;

public sealed class OpportunityActivityService(
    ArgosHoundDbContext dbContext,
    LearningService learningService)
{
    public async Task<OpportunityActivityResponse?> GetAsync(Guid opportunityId, CancellationToken token)
    {
        if (!await dbContext.Opportunities.AnyAsync(x => x.Id == opportunityId, token))
            return null;
        var decisions = await dbContext.BuilderDecisions.AsNoTracking()
                .Where(x => x.OpportunityId == opportunityId)
                .ToListAsync(token);
        var outcomes = await dbContext.Outcomes.AsNoTracking()
                .Where(x => x.OpportunityId == opportunityId)
                .ToListAsync(token);
        return new(
            decisions.OrderByDescending(x => x.OccurredAt)
                .Select(x => new BuilderDecision(x.Id, x.OpportunityId,
                Enum.Parse<BuilderDecisionType>(x.DecisionType), x.Reason, x.OccurredAt)).ToArray(),
            outcomes.OrderByDescending(x => x.OccurredAt)
                .Select(x => new Outcome(x.Id, x.OpportunityId,
                Enum.Parse<OutcomeType>(x.OutcomeType), x.Note, x.OccurredAt)).ToArray());
    }

    public async Task<BuilderDecision?> DecideAsync(
        Guid opportunityId, RecordDecisionRequest request, CancellationToken token)
    {
        if (!await dbContext.Opportunities.AnyAsync(x => x.Id == opportunityId, token))
            return null;
        var item = new BuilderDecisionEntity
        {
            Id = Guid.NewGuid(), OpportunityId = opportunityId,
            DecisionType = request.DecisionType.ToString(),
            Reason = Clean(request.Reason), OccurredAt = DateTimeOffset.UtcNow,
        };
        dbContext.BuilderDecisions.Add(item);
        await dbContext.SaveChangesAsync(token);
        await learningService.RescoreAsync(opportunityId, token);
        return new(item.Id, item.OpportunityId, request.DecisionType, item.Reason, item.OccurredAt);
    }

    public async Task<Outcome?> AddOutcomeAsync(
        Guid opportunityId, RecordOutcomeRequest request, CancellationToken token)
    {
        if (!await dbContext.Opportunities.AnyAsync(x => x.Id == opportunityId, token))
            return null;
        var item = new OutcomeEntity
        {
            Id = Guid.NewGuid(), OpportunityId = opportunityId,
            OutcomeType = request.OutcomeType.ToString(),
            Note = Clean(request.Note), OccurredAt = DateTimeOffset.UtcNow,
        };
        dbContext.Outcomes.Add(item);
        await dbContext.SaveChangesAsync(token);
        await learningService.RescoreAsync(opportunityId, token);
        return new(item.Id, item.OpportunityId, request.OutcomeType, item.Note, item.OccurredAt);
    }

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
