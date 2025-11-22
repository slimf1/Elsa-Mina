using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsaMina.DataAccess.Configurations;

public class RoomBotParameterValueConfiguration : IEntityTypeConfiguration<RoomBotParameterValue>
{
    public void Configure(EntityTypeBuilder<RoomBotParameterValue> builder)
    {
        builder
            .HasKey(roomBotParameterValue => new { roomBotParameterValue.RoomId, roomBotParameterValue.ParameterId });

        builder
            .HasOne(roomBotParameterValue => roomBotParameterValue.Room)
            .WithMany(roomsParameters => roomsParameters.ParameterValues)
            .HasForeignKey(roomBotParameterValue => roomBotParameterValue.RoomId);
    }
}