using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsaMina.DataAccess.Configurations;

public class BadgeConfiguration : IEntityTypeConfiguration<Badge>
{
    public void Configure(EntityTypeBuilder<Badge> builder)
    {
        builder
            .HasKey(badge => new { badge.Id, badge.RoomId });

        builder
            .HasOne<SavedRoom>()
            .WithMany(room => room.Badges)
            .HasForeignKey(badge => badge.RoomId);

        builder
            .HasMany(badge => badge.BadgeHolders)
            .WithOne(holding => holding.Badge)
            .HasForeignKey(badge => new { badge.BadgeId, badge.RoomId });
    }
}