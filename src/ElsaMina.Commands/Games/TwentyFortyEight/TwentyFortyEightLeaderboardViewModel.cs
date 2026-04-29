using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Games.TwentyFortyEight;

public class TwentyFortyEightLeaderboardViewModel : LocalizableViewModel
{
    public IReadOnlyList<TwentyFortyEightScore> Leaderboard { get; init; }
}
