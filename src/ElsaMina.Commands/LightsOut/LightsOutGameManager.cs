using System.Collections.Concurrent;

namespace ElsaMina.Commands.LightsOut;

public class LightsOutGameManager : ILightsOutGameManager
{
    private readonly ConcurrentDictionary<(string RoomId, string UserId), ILightsOutGame> _games = new();

    public ILightsOutGame GetGame(string roomId, string userId)
    {
        _games.TryGetValue((roomId, userId), out var game);
        return game;
    }

    public void RegisterGame(string roomId, string userId, ILightsOutGame game)
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
