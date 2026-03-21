using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsaMina.DataAccess.Configurations;

public class TourConfigConfiguration : IEntityTypeConfiguration<TourConfig>
{
    public void Configure(EntityTypeBuilder<TourConfig> builder)
    {
        builder.HasKey(tourConfig => new { tourConfig.Id, tourConfig.RoomId });
    }
}
