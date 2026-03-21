using System.Collections.Concurrent;

namespace ElsaMina.Commands.VoltorbFlip;

public class VoltorbFlipGameManager : IVoltorbFlipGameManager
{
    private readonly ConcurrentDictionary<(string RoomId, string UserId), IVoltorbFlipGame> _games = new();

    public IVoltorbFlipGame GetGame(string roomId, string userId)
    {
        _games.TryGetValue((roomId, userId), out var game);
        return game;
    }

    public void RegisterGame(string roomId, string userId, IVoltorbFlipGame game)
    {
        var key = (roomId, userId);
        _games[key] = game;
        game.GameEnded += () => _games.TryRemove(key, out _);
    }

    public void RemoveGame(string roomId, string userId)
    {
        _games.TryRemove((roomId, userId), out _);
    }
}
