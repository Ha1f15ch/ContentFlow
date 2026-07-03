using ContentFlow.Domain.Entities;
using ContentFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentFlow.Infrastructure.DatabaseEngine.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Reports", schema: "dbo", t =>
        {
            t.HasCheckConstraint(
                "CK_Reports_Target",
                "([PostId] IS NOT NULL AND [CommentId] IS NULL) OR ([PostId] IS NULL AND [CommentId] IS NOT NULL)");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReasonType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnType("nvarchar(2000)")
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.ReporterId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Post>()
            .WithMany()
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Comment>()
            .WithMany()
            .HasForeignKey(x => x.CommentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.ReporterId, x.PostId })
            .IsUnique()
            .HasFilter("[PostId] IS NOT NULL");

        builder.HasIndex(x => new { x.ReporterId, x.CommentId })
            .IsUnique()
            .HasFilter("[CommentId] IS NOT NULL");

        builder.HasIndex(x => x.PostId);
        builder.HasIndex(x => x.CommentId);
        builder.HasIndex(x => x.ReasonType);
        builder.HasIndex(x => x.CreatedAt);
    }
}
