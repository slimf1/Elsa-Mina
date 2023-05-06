using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("AddedCommands")]
public class AddedCommand
{
    public string Id { get; set; }
    public string? Content { get; set; }
    public string? Author { get; set; }
    public DateTime? CreationDate { get; set; }
}