using ArgosHound.Api.Models;

namespace ArgosHound.Api.Contracts;

public sealed record CreateCampaignLinkResponse(
    CampaignLink Campaign,
    string RedirectUrl);
