using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess;

public class BotDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RoomSpecificUserData> UserData { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<AddedCommand> AddedCommands { get; set; }

    public BotDbContext()
    {
        Database.EnsureCreated();
    }

    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<AddedCommand>().ToTable("AddedCommands");
        modelBuilder.Entity<Badge>().ToTable("Badges");
        modelBuilder.Entity<RoomSpecificUserData>().ToTable("RoomSpecificUserData");
        modelBuilder.Entity<User>().ToTable("Users");
        
        modelBuilder.Entity<RoomSpecificUserData>()
            .HasMany(userData => userData.Badges)
            .WithMany(badge => badge.BadgeHolders)
            .UsingEntity(builder => builder.ToTable("BadgeHoldings"));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=mytestdatabase.db");
        }
    }
}