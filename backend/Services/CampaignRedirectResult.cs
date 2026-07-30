namespace ArgosHound.Api.Services;

public enum CampaignRedirectStatus
{
    Found,
    NotFound,
    Expired,
}

public sealed record CampaignRedirectResult(
    CampaignRedirectStatus Status,
    string? DestinationUrl = null);
