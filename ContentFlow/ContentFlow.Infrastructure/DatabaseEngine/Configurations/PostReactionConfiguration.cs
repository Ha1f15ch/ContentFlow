using ContentFlow.Domain.Entities;
using ContentFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentFlow.Infrastructure.DatabaseEngine.Configurations;

public class PostReactionConfiguration : IEntityTypeConfiguration<PostReaction>
{
    public void Configure(EntityTypeBuilder<PostReaction> builder)
    {
        builder.ToTable("PostReactions", schema: "dbo");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ReactionType)
            .HasConversion<int>()
            .IsRequired();
        
        builder.HasOne<Post>()
            .WithMany()
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(c => c.CreatedAt)
            .IsRequired();
        
        builder.Property(c => c.UpdatedAt)
            .IsRequired();
        
        builder.HasIndex(c => new { c.PostId, c.UserId }).IsUnique();
        builder.HasIndex(c => c.PostId);
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => new { c.PostId, c.ReactionType });
    }
}