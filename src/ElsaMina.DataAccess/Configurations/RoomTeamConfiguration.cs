using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsaMina.DataAccess.Configurations;

public class RoomTeamConfiguration : IEntityTypeConfiguration<RoomTeam>
{
    public void Configure(EntityTypeBuilder<RoomTeam> builder)
    {
        builder
            .HasKey(roomTeam => new { roomTeam.TeamId, roomTeam.RoomId });

        builder
            .HasOne(roomTeam => roomTeam.Room)
            .WithMany(roomParameters => roomParameters.Teams)
            .HasForeignKey(roomTeam => roomTeam.RoomId);

        builder
            .HasOne(roomTeam => roomTeam.Team)
            .WithMany(team => team.Rooms)
            .HasForeignKey(roomTeam => roomTeam.TeamId);
    }
}