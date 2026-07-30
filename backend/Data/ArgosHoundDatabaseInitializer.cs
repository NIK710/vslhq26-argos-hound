using Microsoft.EntityFrameworkCore;

namespace ArgosHound.Api.Data;

public static class ArgosHoundDatabaseInitializer
{
    public static void Initialize(ArgosHoundDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        // EnsureCreated does not evolve an existing MVP database. These idempotent
        // statements preserve Step 6 databases until formal migrations are introduced.
        dbContext.Database.ExecuteSqlRaw(
            """
            CREATE TABLE IF NOT EXISTS "CampaignLinks" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_CampaignLinks" PRIMARY KEY,
                "OpportunityId" TEXT NOT NULL,
                "CodeHash" TEXT NOT NULL,
                "DestinationUrl" TEXT NOT NULL,
                "Purpose" TEXT NOT NULL,
                "CreatedAt" TEXT NOT NULL,
                "ExpiresAt" TEXT NULL,
                CONSTRAINT "FK_CampaignLinks_Opportunities_OpportunityId"
                    FOREIGN KEY ("OpportunityId") REFERENCES "Opportunities" ("Id")
                    ON DELETE CASCADE
            );
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_CampaignLinks_CodeHash"
                ON "CampaignLinks" ("CodeHash");
            CREATE INDEX IF NOT EXISTS "IX_CampaignLinks_OpportunityId"
                ON "CampaignLinks" ("OpportunityId");

            CREATE TABLE IF NOT EXISTS "EngagementEvents" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_EngagementEvents" PRIMARY KEY,
                "CampaignLinkId" TEXT NOT NULL,
                "EventType" TEXT NOT NULL,
                "OccurredAt" TEXT NOT NULL,
                "MetadataJson" TEXT NOT NULL,
                CONSTRAINT "FK_EngagementEvents_CampaignLinks_CampaignLinkId"
                    FOREIGN KEY ("CampaignLinkId") REFERENCES "CampaignLinks" ("Id")
                    ON DELETE CASCADE
            );
            CREATE INDEX IF NOT EXISTS "IX_EngagementEvents_CampaignLinkId_OccurredAt"
                ON "EngagementEvents" ("CampaignLinkId", "OccurredAt");

            CREATE TABLE IF NOT EXISTS "BuilderDecisions" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_BuilderDecisions" PRIMARY KEY,
                "OpportunityId" TEXT NOT NULL, "DecisionType" TEXT NOT NULL,
                "Reason" TEXT NULL, "OccurredAt" TEXT NOT NULL,
                CONSTRAINT "FK_BuilderDecisions_Opportunities_OpportunityId"
                    FOREIGN KEY ("OpportunityId") REFERENCES "Opportunities" ("Id") ON DELETE CASCADE
            );
            CREATE INDEX IF NOT EXISTS "IX_BuilderDecisions_OpportunityId_OccurredAt"
                ON "BuilderDecisions" ("OpportunityId", "OccurredAt");
            CREATE TABLE IF NOT EXISTS "Outcomes" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_Outcomes" PRIMARY KEY,
                "OpportunityId" TEXT NOT NULL, "OutcomeType" TEXT NOT NULL,
                "Note" TEXT NULL, "OccurredAt" TEXT NOT NULL,
                CONSTRAINT "FK_Outcomes_Opportunities_OpportunityId"
                    FOREIGN KEY ("OpportunityId") REFERENCES "Opportunities" ("Id") ON DELETE CASCADE
            );
            CREATE INDEX IF NOT EXISTS "IX_Outcomes_OpportunityId_OccurredAt"
                ON "Outcomes" ("OpportunityId", "OccurredAt");
            """);

        AddColumn(dbContext, "Opportunities", "BuilderSubtype", "TEXT NULL");
        AddColumn(dbContext, "Opportunities", "MatchedSkillsJson", "TEXT NOT NULL DEFAULT '[]'");
        AddColumn(dbContext, "Opportunities", "AdvancedGoalsJson", "TEXT NOT NULL DEFAULT '[]'");
        AddColumn(dbContext, "Opportunities", "EffortEstimate", "TEXT NULL");
        AddColumn(dbContext, "Opportunities", "NextStepsJson", "TEXT NOT NULL DEFAULT '[]'");
    }

#pragma warning disable EF1002 // Table and column inputs are private, fixed application constants.
    private static void AddColumn(
        ArgosHoundDbContext dbContext, string table, string column, string definition)
    {
        var exists = dbContext.Database.SqlQueryRaw<int>(
            $"SELECT COUNT(*) AS Value FROM pragma_table_info('{table}') WHERE name = '{column}'")
            .AsEnumerable().Single() > 0;
        if (!exists)
            dbContext.Database.ExecuteSqlRaw($"ALTER TABLE \"{table}\" ADD COLUMN \"{column}\" {definition}");
    }
#pragma warning restore EF1002
}
