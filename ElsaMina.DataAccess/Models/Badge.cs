using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("Badges")]
public class Badge
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public bool IsTrophy { get; set; }
    public ICollection<RoomSpecificUserData> BadgeHolders { get; set; }
}