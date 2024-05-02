using ElsaMina.Core.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Teams;

public class SampleTeamViewModel : LocalizableViewModel
{
    public Team Team { get; set; }
}