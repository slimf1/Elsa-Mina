namespace ElsaMina.DataAccess.Models;

public class RoomParameters : IKeyed<string>
{
    public string Key => Id;

    public RoomParameters()
    {
        Teams = new HashSet<RoomTeam>();
    }
    
    public string Id { get; set; }
    public bool? IsShowingErrorMessages { get; set; }
    public bool? IsCommandAutocorrectEnabled { get; set; }
    public string? Locale { get; set; }
    public ICollection<RoomTeam> Teams { get; set; }
}