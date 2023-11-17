using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("Repeats")]
public class Repeat : IKeyed<Tuple<string, string>>
{
    public Tuple<string, string> Key => new(RoomId, Name);
    
    public string RoomId { get; set; }
    public string Name { get; set; }
    public uint Delay { get; set; }
    public DateTime CreatedAt { get; set; }
}