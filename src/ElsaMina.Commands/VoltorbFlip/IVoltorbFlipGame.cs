using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.VoltorbFlip;

public interface IVoltorbFlipGame : IGame
{
    int Level { get; }
    bool IsRoundActive { get; }
    int CurrentCoins { get; }
    int[,] TileValues { get; }
    bool[,] IsRevealed { get; }
    int[] RowSums { get; }
    int[] ColSums { get; }
    int[] RowVoltorbs { get; }
    int[] ColVoltorbs { get; }
    bool IsEnded { get; }
    IContext Context { get; set; }
    IUser Owner { get; set; }

    Task StartNewRound();
    Task FlipTile(IUser user, int row, int col);
    Task QuitRound(IUser user);
    void Cancel();
}
