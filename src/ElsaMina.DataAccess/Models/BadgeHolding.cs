namespace ElsaMina.DataAccess.Models;

public class BadgeHolding
{
    public string BadgeId { get; set; }
    public string RoomId { get; set; }
    public virtual Badge Badge { get; set; }
    
    public string UserId { get; set; }
    public virtual RoomUser RoomUser { get; set; }
}