using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsaMina.DataAccess.Configurations;

public class SavedTournamentConfiguration : IEntityTypeConfiguration<SavedTournament>
{
    public void Configure(EntityTypeBuilder<SavedTournament> builder)
    {
        builder
            .HasOne(tournament => tournament.SavedRoom)
            .WithMany(room => room.TournamentHistory)
            .HasForeignKey(tournament => tournament.RoomId);
    }
}
