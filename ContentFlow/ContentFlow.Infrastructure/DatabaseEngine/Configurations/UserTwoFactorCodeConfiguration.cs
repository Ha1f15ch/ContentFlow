using ContentFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentFlow.Infrastructure.DatabaseEngine.Configurations;

public class UserTwoFactorCodeConfiguration : IEntityTypeConfiguration<UserTwoFactorCode>
{
    public void Configure(EntityTypeBuilder<UserTwoFactorCode> builder)
    {
        builder.ToTable("UserCode", schema: "security");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.CodeHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.CodeSalt)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Purpose)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("getdate()");
        
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasIndex(x => new { x.UserId, x.Purpose, x.IsUsed });
        builder.HasIndex(x => x.ExpiresAt);
    }
}