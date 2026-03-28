using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.LightsOut;

public class LightsOutLeaderboardViewModel : LocalizableViewModel
{
    public IReadOnlyList<LightsOutScore> Leaderboard { get; init; }
}
