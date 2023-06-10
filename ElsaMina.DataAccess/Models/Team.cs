using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("Teams")]
public class Team : IKeyed<string>
{
    public string Key => Id;

    public Team()
    {
        Rooms = new HashSet<RoomTeam>();
    }
    
    public string Id { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    public string Link { get; set; }
    public DateTime CreationDate { get; set; }
    public string TeamJson { get; set; }
    public ICollection<RoomTeam> Rooms { get; set; }
}