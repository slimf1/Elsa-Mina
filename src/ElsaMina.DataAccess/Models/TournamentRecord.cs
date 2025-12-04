using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("TournamentRecords")]
public class TournamentRecord
{
    public string UserId { get; set; }
    public string RoomId { get; set; }
    public int TournamentsEnteredCount { get; set; }
    public int WinsCount { get; set; }
    public int RunnerUpCount { get; set; }
    public int PlayedGames { get; set; }
    public int WonGames { get; set; }
    public RoomUser RoomUser { get; set; }
}