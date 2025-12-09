using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsaMina.DataAccess.Configurations;

public class SavedUserConfiguration : IEntityTypeConfiguration<SavedUser>
{
    public void Configure(EntityTypeBuilder<SavedUser> builder)
    {
        builder
            .HasKey(user => user.UserId);

        builder
            .HasMany(user => user.RoomData)
            .WithOne(roomUser => roomUser.User)
            .HasForeignKey(roomUser => roomUser.Id)
            .IsRequired(false);
    }
}