using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduKidsGhana.Domain.Entities.Curriculum;

namespace EduKidsGhana.Infrastructure.Persistence.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Code).IsRequired().HasMaxLength(20);
        builder.Property(e => e.ColourHex).HasMaxLength(10);
        builder.Property(e => e.IconUrl).HasMaxLength(500);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.HasIndex(e => e.Code).IsUnique();
        builder.HasIndex(e => e.IsActive);
    }
}
