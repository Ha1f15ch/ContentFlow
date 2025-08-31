using ContentFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentFlow.Infrastructure.DatabaseEngine.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags", schema: "dict");
        builder.HasKey(p => p.Id);
        
        builder.Property(t => t.Name)
            .IsRequired()
            .HasColumnType("text")
            .HasMaxLength(50);
        
        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);
        
        builder.HasMany(t => t.PostTags)
            .WithOne(pt => pt.Tag)
            .HasForeignKey(pt => pt.TagId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(t => t.Slug)
            .IsUnique();
    }
}