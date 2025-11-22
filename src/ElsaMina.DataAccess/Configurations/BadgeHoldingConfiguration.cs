using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsaMina.DataAccess.Configurations;

public class BadgeHoldingConfiguration : IEntityTypeConfiguration<BadgeHolding>
{
    public void Configure(EntityTypeBuilder<BadgeHolding> builder)
    {
        builder
            .HasKey(badgeHolding => new { badgeHolding.BadgeId, badgeHolding.UserId, badgeHolding.RoomId });

        builder
            .HasOne(badgeHolding => badgeHolding.Badge)
            .WithMany(badge => badge.BadgeHolders)
            .HasForeignKey(badgeHolding => new { badgeHolding.BadgeId, badgeHolding.RoomId });

        builder
            .HasOne(badgeHolding => badgeHolding.RoomUser)
            .WithMany(userData => userData.Badges)
            .HasForeignKey(badgeHolding => new { badgeHolding.UserId, badgeHolding.RoomId });
    }
}