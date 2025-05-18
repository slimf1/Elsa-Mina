using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("RoomTeams")]
public class RoomTeam
{
    public string TeamId { get; set; }
    public Team Team { get; set; }
    
    public string RoomId { get; set; }
    public RoomInfo RoomInfo { get; set; }
}