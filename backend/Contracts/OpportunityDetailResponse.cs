using ArgosHound.Api.Models;

namespace ArgosHound.Api.Contracts;

public sealed record OpportunityDetailResponse(
    Opportunity Opportunity,
    SourceDiscussion Source,
    IReadOnlyList<SourceComment> RelevantComments,
    IReadOnlyList<CampaignLink> Campaigns,
    OpportunityActivityResponse Activity);
