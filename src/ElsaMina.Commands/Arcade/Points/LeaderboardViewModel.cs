using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Arcade.Points;

public class LeaderboardViewModel : LocalizableViewModel
{
    public IReadOnlyDictionary<string, double> Leaderboard { get; init; }
}
