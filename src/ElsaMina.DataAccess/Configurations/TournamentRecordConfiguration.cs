using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsaMina.DataAccess.Configurations;

public class TournamentRecordConfiguration : IEntityTypeConfiguration<TournamentRecord>
{
    public void Configure(EntityTypeBuilder<TournamentRecord> builder)
    {
        builder
            .HasKey(record => new { record.UserId, record.RoomId });

        builder
            .HasOne(record => record.RoomUser)
            .WithOne(user => user.TournamentRecord)
            .HasForeignKey<TournamentRecord>(record => new { record.UserId, record.RoomId });
    }
}