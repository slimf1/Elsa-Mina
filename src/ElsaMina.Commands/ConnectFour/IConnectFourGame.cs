using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.ConnectFour;

public interface IConnectFourGame : IGame
{
    List<(int, int)> WinningLineIndices { get; }
    IUser PlayerCurrentlyPlaying { get; }
    char CurrentPlayerSymbol { get; }
    int TurnCount { get; }
    (int, int) LastPlayIndices { get; }
    char[,] Grid { get; }
    string PlayerNames { get; }
    Task DisplayAnnounce();
    Task JoinGame(IUser user);
    Task Play(IUser user, string playedColumn);
    Task Forfeit(IUser user);
    void Cancel();
}