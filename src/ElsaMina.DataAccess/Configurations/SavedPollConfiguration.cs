using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsaMina.DataAccess.Configurations;

public class  SavedPollConfiguration : IEntityTypeConfiguration<SavedPoll>
{
    public void Configure(EntityTypeBuilder<SavedPoll> builder)
    {
        builder
            .HasOne(poll => poll.Room)
            .WithMany(room => room.PollHistory);
    }
}