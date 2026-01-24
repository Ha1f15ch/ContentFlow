using ContentFlow.Domain.Entities;
using ContentFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentFlow.Infrastructure.DatabaseEngine.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles", schema: "dbo");
        
        builder.HasKey(x => x.Id);
        
        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<UserProfile>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // configuration properties
        builder.Property(up => up.FirstName).HasMaxLength(200);
        builder.Property(up => up.LastName).HasMaxLength(200);
        builder.Property(up => up.MiddleName).HasMaxLength(200);
        builder.Property(up => up.City).HasMaxLength(500);
        builder.Property(up => up.Bio).HasMaxLength(2000);
        builder.Property(up => up.AvatarUrl).HasMaxLength(2000);
    }
}