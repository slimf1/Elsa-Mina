using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("AddedCommands")]
public class AddedCommand : IKeyed<Tuple<string, string>>
{
    public Tuple<string, string> Key => new(Id, RoomId);

    public string Id { get; set; }
    public string RoomId { get; set; }
    public string? Content { get; set; }
    public string? Author { get; set; }
    public DateTime? CreationDate { get; set; }
}