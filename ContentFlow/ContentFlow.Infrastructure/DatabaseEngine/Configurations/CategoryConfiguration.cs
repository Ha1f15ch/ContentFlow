using ContentFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentFlow.Infrastructure.DatabaseEngine.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", schema: "dict");
        builder.HasKey(x => x.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasColumnType("text")
            .HasMaxLength(100);
        
        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);
        
        builder.Property(c => c.Description)
            .IsRequired(false)
            .HasColumnType("text")
            .HasMaxLength(500);
        
        builder.HasIndex(c => c.Slug)
            .IsUnique();
    }
}