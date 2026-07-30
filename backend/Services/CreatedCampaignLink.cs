using ArgosHound.Api.Models;

namespace ArgosHound.Api.Services;

public sealed record CreatedCampaignLink(
    CampaignLink Campaign,
    string Code);
