using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsaMina.DataAccess.Configurations;

public class PollSuggestionBanConfiguration : IEntityTypeConfiguration<PollSuggestionBan>
{
    public void Configure(EntityTypeBuilder<PollSuggestionBan> builder)
    {
        builder.HasKey(ban => new { ban.UserId, ban.RoomId });
    }
}
