using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduKidsGhana.Domain.Entities.Users;

namespace EduKidsGhana.Infrastructure.Persistence.Configurations;

public class LearnerProfileConfiguration : IEntityTypeConfiguration<LearnerProfile>
{
    public void Configure(EntityTypeBuilder<LearnerProfile> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.DisplayName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.AvatarCode).HasMaxLength(50);
        builder.Property(e => e.PreferredLanguage).HasMaxLength(10).HasDefaultValue("en");
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.GradeLevel);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Preference)
            .WithOne(p => p.Learner)
            .HasForeignKey<LearnerPreference>(p => p.LearnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
