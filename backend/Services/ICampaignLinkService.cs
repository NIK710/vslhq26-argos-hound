namespace ArgosHound.Api.Services;

public interface ICampaignLinkService
{
    Task<CreatedCampaignLink> CreateAsync(
        Guid opportunityId,
        string destinationUrl,
        ArgosHound.Api.Models.CampaignPurpose purpose,
        DateTimeOffset? expiresAt,
        CancellationToken cancellationToken = default);

    Task<CampaignRedirectResult> OpenAsync(
        string code,
        CancellationToken cancellationToken = default);
}
