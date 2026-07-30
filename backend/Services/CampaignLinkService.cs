using ArgosHound.Api.Configuration;
using ArgosHound.Api.Models;
using Microsoft.Extensions.Options;

namespace ArgosHound.Api.Services;

public sealed class CampaignLinkService(
    IOpportunityRepository opportunityRepository,
    ICampaignRepository campaignRepository,
    ICampaignCodeService codeService,
    IOptions<CampaignOptions> options) : ICampaignLinkService
{
    private readonly HashSet<string> _allowedHosts =
        options.Value.AllowedDestinationHosts.ToHashSet(
            StringComparer.OrdinalIgnoreCase);

    public async Task<CreatedCampaignLink> CreateAsync(
        Guid opportunityId,
        string destinationUrl,
        CampaignPurpose purpose,
        DateTimeOffset? expiresAt,
        CancellationToken cancellationToken = default)
    {
        if (await opportunityRepository.GetAsync(
                opportunityId,
                cancellationToken) is null)
        {
            throw new KeyNotFoundException("Opportunity was not found.");
        }

        var normalizedDestination = ValidateDestination(destinationUrl);
        if (expiresAt is not null && expiresAt <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentException("expiresAt must be in the future.");
        }

        var code = codeService.Generate();
        var campaign = new CampaignLink
        {
            Id = Guid.NewGuid(),
            OpportunityId = opportunityId,
            DestinationUrl = normalizedDestination,
            Purpose = purpose,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = expiresAt,
            Events = [],
        };

        var persisted = await campaignRepository.AddAsync(
            campaign,
            codeService.Hash(code),
            cancellationToken);
        return new CreatedCampaignLink(persisted, code);
    }

    public Task<CampaignRedirectResult> OpenAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        if (!codeService.IsValidFormat(code))
        {
            return Task.FromResult(
                new CampaignRedirectResult(CampaignRedirectStatus.NotFound));
        }

        return campaignRepository.RecordOpenedAsync(
            codeService.Hash(code),
            DateTimeOffset.UtcNow,
            cancellationToken);
    }

    private string ValidateDestination(string destinationUrl)
    {
        if (!Uri.TryCreate(destinationUrl, UriKind.Absolute, out var destination)
            || (destination.Scheme != Uri.UriSchemeHttps
                && destination.Scheme != Uri.UriSchemeHttp))
        {
            throw new ArgumentException(
                "destinationUrl must be an absolute HTTP or HTTPS URL.");
        }

        if (!_allowedHosts.Contains(destination.IdnHost))
        {
            throw new ArgumentException(
                "The destination host is not allowlisted.");
        }

        var isLocal = destination.IdnHost.Equals(
                "localhost",
                StringComparison.OrdinalIgnoreCase)
            || destination.IdnHost == "127.0.0.1";
        if (destination.Scheme != Uri.UriSchemeHttps && !isLocal)
        {
            throw new ArgumentException(
                "Non-local campaign destinations must use HTTPS.");
        }

        return destination.AbsoluteUri;
    }
}
