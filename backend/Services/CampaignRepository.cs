using System.Text.Json;
using ArgosHound.Api.Data;
using ArgosHound.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArgosHound.Api.Services;

public sealed class CampaignRepository(
    ArgosHoundDbContext dbContext) : ICampaignRepository
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public async Task<CampaignLink> AddAsync(
        CampaignLink campaign,
        string codeHash,
        CancellationToken cancellationToken = default)
    {
        var entity = new CampaignLinkEntity
        {
            Id = campaign.Id,
            OpportunityId = campaign.OpportunityId,
            CodeHash = codeHash,
            DestinationUrl = campaign.DestinationUrl,
            Purpose = campaign.Purpose.ToString(),
            CreatedAt = campaign.CreatedAt,
            ExpiresAt = campaign.ExpiresAt,
        };

        dbContext.CampaignLinks.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    public async Task<IReadOnlyList<CampaignLink>> GetForOpportunityAsync(
        Guid opportunityId,
        CancellationToken cancellationToken = default) =>
        (await dbContext.CampaignLinks
            .AsNoTracking()
            .Include(item => item.Events)
            .Where(item => item.OpportunityId == opportunityId)
            .ToListAsync(cancellationToken))
        .Select(ToDomain)
        .OrderByDescending(item => item.CreatedAt)
        .ToArray();

    public async Task<CampaignRedirectResult> RecordOpenedAsync(
        string codeHash,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken = default)
    {
        var campaign = await dbContext.CampaignLinks.SingleOrDefaultAsync(
            item => item.CodeHash == codeHash,
            cancellationToken);
        if (campaign is null)
        {
            return new CampaignRedirectResult(CampaignRedirectStatus.NotFound);
        }

        if (campaign.ExpiresAt is not null
            && campaign.ExpiresAt <= occurredAt)
        {
            return new CampaignRedirectResult(CampaignRedirectStatus.Expired);
        }

        dbContext.EngagementEvents.Add(new EngagementEventEntity
        {
            Id = Guid.NewGuid(),
            CampaignLinkId = campaign.Id,
            EventType = EngagementEventType.Opened.ToString(),
            OccurredAt = occurredAt,
            MetadataJson = "{}",
        });
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CampaignRedirectResult(
            CampaignRedirectStatus.Found,
            campaign.DestinationUrl);
    }

    private static CampaignLink ToDomain(CampaignLinkEntity entity) =>
        new()
        {
            Id = entity.Id,
            OpportunityId = entity.OpportunityId,
            DestinationUrl = entity.DestinationUrl,
            Purpose = Enum.Parse<CampaignPurpose>(entity.Purpose),
            CreatedAt = entity.CreatedAt,
            ExpiresAt = entity.ExpiresAt,
            Events = entity.Events
                .Select(eventEntity => new EngagementEvent
                {
                    Id = eventEntity.Id,
                    CampaignLinkId = eventEntity.CampaignLinkId,
                    EventType =
                        Enum.Parse<EngagementEventType>(eventEntity.EventType),
                    OccurredAt = eventEntity.OccurredAt,
                    Metadata =
                        JsonSerializer.Deserialize<Dictionary<string, string>>(
                            eventEntity.MetadataJson,
                            JsonOptions)
                        ?? new Dictionary<string, string>(),
                })
                .OrderByDescending(item => item.OccurredAt)
                .ToArray(),
        };
}
