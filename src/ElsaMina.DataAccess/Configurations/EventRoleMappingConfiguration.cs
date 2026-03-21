using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElsaMina.DataAccess.Configurations;

public class EventRoleMappingConfiguration : IEntityTypeConfiguration<EventRoleMapping>
{
    public void Configure(EntityTypeBuilder<EventRoleMapping> builder)
    {
        builder.HasKey(mapping => new { mapping.EventName, mapping.RoomId });
    }
}
