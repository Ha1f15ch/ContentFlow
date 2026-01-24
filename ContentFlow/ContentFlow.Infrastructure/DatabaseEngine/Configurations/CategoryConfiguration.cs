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
            .HasColumnType("nvarchar(100)");
        
        builder.Property(c => c.Slug)
            .IsRequired()
            .HasColumnType("varchar(100)");
        
        builder.Property(c => c.Description)
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");
        
        builder.HasIndex(c => c.Slug)
            .IsUnique();
    }
}