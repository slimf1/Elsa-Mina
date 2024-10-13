using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess;

public class BotDbContext : DbContext
{
    public BotDbContext()
    {
    }

    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RoomSpecificUserData> UserData { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<AddedCommand> AddedCommands { get; set; }
    public DbSet<RoomParameters> RoomParameters { get; set; }
    public DbSet<BadgeHolding> BadgeHoldings { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<RoomTeam> RoomTeams { get; set; }
    public DbSet<Repeat> Repeats { get; set; }

    public string ConnectionString { get; set; }

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
            .HasKey(roomTeam => new { roomTeam.TeamId, roomTeam.RoomId });

        modelBuilder.Entity<RoomTeam>()
            .HasOne(roomTeam => roomTeam.RoomParameters)
            .WithMany(roomParameters => roomParameters.Teams)
            .HasForeignKey(roomTeam => roomTeam.RoomId);

        modelBuilder.Entity<RoomTeam>()
            .HasOne(roomTeam => roomTeam.Team)
            .WithMany(team => team.Rooms)
            .HasForeignKey(roomTeam => roomTeam.TeamId);

        modelBuilder.Entity<Repeat>()
            .HasKey(repeat => new { repeat.RoomId, repeat.Name });

        modelBuilder.Entity<RoomBotParameterValue>()
            .HasKey(roomBotParameterValue => new { roomBotParameterValue.RoomId, roomBotParameterValue.ParameterId });

        modelBuilder.Entity<RoomBotParameterValue>()
            .HasOne(roomBotParameterValue => roomBotParameterValue.RoomParameters)
            .WithMany(roomsParameters => roomsParameters.ParameterValues)
            .HasForeignKey(roomBotParameterValue => roomBotParameterValue.RoomId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        optionsBuilder.UseNpgsql(ConnectionString);
    }
}