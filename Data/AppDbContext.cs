using Microsoft.EntityFrameworkCore;
using SceneBuilderApi.Models;

namespace SceneBuilderApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Story> Stories => Set<Story>();
    public DbSet<Scene> Scenes => Set<Scene>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        modelBuilder.Entity<Story>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Stories)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Scene>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).HasColumnType("nvarchar(max)");
            entity.HasOne(e => e.Story)
                  .WithMany(s => s.Scenes)
                  .HasForeignKey(e => e.StoryId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
