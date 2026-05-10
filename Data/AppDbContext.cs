using Microsoft.EntityFrameworkCore;
using SceneBuilderApi.Models;

namespace SceneBuilderApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Universe> Universes => Set<Universe>();
    public DbSet<Story> Stories => Set<Story>();
    public DbSet<Scene> Scenes => Set<Scene>();
    public DbSet<EntityType> EntityTypes => Set<EntityType>();
    public DbSet<EntityTypeField> EntityTypeFields => Set<EntityTypeField>();
    public DbSet<Entity> Entities => Set<Entity>();
    public DbSet<EntityFieldValue> EntityFieldValues => Set<EntityFieldValue>();
    public DbSet<EntityFieldRefValue> EntityFieldRefValues => Set<EntityFieldRefValue>();
    public DbSet<RelationshipType> RelationshipTypes => Set<RelationshipType>();
    public DbSet<EntityRelationship> EntityRelationships => Set<EntityRelationship>();
    public DbSet<Calendar> Calendars => Set<Calendar>();
    public DbSet<CalendarUnit> CalendarUnits => Set<CalendarUnit>();
    public DbSet<CalendarEra> CalendarEras => Set<CalendarEra>();
    public DbSet<CalendarSpecialDate> CalendarSpecialDates => Set<CalendarSpecialDate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).IsRequired().HasMaxLength(256);
            e.Property(u => u.DisplayName).IsRequired().HasMaxLength(100);
            e.Property(u => u.Role).HasMaxLength(50);
        });

        modelBuilder.Entity<Universe>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Name).IsRequired().HasMaxLength(200);
            e.HasOne(u => u.User)
             .WithMany(u => u.Universes)
             .HasForeignKey(u => u.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Story>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Title).IsRequired().HasMaxLength(200);
            e.HasOne(s => s.User)
             .WithMany(u => u.Stories)
             .HasForeignKey(s => s.UserId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(s => s.Universe)
             .WithMany(u => u.Stories)
             .HasForeignKey(s => s.UniverseId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Scene>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Title).IsRequired().HasMaxLength(200);
            e.Property(s => s.Content).HasColumnType("nvarchar(max)");
            e.HasOne(s => s.Story)
             .WithMany(s => s.Scenes)
             .HasForeignKey(s => s.StoryId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EntityType>(e =>
        {
            e.HasKey(et => et.Id);
            e.Property(et => et.Name).IsRequired().HasMaxLength(100);
            e.HasOne(et => et.User)
             .WithMany()
             .HasForeignKey(et => et.UserId)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired(false);
        });

        modelBuilder.Entity<EntityTypeField>(e =>
        {
            e.HasKey(f => f.Id);
            e.Property(f => f.Key).IsRequired().HasMaxLength(100);
            e.Property(f => f.Label).IsRequired().HasMaxLength(200);
            e.Property(f => f.ValueType).IsRequired().HasMaxLength(50);
            e.HasOne(f => f.EntityType)
             .WithMany(et => et.Fields)
             .HasForeignKey(f => f.EntityTypeId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(f => f.RefEntityType)
             .WithMany()
             .HasForeignKey(f => f.RefEntityTypeId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);
        });

        modelBuilder.Entity<Entity>(e =>
        {
            e.HasKey(en => en.Id);
            e.Property(en => en.Name).IsRequired().HasMaxLength(200);
            e.HasOne(en => en.User)
             .WithMany(u => u.Entities)
             .HasForeignKey(en => en.UserId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(en => en.Universe)
             .WithMany(u => u.Entities)
             .HasForeignKey(en => en.UniverseId)
             .OnDelete(DeleteBehavior.SetNull)
             .IsRequired(false);
            e.HasOne(en => en.EntityType)
             .WithMany(et => et.Entities)
             .HasForeignKey(en => en.EntityTypeId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EntityFieldValue>(e =>
        {
            e.HasKey(v => v.Id);
            e.HasOne(v => v.Entity)
             .WithMany(en => en.FieldValues)
             .HasForeignKey(v => v.EntityId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(v => v.Field)
             .WithMany()
             .HasForeignKey(v => v.FieldId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EntityFieldRefValue>(e =>
        {
            e.HasKey(v => v.Id);
            e.HasOne(v => v.Entity)
             .WithMany(en => en.FieldRefValues)
             .HasForeignKey(v => v.EntityId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(v => v.Field)
             .WithMany()
             .HasForeignKey(v => v.FieldId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(v => v.RefEntity)
             .WithMany()
             .HasForeignKey(v => v.RefEntityId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RelationshipType>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Name).IsRequired().HasMaxLength(100);
            e.HasOne(r => r.User)
             .WithMany()
             .HasForeignKey(r => r.UserId)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired(false);
        });

        modelBuilder.Entity<EntityRelationship>(e =>
        {
            e.HasKey(r => r.Id);
            e.HasOne(r => r.FromEntity)
             .WithMany(en => en.OutgoingRelationships)
             .HasForeignKey(r => r.FromEntityId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.ToEntity)
             .WithMany(en => en.IncomingRelationships)
             .HasForeignKey(r => r.ToEntityId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.RelationshipType)
             .WithMany(rt => rt.Relationships)
             .HasForeignKey(r => r.RelationshipTypeId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Calendar>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).IsRequired().HasMaxLength(200);
            e.Property(c => c.BaseUnitName).IsRequired().HasMaxLength(100);
            e.HasOne(c => c.User)
             .WithMany(u => u.Calendars)
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(c => c.Universe)
             .WithMany(u => u.Calendars)
             .HasForeignKey(c => c.UniverseId)
             .OnDelete(DeleteBehavior.SetNull)
             .IsRequired(false);
        });

        modelBuilder.Entity<CalendarUnit>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Name).IsRequired().HasMaxLength(100);
            e.Property(u => u.Direction).IsRequired().HasMaxLength(50);
            e.Property(u => u.Type).IsRequired().HasMaxLength(50);
            e.HasOne(u => u.Calendar)
             .WithMany(c => c.Units)
             .HasForeignKey(u => u.CalendarId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(u => u.ParentUnit)
             .WithMany(u => u.ChildUnits)
             .HasForeignKey(u => u.ParentUnitId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);
        });

        modelBuilder.Entity<CalendarEra>(e =>
        {
            e.HasKey(era => era.Id);
            e.Property(era => era.Name).IsRequired().HasMaxLength(200);
            e.HasOne(era => era.Calendar)
             .WithMany(c => c.Eras)
             .HasForeignKey(era => era.CalendarId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CalendarSpecialDate>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.Name).IsRequired().HasMaxLength(200);
            e.HasOne(d => d.Calendar)
             .WithMany(c => c.SpecialDates)
             .HasForeignKey(d => d.CalendarId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
