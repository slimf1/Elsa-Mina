using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("SavedTournaments")]
public class SavedTournament
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string RoomId { get; set; }
    public SavedRoom SavedRoom { get; set; }
    public string Format { get; set; }
    public string Winner { get; set; }
    public string RunnerUp { get; set; }
    public string SemiFinalists { get; set; }
    public int PlayerCount { get; set; }
    public DateTimeOffset EndedAt { get; set; }
}
