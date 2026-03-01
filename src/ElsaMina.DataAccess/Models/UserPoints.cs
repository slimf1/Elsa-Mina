using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("UserPoints")]
public class UserPoints
{
    public string Id { get; set; }
    public double Points { get; set; }
}
