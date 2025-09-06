using ContentFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentFlow.Infrastructure.DatabaseEngine.Configurations;

public class RefreshTokenConfiguration :  IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", schema: "security");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(x => x.TokenSalt)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(x => x.CreatedByIp)
            .HasMaxLength(100);
        
        builder.Property(x => x.RevokedByIp)
            .HasMaxLength(100);
        
        builder.Property(x => x.ReplacedByTokenHash)
            .HasMaxLength(500);
        
        builder.Property(x => x.DeviceId)
            .HasMaxLength(200);
        
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ExpiresAt);
    }
}