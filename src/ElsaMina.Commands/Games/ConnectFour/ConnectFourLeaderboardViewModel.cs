using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Games.ConnectFour;

public class ConnectFourLeaderboardViewModel : LocalizableViewModel
{
    public IReadOnlyList<ConnectFourRating> Leaderboard { get; init; }
}
