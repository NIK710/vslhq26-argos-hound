using ArgosHound.Api.Models;

namespace ArgosHound.Api.Contracts;

public sealed class CreateCampaignLinkRequest
{
    public required string DestinationUrl { get; init; }

    public required CampaignPurpose Purpose { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }
}
