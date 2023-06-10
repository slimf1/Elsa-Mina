using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Utils;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess;

public class BotDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RoomSpecificUserData> UserData { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<AddedCommand> AddedCommands { get; set; }
    public DbSet<RoomParameters> RoomParameters { get; set; }
    public DbSet<BadgeHolding> BadgeHoldings { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<RoomTeam> RoomTeams { get; set; }

    public BotDbContext()
    {
    }

    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AddedCommand>()
            .HasKey(command => new { command.Id, command.RoomId });
        modelBuilder.Entity<Badge>()
            .HasKey(badge => new { badge.Id, badge.RoomId });

        modelBuilder.Entity<BadgeHolding>()
            .HasKey(badgeHolding => new { badgeHolding.BadgeId, badgeHolding.UserId, badgeHolding.RoomId });
        
        modelBuilder.Entity<BadgeHolding>()
            .HasOne(badgeHolding => badgeHolding.Badge)
            .WithMany(badge => badge.BadgeHolders)
            .HasForeignKey(badgeHolding => new { badgeHolding.BadgeId, badgeHolding.RoomId });

        modelBuilder.Entity<BadgeHolding>()
            .HasOne(badgeHolding => badgeHolding.RoomSpecificUserData)
            .WithMany(userData => userData.Badges)
            .HasForeignKey(badgeHolding => new { badgeHolding.UserId, badgeHolding.RoomId });
        
        modelBuilder.Entity<RoomSpecificUserData>()
            .HasKey(userData => new { userData.Id, userData.RoomId });

        modelBuilder.Entity<RoomTeam>()
            .HasKey(roomTeam => new { Id = roomTeam.TeamId, roomTeam.RoomId });

        modelBuilder.Entity<RoomTeam>()
            .HasOne(roomTeam => roomTeam.RoomParameters)
            .WithMany(roomParameters => roomParameters.Teams)
            .HasForeignKey(roomTeam => roomTeam.RoomId);

        modelBuilder.Entity<RoomTeam>()
            .HasOne(roomTeam => roomTeam.Team)
            .WithMany(team => team.Rooms)
            .HasForeignKey(roomTeam => roomTeam.TeamId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        if (!optionsBuilder.IsConfigured)
        {
            var dbConfig = DbConfigProvider.GetDbConfig();
            optionsBuilder.UseNpgsql(dbConfig.ConnectionString);
        }
    }
}