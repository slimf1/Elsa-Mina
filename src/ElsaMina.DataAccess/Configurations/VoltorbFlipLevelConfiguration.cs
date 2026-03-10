using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsaMina.DataAccess.Configurations;

public class VoltorbFlipLevelConfiguration : IEntityTypeConfiguration<VoltorbFlipLevel>
{
    public void Configure(EntityTypeBuilder<VoltorbFlipLevel> builder)
    {
        builder
            .HasKey(record => new { record.UserId });
    }
}