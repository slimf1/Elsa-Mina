using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("SavedPolls")]
public class SavedPoll
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string RoomId { get; set; }
    public Room Room { get; set; }
    public string Content { get; set; }
    public DateTimeOffset EndedAt { get; set; }
}