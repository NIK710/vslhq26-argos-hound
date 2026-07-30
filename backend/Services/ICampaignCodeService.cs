namespace ArgosHound.Api.Services;

public interface ICampaignCodeService
{
    string Generate();

    string Hash(string code);

    bool IsValidFormat(string code);
}
