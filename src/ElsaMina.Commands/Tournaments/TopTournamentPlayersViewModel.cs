using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Tournaments;

public class TopTournamentPlayersViewModel : LocalizableViewModel
{
    public string Room { get; init; }
    public IEnumerable<TopTournamentPlayersEntry> TopList { get; init; }
}
