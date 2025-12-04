using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("ArcadeLevels")]
public class ArcadeLevel
{
    public string Id { get; set; }
    public int Level { get; set; }
}
