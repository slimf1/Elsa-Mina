﻿using ElsaMina.DataAccess.Models;
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
        modelBuilder.Entity<RoomSpecificUserData>()
            .HasMany(userData => userData.Badges)
            .WithMany(badge => badge.BadgeHolders)
            .UsingEntity(builder => builder.ToTable("BadgeHoldings"))
            .HasKey(userData => new { userData.Id, userData.RoomId });
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