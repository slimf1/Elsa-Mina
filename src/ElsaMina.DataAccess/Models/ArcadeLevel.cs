using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("ArcadeLevels")]
public class ArcadeLevel : IKeyed<string>
{
    public string Key => Id;
    
    [MaxLength(20)]
    public string Id { get; set; }
    public int Level { get; set; }
}
