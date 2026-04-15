using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.ConnectFour;

public class ConnectFourLeaderboardViewModel : LocalizableViewModel
{
    public IReadOnlyList<ConnectFourRating> Leaderboard { get; init; }
}
