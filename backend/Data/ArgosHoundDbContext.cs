using Microsoft.EntityFrameworkCore;

namespace ArgosHound.Api.Data;

public sealed class ArgosHoundDbContext(
    DbContextOptions<ArgosHoundDbContext> options) : DbContext(options)
{
    public DbSet<OpportunityEntity> Opportunities => Set<OpportunityEntity>();

    public DbSet<OpportunityEvidenceEntity> OpportunityEvidence =>
        Set<OpportunityEvidenceEntity>();

    public DbSet<CampaignLinkEntity> CampaignLinks => Set<CampaignLinkEntity>();

    public DbSet<EngagementEventEntity> EngagementEvents =>
        Set<EngagementEventEntity>();
    public DbSet<BuilderDecisionEntity> BuilderDecisions => Set<BuilderDecisionEntity>();
    public DbSet<OutcomeEntity> Outcomes => Set<OutcomeEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OpportunityEntity>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => item.DiscussionId).IsUnique();
            entity.Property(item => item.Type).HasMaxLength(32);
            entity.Property(item => item.ProductMatchType).HasMaxLength(32);
            entity.Property(item => item.BuilderSubtype).HasMaxLength(32);
            entity.Property(item => item.Sentiment).HasMaxLength(32);
            entity.Property(item => item.Confidence).HasPrecision(5, 4);
            entity.HasMany(item => item.EvidenceReferences)
                .WithOne(item => item.Opportunity)
                .HasForeignKey(item => item.OpportunityId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(item => item.CampaignLinks)
                .WithOne(item => item.Opportunity)
                .HasForeignKey(item => item.OpportunityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BuilderDecisionEntity>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => new { item.OpportunityId, item.OccurredAt });
            entity.Property(item => item.DecisionType).HasMaxLength(32);
        });
        modelBuilder.Entity<OutcomeEntity>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => new { item.OpportunityId, item.OccurredAt });
            entity.Property(item => item.OutcomeType).HasMaxLength(32);
        });

        modelBuilder.Entity<OpportunityEvidenceEntity>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.ExternalId).HasMaxLength(200);
            entity.HasIndex(item => new { item.OpportunityId, item.ExternalId })
                .IsUnique();
        });

        modelBuilder.Entity<CampaignLinkEntity>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => item.CodeHash).IsUnique();
            entity.HasIndex(item => item.OpportunityId);
            entity.Property(item => item.CodeHash).HasMaxLength(64);
            entity.Property(item => item.DestinationUrl).HasMaxLength(2_000);
            entity.Property(item => item.Purpose).HasMaxLength(32);
            entity.HasMany(item => item.Events)
                .WithOne(item => item.CampaignLink)
                .HasForeignKey(item => item.CampaignLinkId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EngagementEventEntity>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => new { item.CampaignLinkId, item.OccurredAt });
            entity.Property(item => item.EventType).HasMaxLength(32);
        });
    }
}
