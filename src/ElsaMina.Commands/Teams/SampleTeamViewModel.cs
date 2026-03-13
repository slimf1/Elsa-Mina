using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Teams;

public class SampleTeamViewModel : LocalizableViewModel
{
    public Team Team { get; init; }
    public TimeZoneInfo TimeZone { get; init; } 
}