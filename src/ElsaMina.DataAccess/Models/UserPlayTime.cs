using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("UserPlayTimes")]
public class UserPlayTime : IKeyed<Tuple<string, string>>
{
    public Tuple<string, string> Key => Tuple.Create(UserId, RoomId);

    public string UserId { get; set; }
    public string RoomId { get; set; }
    public TimeSpan PlayTime { get; set; }
}