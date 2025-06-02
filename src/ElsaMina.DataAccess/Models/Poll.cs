using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("SavedPolls")]
public class SavedPoll : IKeyed<int>
{
    public int Key => Id;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [MaxLength(50)] 
    public string RoomId { get; set; }
    public RoomInfo RoomInfo { get; set; }
    public string Content { get; set; }
    public DateTimeOffset EndedAt { get; set; }
}