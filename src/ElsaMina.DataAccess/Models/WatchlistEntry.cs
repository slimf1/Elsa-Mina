using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("WatchlistEntries")]
public class WatchlistEntry
{
    public string RoomId { get; set; }
    public string UserId { get; set; }
    public string Rank { get; set; }
}
