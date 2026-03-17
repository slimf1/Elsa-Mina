using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.VoltorbFlip;

public class VoltorbFlipLeaderboardViewModel : LocalizableViewModel
{
    public IReadOnlyList<VoltorbFlipLevel> Leaderboard { get; init; }
}
