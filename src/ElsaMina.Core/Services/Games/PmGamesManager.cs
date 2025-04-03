using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Services.Games;

public class PmGamesManager
{
    private readonly Dictionary<IEnumerable<string>, IGame> _currentGames = [];

    public bool RegisterGame(IGame game, IEnumerable<string> players)
    {
        return true;
    }

    private bool IsUserCurrentlyInGame(string user)
    {
        var userId = user.ToLowerAlphaNum();
        return _currentGames.Any(games => games.Key.Contains(userId));
    }
}