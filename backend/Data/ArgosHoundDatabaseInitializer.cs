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
            """);
    }
}
