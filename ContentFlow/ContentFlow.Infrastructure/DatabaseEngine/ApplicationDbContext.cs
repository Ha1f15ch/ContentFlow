using System.Reflection;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Domain.Entities;
using ContentFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContentFlow.Infrastructure.DatabaseEngine;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    // entities
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UserTwoFactorCode> UserTwoFactorCodes { get; set; }
    public DbSet<TrustedDevice> TrustedDevices { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<PostReaction> PostReactions { get; set; }
    public DbSet<CommentReaction> CommentReactions { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<ModerationCase> ModerationCases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }
}