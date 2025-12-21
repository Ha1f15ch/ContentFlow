using ContentFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentFlow.Infrastructure.DatabaseEngine.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions", schema: "dbo");
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(s => new {s.UserProfileFollowerId, s.UserProfileFollowingId})
            .IsUnique()
            .HasDatabaseName("IX_Subscriptions_UserProfileFollowerId");

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.DeactivatedAt).IsRequired(false);
        
        builder.Property(x => x.IsPaused)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Ignore(s => s.IsActive);

        builder.Property(x => x.UserProfileFollowerId)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(x => x.NotificationsEnabled)
            .IsRequired()
            .HasDefaultValue(true);
        
        builder.HasOne<UserProfile>()
            .WithMany()
            .HasForeignKey(x => x.UserProfileFollowerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne<UserProfile>()
            .WithMany()
            .HasForeignKey(x => x.UserProfileFollowingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}