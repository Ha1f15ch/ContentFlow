using ContentFlow.Domain.Entities;
using ContentFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentFlow.Infrastructure.DatabaseEngine.Configurations;

public class CommentReactionConfiguration : IEntityTypeConfiguration<CommentReaction>
{
    public void Configure(EntityTypeBuilder<CommentReaction> builder)
    {
        builder.ToTable("CommentReactions", schema: "dbo");
        builder.HasKey(pk => pk.Id);
        
        builder.Property(x => x.ReactionType)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne<Comment>()
            .WithMany()
            .HasForeignKey(x => x.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.Property(c => c.UpdatedAt)
            .IsRequired();
        
        builder.HasIndex(c => new { c.CommentId, c.UserId }).IsUnique();
        builder.HasIndex(c => c.CommentId);
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => new { c.CommentId, c.ReactionType });
    }
}