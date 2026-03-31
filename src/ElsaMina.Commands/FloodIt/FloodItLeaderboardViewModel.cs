using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.FloodIt;

public class FloodItLeaderboardViewModel : LocalizableViewModel
{
    public IReadOnlyList<FloodItScore> Leaderboard { get; init; }
}
