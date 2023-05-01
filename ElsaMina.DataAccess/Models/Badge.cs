namespace ElsaMina.DataAccess.Models;

public class Badge
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public bool IsTrophy { get; set; }
    public ICollection<RoomSpecificUserData> BadgeHolders { get; set; }
}