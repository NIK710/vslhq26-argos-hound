using System.Text.Json;
using ArgosHound.Api.Data;
using ArgosHound.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArgosHound.Api.Services;

public sealed class OpportunityRepository(
    ArgosHoundDbContext dbContext) : IOpportunityRepository
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyList<Opportunity>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        (await dbContext.Opportunities
            .AsNoTracking()
            .Include(item => item.EvidenceReferences)
            .ToListAsync(cancellationToken))
        .Select(ToDomain)
        .OrderByDescending(item => item.CreatedAt)
        .ToArray();

    public async Task<Opportunity?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Opportunities
            .AsNoTracking()
            .Include(item => item.EvidenceReferences)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Opportunity?> GetByDiscussionAsync(
        Guid discussionId,
        CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Opportunities
            .AsNoTracking()
            .Include(item => item.EvidenceReferences)
            .SingleOrDefaultAsync(
                item => item.DiscussionId == discussionId,
                cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Opportunity> AddAsync(
        Opportunity opportunity,
        CancellationToken cancellationToken = default)
    {
        var entity = ToEntity(opportunity);
        dbContext.Opportunities.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    private static OpportunityEntity ToEntity(Opportunity opportunity) =>
        new()
        {
            Id = opportunity.Id,
            DiscussionId = opportunity.DiscussionId,
            Type = opportunity.Type.ToString(),
            ProductMatchType = opportunity.ProductMatchType?.ToString(),
            Problem = opportunity.Problem,
            ProblemInferred = opportunity.ProblemInferred,
            Topic = opportunity.Topic,
            Sentiment = opportunity.Sentiment.ToString(),
            MatchedProductId = opportunity.MatchedProductId,
            MatchedProductName = opportunity.MatchedProductName,
            MatchedCapabilitiesJson = JsonSerializer.Serialize(
                opportunity.MatchedCapabilities,
                JsonOptions),
            BuilderSubtype = opportunity.BuilderSubtype?.ToString(),
            MatchedSkillsJson = JsonSerializer.Serialize(opportunity.MatchedSkills, JsonOptions),
            AdvancedGoalsJson = JsonSerializer.Serialize(opportunity.AdvancedGoals, JsonOptions),
            EffortEstimate = opportunity.EffortEstimate,
            NextStepsJson = JsonSerializer.Serialize(opportunity.NextSteps, JsonOptions),
            LimitationsJson = JsonSerializer.Serialize(
                opportunity.Limitations,
                JsonOptions),
            Explanation = opportunity.Explanation,
            SuggestedAction = opportunity.SuggestedAction,
            Confidence = opportunity.Confidence,
            Score = opportunity.Score,
            ScoreFactorsJson = JsonSerializer.Serialize(
                opportunity.ScoreFactors,
                JsonOptions),
            CreatedAt = opportunity.CreatedAt,
            EvidenceReferences = opportunity.EvidenceReferences
                .Select(externalId => new OpportunityEvidenceEntity
                {
                    Id = Guid.NewGuid(),
                    OpportunityId = opportunity.Id,
                    ExternalId = externalId,
                })
                .ToList(),
        };

    private static Opportunity ToDomain(OpportunityEntity entity) =>
        new()
        {
            Id = entity.Id,
            DiscussionId = entity.DiscussionId,
            Type = Enum.Parse<OpportunityType>(entity.Type),
            ProductMatchType = entity.ProductMatchType is null
                ? null
                : Enum.Parse<ProductMatchType>(entity.ProductMatchType),
            Problem = entity.Problem,
            ProblemInferred = entity.ProblemInferred,
            Topic = entity.Topic,
            Sentiment = Enum.Parse<DiscussionSentiment>(entity.Sentiment),
            MatchedProductId = entity.MatchedProductId,
            MatchedProductName = entity.MatchedProductName,
            MatchedCapabilities = DeserializeList(entity.MatchedCapabilitiesJson),
            BuilderSubtype = entity.BuilderSubtype is null
                ? null
                : Enum.Parse<BuilderOpportunitySubtype>(entity.BuilderSubtype),
            MatchedSkills = DeserializeList(entity.MatchedSkillsJson),
            AdvancedGoals = DeserializeList(entity.AdvancedGoalsJson),
            EffortEstimate = entity.EffortEstimate,
            NextSteps = DeserializeList(entity.NextStepsJson),
            Limitations = DeserializeList(entity.LimitationsJson),
            EvidenceReferences = entity.EvidenceReferences
                .OrderBy(item => item.ExternalId, StringComparer.Ordinal)
                .Select(item => item.ExternalId)
                .ToArray(),
            Explanation = entity.Explanation,
            SuggestedAction = entity.SuggestedAction,
            Confidence = entity.Confidence,
            Score = entity.Score,
            ScoreFactors =
                JsonSerializer.Deserialize<OpportunityScoreFactor[]>(
                    entity.ScoreFactorsJson,
                    JsonOptions)
                ?? [],
            CreatedAt = entity.CreatedAt,
        };

    private static IReadOnlyList<string> DeserializeList(string json) =>
        JsonSerializer.Deserialize<string[]>(json, JsonOptions) ?? [];
}
