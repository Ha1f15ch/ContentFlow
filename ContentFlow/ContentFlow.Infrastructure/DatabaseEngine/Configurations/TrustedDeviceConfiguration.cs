using ContentFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentFlow.Infrastructure.DatabaseEngine.Configurations;

public class TrustedDeviceConfiguration : IEntityTypeConfiguration<TrustedDevice>
{
    public void Configure(EntityTypeBuilder<TrustedDevice> builder)
    {
        builder.ToTable("TrustedDevic", schema: "security");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeviceId)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("getdate()");
        
        builder.Property(x => x.LastUsedAt)
            .IsRequired()
            .HasDefaultValueSql("getdate()");
        
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasIndex(x => new  { x.UserId, x.DeviceId }).IsUnique();
        builder.HasIndex(x => x.ExpiresAt);
    }
}