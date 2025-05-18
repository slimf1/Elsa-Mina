namespace ElsaMina.DataAccess.Models;

// todo : renommer "RoomInfo" ou similaire
public class RoomInfo : IKeyed<string>
{
    public string Key => Id;

    public string Id { get; set; }
    public ICollection<RoomTeam> Teams { get; set; } = new HashSet<RoomTeam>();
    public ICollection<RoomBotParameterValue> ParameterValues { get; set; } = new HashSet<RoomBotParameterValue>();
}