namespace ArgosHound.Api.Configuration;

public sealed class CampaignOptions
{
    public const string SectionName = "Campaign";

    public string PublicBaseUrl { get; init; } = "http://localhost:5080";

    public IReadOnlyList<string> AllowedDestinationHosts { get; init; } =
        ["localhost", "127.0.0.1"];
}
