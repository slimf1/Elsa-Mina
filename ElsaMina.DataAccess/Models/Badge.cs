using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("Badges")]
public class Badge : IKeyed<Tuple<string, string>>
{
    public Tuple<string, string> Key => new(Id, RoomId);

    public Badge()
    {
        BadgeHolders = new HashSet<BadgeHolding>();
    }

    public string Id { get; set; }
    public string RoomId { get; set; }
    public string? Name { get; set; }
    public string? Image { get; set; }
    public bool? IsTrophy { get; set; }
    public bool? IsTeamTournament { get; set; }
    public ICollection<BadgeHolding> BadgeHolders { get; set; }
}