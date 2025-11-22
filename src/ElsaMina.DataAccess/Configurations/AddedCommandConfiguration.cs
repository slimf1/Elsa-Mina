using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsaMina.DataAccess.Configurations;

public class AddedCommandConfiguration : IEntityTypeConfiguration<AddedCommand>
{
    public void Configure(EntityTypeBuilder<AddedCommand> builder)
    {
        builder
            .HasOne(command => command.Room)
            .WithMany(room => room.AddedCommands)
            .HasForeignKey(command => command.RoomId);

        builder
            .HasKey(command => new { command.Id, command.RoomId });
    }
}