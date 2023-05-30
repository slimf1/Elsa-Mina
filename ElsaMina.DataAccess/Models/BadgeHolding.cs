namespace ElsaMina.DataAccess.Models;

public class BadgeHolding : IKeyed<Tuple<string, string, string>>
{
    public Tuple<string, string, string> Key => new(BadgeId, UserId, RoomId);

    public string BadgeId { get; set; }
    public string RoomId { get; set; }
    public virtual Badge Badge { get; set; }
    
    public string UserId { get; set; }
    public virtual RoomSpecificUserData RoomSpecificUserData { get; set; }
}