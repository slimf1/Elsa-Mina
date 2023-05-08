namespace ElsaMina.DataAccess.Models;

public class RoomParameters
{
    public string Id { get; set; }
    public bool? IsShowingErrorMessages { get; set; }
    public bool? IsCommandAutocorrectEnabled { get; set; }
    public string? Locale { get; set; }
}