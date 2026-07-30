using ArgosHound.Api.Services;
using Xunit;

namespace ArgosHound.Api.Tests;

public sealed class CampaignCodeServiceTests
{
    [Fact]
    public void GeneratesUniqueUrlSafeHighEntropyCodes()
    {
        var service = new CampaignCodeService();
        var codes = Enumerable.Range(0, 100)
            .Select(_ => service.Generate())
            .ToArray();

        Assert.Equal(codes.Length, codes.Distinct(StringComparer.Ordinal).Count());
        Assert.All(codes, code =>
        {
            Assert.Equal(43, code.Length);
            Assert.True(service.IsValidFormat(code));
            Assert.Equal(64, service.Hash(code).Length);
            Assert.DoesNotContain(code, service.Hash(code), StringComparison.Ordinal);
        });
    }
}
