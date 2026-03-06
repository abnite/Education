using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduKidsGhana.Domain.Entities.Learning;

namespace EduKidsGhana.Infrastructure.Persistence.Configurations;

public class TopicMasteryConfiguration : IEntityTypeConfiguration<TopicMastery>
{
    public void Configure(EntityTypeBuilder<TopicMastery> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.MasteryScore).HasPrecision(5, 2);
        builder.HasIndex(e => new { e.LearnerId, e.TopicId }).IsUnique();
        builder.HasIndex(e => e.MasteryLevel);

        builder.HasOne(e => e.Learner)
            .WithMany(l => l.TopicMasteries)
            .HasForeignKey(e => e.LearnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Topic)
            .WithMany(t => t.Masteries)
            .HasForeignKey(e => e.TopicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
