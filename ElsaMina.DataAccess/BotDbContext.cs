using System.Reflection;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess;

public class BotDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RoomSpecificUserData> UserData { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<AddedCommand> AddedCommands { get; set; }
    public DbSet<RoomParameters> RoomParameters { get; set; }

    public BotDbContext()
    {
    }

    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
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
            var dbPath = Environment.GetEnvironmentVariable("ELSA_MINA_DB_PATH");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}