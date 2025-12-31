using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("Users")]
public class SavedUser
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public ICollection<RoomUser> RoomData { get; set; } = [];
    public DateTimeOffset? RegisterDate { get; set; }
    public DateTimeOffset? LastOnline { get; set; }
    public string LastSeenRoomId { get; set; }
}