using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ArgosHound.Api.Services;

public sealed partial class CampaignCodeService : ICampaignCodeService
{
    private const int RandomByteCount = 32;

    public string Generate()
    {
        Span<byte> bytes = stackalloc byte[RandomByteCount];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public string Hash(string code) =>
        Convert.ToHexStringLower(
            SHA256.HashData(Encoding.UTF8.GetBytes(code)));

    public bool IsValidFormat(string code) =>
        CampaignCodePattern().IsMatch(code);

    [GeneratedRegex("^[A-Za-z0-9_-]{43}$", RegexOptions.CultureInvariant)]
    private static partial Regex CampaignCodePattern();
}
