using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Arcade.Points;

public class PointsUpdateViewModel : LocalizableViewModel
{
    public string Username { get; init; }
    public double PointsAdded { get; init; }
    public double NewTotal { get; init; }
    public bool IsAddition { get; init; }
    public IReadOnlyDictionary<string, double> Leaderboard { get; init; }
}
