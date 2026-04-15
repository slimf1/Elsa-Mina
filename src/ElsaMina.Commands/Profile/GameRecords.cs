using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Profile;

public class GameRecords
{
    public FloodItScore FloodIt { get; init; }
    public LightsOutScore LightsOut { get; init; }
    public VoltorbFlipLevel VoltorbFlip { get; init; }
    public TwentyFortyEightScore TwentyFortyEight { get; init; }
    public ConnectFourRating ConnectFour { get; init; }

    public bool HasAnyRecord => FloodIt != null || LightsOut != null || VoltorbFlip != null
                                || TwentyFortyEight != null || ConnectFour != null;
}
