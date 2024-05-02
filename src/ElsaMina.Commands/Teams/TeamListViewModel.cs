using ElsaMina.Core.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Teams;

public class TeamListViewModel : LocalizableViewModel
{
    public IEnumerable<Team> Teams { get; set; }
}