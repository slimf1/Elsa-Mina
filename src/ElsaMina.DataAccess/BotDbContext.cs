using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess;

public class BotDbContext : DbContext
{
    public BotDbContext()
    {
        // Empty
    }

    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options)
    {
    }

    public DbSet<RoomUser> RoomUsers { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<AddedCommand> AddedCommands { get; set; }
    public DbSet<Room> RoomInfo { get; set; }
    public DbSet<BadgeHolding> BadgeHoldings { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<RoomTeam> RoomTeams { get; set; }
    public DbSet<ArcadeLevel> ArcadeLevels { get; set; }
    public DbSet<SavedPoll> SavedPolls { get; set; }
    public DbSet<RoomBotParameterValue> RoomBotParameterValues { get; set; }
    public DbSet<TournamentRecord> TournamentRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BotDbContext).Assembly);
    }
}