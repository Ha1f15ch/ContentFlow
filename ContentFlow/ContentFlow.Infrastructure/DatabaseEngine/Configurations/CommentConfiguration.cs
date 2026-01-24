using ContentFlow.Domain.Entities;
using ContentFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentFlow.Infrastructure.DatabaseEngine.Configurations;

public class CommentConfiguration :  IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments", schema: "dbo");
        builder.HasKey(x => x.Id);
        
        builder.Property(p => p.Content)
            .HasColumnType("nvarchar(2000)")
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();
        
        builder.Property(c => c.UpdatedAt)
            .IsRequired();
        
        builder.Property(c => c.Status)
            .HasConversion<int>()
            .IsRequired();
        
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne<Post>()
            .WithMany()
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Comment>()
            .WithMany()
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(c => c.PostId);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.ParentCommentId);
    }
}