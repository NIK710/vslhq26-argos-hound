using ArgosHound.Api.Data;
using ArgosHound.Api.Models;
using ArgosHound.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ArgosHound.Api.Tests;

public sealed class DiscoveryServiceTests
{
    [Fact]
    public async Task PersistsEvidenceAndIsIdempotentPerDiscussion()
    {
        var options = new DbContextOptionsBuilder<ArgosHoundDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        await using var dbContext = new ArgosHoundDbContext(options);
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var provider = new StubAnalysisProvider(
            OpportunityScoringServiceTests.CreateAnalysis(0.8m));
        var repository = new OpportunityRepository(dbContext);
        var service = new DiscoveryService(
            new InMemoryBuilderProfileStore(),
            new DemoProductCatalog(),
            new InMemorySourceDiscussionService(),
            provider,
            new OpportunityScoringService(),
            repository);

        var first = await service.DiscoverAsync(
            DemoSourceData.DoomscrollingDiscussionId);
        var second = await service.DiscoverAsync(
            DemoSourceData.DoomscrollingDiscussionId);

        Assert.Equal(first.Id, second.Id);
        Assert.Equal(1, provider.CallCount);
        Assert.Equal(1, await dbContext.Opportunities.CountAsync());
        Assert.Equal(
            first.EvidenceReferences.Count,
            await dbContext.OpportunityEvidence.CountAsync());
        Assert.Equal(DemoData.Products[0].Id, first.MatchedProductId);
        Assert.Contains(
            "Interrupt infinite-scroll behavior",
            first.MatchedCapabilities);
    }

    private sealed class StubAnalysisProvider(
        OpportunityAnalysis analysis) : ILlmAnalysisProvider
    {
        public int CallCount { get; private set; }

        public Task<OpportunityAnalysis> AnalyzeAsync(
            OpportunityAnalysisContext context,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(analysis);
        }
    }
}
