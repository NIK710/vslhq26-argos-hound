using Microsoft.EntityFrameworkCore;

namespace ArgosHound.Api.Data;

public sealed class ArgosHoundDbContext(
    DbContextOptions<ArgosHoundDbContext> options) : DbContext(options)
{
    public DbSet<OpportunityEntity> Opportunities => Set<OpportunityEntity>();

    public DbSet<OpportunityEvidenceEntity> OpportunityEvidence =>
        Set<OpportunityEvidenceEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OpportunityEntity>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => item.DiscussionId).IsUnique();
            entity.Property(item => item.Type).HasMaxLength(32);
            entity.Property(item => item.ProductMatchType).HasMaxLength(32);
            entity.Property(item => item.Sentiment).HasMaxLength(32);
            entity.Property(item => item.Confidence).HasPrecision(5, 4);
            entity.HasMany(item => item.EvidenceReferences)
                .WithOne(item => item.Opportunity)
                .HasForeignKey(item => item.OpportunityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OpportunityEvidenceEntity>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.ExternalId).HasMaxLength(200);
            entity.HasIndex(item => new { item.OpportunityId, item.ExternalId })
                .IsUnique();
        });
    }
}
