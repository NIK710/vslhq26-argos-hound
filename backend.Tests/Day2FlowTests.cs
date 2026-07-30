using ArgosHound.Api.Contracts;
using ArgosHound.Api.Data;
using ArgosHound.Api.Models;
using ArgosHound.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ArgosHound.Api.Tests;

public sealed class Day2FlowTests
{
    [Fact]
    public void BuilderAnalysisRequiresExactProfileSignalsAndMultipleNextSteps()
    {
        var discussion = DemoSourceData.Discussions.Single(
            x => x.Id == DemoSourceData.ChessClubDiscussionId);
        var context = new OpportunityAnalysisContext(
            DemoData.Builder, DemoData.Products, discussion);
        var json =
            $$"""
            {
              "problem":{"summary":"Club coordination is fragmented.","inferred":false},
              "topic":"Chess club coordination","sentiment":"NEGATIVE",
              "evidenceReferences":["{{discussion.ExternalId}}","comment_pairings"],
              "opportunityType":"BUILDER","productMatch":null,
              "builderMatch":{
                "subtype":"COMMUNITY_SERVICE",
                "matchedSkills":["C#","React"],
                "advancedGoals":["Build production-quality AI applications"],
                "effortEstimate":"Prototype in one to four weeks",
                "nextSteps":["Interview organizers","Prototype a check-in flow"]
              },
              "limitations":["Organizer requirements are not confirmed."],
              "explanation":"The builder can use existing skills while learning with real users.",
              "suggestedAction":"Interview the organizers before prototyping.",
              "confidence":0.82
            }
            """;

        var result = new OpportunityAnalysisValidator().ParseAndValidate(json, context);

        Assert.Equal(OpportunityType.Builder, result.OpportunityType);
        Assert.Equal(BuilderOpportunitySubtype.CommunityService, result.BuilderMatch!.Subtype);
    }

    [Fact]
    public async Task RecordsDecisionsAndOutcomesInOneTimeline()
    {
        var options = new DbContextOptionsBuilder<ArgosHoundDbContext>()
            .UseSqlite("Data Source=:memory:").Options;
        await using var db = new ArgosHoundDbContext(options);
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();
        var opportunityId = Guid.NewGuid();
        db.Opportunities.Add(new OpportunityEntity
        {
            Id = opportunityId, DiscussionId = Guid.NewGuid(), Type = "Builder",
            Problem = "Coordination", Topic = "Chess", Sentiment = "Negative",
            MatchedCapabilitiesJson = "[]", LimitationsJson = "[]",
            Explanation = "Fit", SuggestedAction = "Interview", Confidence = .8m,
            ScoreFactorsJson = "[]", CreatedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync();
        var service = new OpportunityActivityService(db);

        await service.DecideAsync(opportunityId,
            new RecordDecisionRequest(BuilderDecisionType.Pursued, "Strong local fit"),
            default);
        await service.AddOutcomeAsync(opportunityId,
            new RecordOutcomeRequest(OutcomeType.PrototypeCompleted, "Check-in demo"),
            default);
        var activity = await service.GetAsync(opportunityId, default);

        Assert.Single(activity!.Decisions);
        Assert.Single(activity.Outcomes);
    }
}
