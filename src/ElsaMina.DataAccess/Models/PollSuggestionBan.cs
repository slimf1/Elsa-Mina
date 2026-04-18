using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("PollSuggestionBans")]
public class PollSuggestionBan
{
    public string UserId { get; set; }
    public string RoomId { get; set; }
}
