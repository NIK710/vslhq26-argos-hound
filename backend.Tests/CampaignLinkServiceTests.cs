using ArgosHound.Api.Configuration;
using ArgosHound.Api.Data;
using ArgosHound.Api.Models;
using ArgosHound.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace ArgosHound.Api.Tests;

public sealed class CampaignLinkServiceTests
{
    [Fact]
    public async Task StoresOnlyHashAndRecordsPrivacyPreservingOpenedEvent()
    {
        await using var fixture = await CampaignFixture.CreateAsync();

        var created = await fixture.Service.CreateAsync(
            fixture.Opportunity.Id,
            "http://localhost:5080/demo/destination",
            CampaignPurpose.Product,
            null);

        var entity = await fixture.DbContext.CampaignLinks.SingleAsync();
        Assert.Equal(fixture.Opportunity.Id, entity.OpportunityId);
        Assert.NotEqual(created.Code, entity.CodeHash);
        Assert.Equal(fixture.CodeService.Hash(created.Code), entity.CodeHash);
        Assert.DoesNotContain(created.Code, entity.DestinationUrl);

        var redirect = await fixture.Service.OpenAsync(created.Code);

        Assert.Equal(CampaignRedirectStatus.Found, redirect.Status);
        Assert.Equal(created.Campaign.DestinationUrl, redirect.DestinationUrl);
        var engagement = await fixture.DbContext.EngagementEvents.SingleAsync();
        Assert.Equal("Opened", engagement.EventType);
        Assert.Equal("{}", engagement.MetadataJson);
    }

    [Fact]
    public async Task RejectsDestinationOutsideConfiguredHosts()
    {
        await using var fixture = await CampaignFixture.CreateAsync();

        await Assert.ThrowsAsync<ArgumentException>(
            () => fixture.Service.CreateAsync(
                fixture.Opportunity.Id,
                "https://untrusted.example.org/collect",
                CampaignPurpose.Product,
                null));

        Assert.Empty(await fixture.DbContext.CampaignLinks.ToListAsync());
    }

    [Fact]
    public void CampaignRecordsContainNoCommenterIdentityFields()
    {
        var propertyNames = typeof(CampaignLinkEntity)
            .GetProperties()
            .Select(property => property.Name)
            .Concat(typeof(EngagementEventEntity)
                .GetProperties()
                .Select(property => property.Name))
            .ToArray();

        Assert.DoesNotContain(
            propertyNames,
            name => name.Contains("author", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(
            propertyNames,
            name => name.Contains("comment", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(
            propertyNames,
            name => name.Contains("handle", StringComparison.OrdinalIgnoreCase));
    }

    private sealed class CampaignFixture : IAsyncDisposable
    {
        private CampaignFixture(
            ArgosHoundDbContext dbContext,
            Opportunity opportunity,
            CampaignCodeService codeService,
            CampaignLinkService service)
        {
            DbContext = dbContext;
            Opportunity = opportunity;
            CodeService = codeService;
            Service = service;
        }

        public ArgosHoundDbContext DbContext { get; }

        public Opportunity Opportunity { get; }

        public CampaignCodeService CodeService { get; }

        public CampaignLinkService Service { get; }

        public static async Task<CampaignFixture> CreateAsync()
        {
            var options = new DbContextOptionsBuilder<ArgosHoundDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;
            var dbContext = new ArgosHoundDbContext(options);
            await dbContext.Database.OpenConnectionAsync();
            await dbContext.Database.EnsureCreatedAsync();

            var repository = new OpportunityRepository(dbContext);
            var opportunity = CreateOpportunity();
            await repository.AddAsync(opportunity);

            var codeService = new CampaignCodeService();
            var service = new CampaignLinkService(
                repository,
                new CampaignRepository(dbContext),
                codeService,
                Options.Create(new CampaignOptions
                {
                    AllowedDestinationHosts = ["localhost", "127.0.0.1"],
                }));

            return new CampaignFixture(
                dbContext,
                opportunity,
                codeService,
                service);
        }

        public ValueTask DisposeAsync() => DbContext.DisposeAsync();

        private static Opportunity CreateOpportunity() =>
            new()
            {
                Id = Guid.NewGuid(),
                DiscussionId = Guid.NewGuid(),
                Type = OpportunityType.Product,
                ProductMatchType = ProductMatchType.Direct,
                Problem = "A validated problem.",
                ProblemInferred = false,
                Topic = "Test topic",
                Sentiment = DiscussionSentiment.Negative,
                MatchedProductId = Guid.NewGuid(),
                MatchedProductName = "Test product",
                MatchedCapabilities = ["A supplied capability"],
                Limitations = ["A limitation"],
                EvidenceReferences = ["source_external_id"],
                Explanation = "A validated explanation.",
                SuggestedAction = "Review the source.",
                Confidence = 0.8m,
                Score = 75,
                ScoreFactors = [],
                CreatedAt = DateTimeOffset.UtcNow,
            };
    }
}
