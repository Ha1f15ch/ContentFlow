using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using ContentFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentFlow.Infrastructure.DatabaseEngine.Configurations;

public class ModerationCaseConfiguration : IEntityTypeConfiguration<ModerationCase>
{
    public void Configure(EntityTypeBuilder<ModerationCase> builder)
    {
        builder.ToTable("ModerationCases", schema: "dbo", t =>
        {
            t.HasCheckConstraint(
                "CK_ModerationCases_Target",
                "([PostId] IS NOT NULL AND [CommentId] IS NULL) OR ([PostId] IS NULL AND [CommentId] IS NOT NULL)");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ReportCount)
            .IsRequired();

        builder.Property(x => x.UniqueReporterCount)
            .IsRequired();

        builder.Property(x => x.FirstReportedAt)
            .IsRequired();

        builder.Property(x => x.LastReportedAt)
            .IsRequired();

        builder.Property(x => x.Decision)
            .HasConversion<int>()
            .IsRequired(false);

        builder.Property(x => x.ModeratorNote)
            .HasColumnType("nvarchar(2000)")
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne<Post>()
            .WithMany()
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Comment>()
            .WithMany()
            .HasForeignKey(x => x.CommentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.AssignedModeratorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.ResolvedById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.PostId)
            .IsUnique()
            .HasFilter($"[PostId] IS NOT NULL AND [Status] IN ({(int)ModerationCaseStatus.Open}, {(int)ModerationCaseStatus.InReview})");

        builder.HasIndex(x => x.CommentId)
            .IsUnique()
            .HasFilter($"[CommentId] IS NOT NULL AND [Status] IN ({(int)ModerationCaseStatus.Open}, {(int)ModerationCaseStatus.InReview})");

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Priority);
        builder.HasIndex(x => x.LastReportedAt);
        builder.HasIndex(x => x.AssignedModeratorId);
    }
}
