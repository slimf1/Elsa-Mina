using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Templates.TeamList;

public class TeamListViewModel : LocalizableViewModel
{
    public IEnumerable<Team> Teams { get; set; }
}