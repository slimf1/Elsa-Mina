using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("StaffIntros")]
public class StaffIntro
{
    [Key]
    public string RoomId { get; set; }
    public string HtmlContent { get; set; }
}
