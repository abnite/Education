using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduKidsGhana.Domain.Entities.Assessment;

namespace EduKidsGhana.Infrastructure.Persistence.Configurations;

public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.LearnerId);
        builder.HasIndex(e => e.QuizId);
        builder.HasIndex(e => e.StartedAt);

        builder.HasOne(e => e.Quiz)
            .WithMany(q => q.Attempts)
            .HasForeignKey(e => e.QuizId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Learner)
            .WithMany(l => l.QuizAttempts)
            .HasForeignKey(e => e.LearnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
