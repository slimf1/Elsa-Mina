using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Tournaments.Leaderboard;

public class TopTournamentPlayersViewModel : LocalizableViewModel
{
    public string Room { get; init; }
    public IEnumerable<TopTournamentPlayersEntry> TopList { get; init; }
}
