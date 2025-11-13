using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table($"{nameof(TournamentRecord)}s")]
public class TournamentRecord : IKeyed<Tuple<string, string>>
{
    public Tuple<string, string> Key => Tuple.Create(UserId, RoomId);

    [MaxLength(255)]
    public string UserId { get; set; }
    [MaxLength(255)]
    public string RoomId { get; set; }
    public int TournamentsEnteredCount { get; set; }
    public int WinsCount { get; set; }
    public int RunnerUpCount { get; set; }
    public int PlayedGames { get; set; }
    public int WonGames { get; set; }
}