using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("EventRoleMappings")]
public class EventRoleMapping
{
    public string EventName { get; set; }
    public string RoomId { get; set; }
    public string DiscordRoleId { get; set; }
}
