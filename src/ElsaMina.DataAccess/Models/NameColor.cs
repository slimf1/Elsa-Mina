using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("NameColors")]
public class NameColor
{
    [Key]
    public string UserId { get; set; }
    public string Color { get; set; }
}
