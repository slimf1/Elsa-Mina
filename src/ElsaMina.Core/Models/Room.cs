using System.Globalization;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Models;

public class Room : IRoom
{
    private readonly Dictionary<string, DateTime> _joinDateTimes = [];
    private IGame _game;

    public Room(string roomTitle, string roomId, CultureInfo culture)
    {
        RoomId = roomId ?? roomTitle.ToLowerAlphaNum();
        Name = roomTitle;
        Culture = culture;
    }

    public string RoomId { get; }
    public string Name { get; }
    public IDictionary<string, IUser> Users { get; } = new Dictionary<string, IUser>();
    public CultureInfo Culture { get; set; }

    public IGame Game
    {
        get => _game;
        set => OnGameChanged(_game, value);
    }

    public RoomParameters Parameters { get; set; }

    public void AddUser(string username)
    {
        var user = User.FromUsername(username);
        Users[user.UserId] = user;
        _joinDateTimes[user.UserId] = DateTime.UtcNow;
    }

    public void RemoveUser(string username)
    {
        var userId = username.ToLowerAlphaNum();
        Users.Remove(userId);
        _joinDateTimes.Remove(userId);
    }

    public void RenameUser(string oldName, string newName)
    {
        RemoveUser(oldName);
        AddUser(newName);
    }

    public DateTime GetUserJoinDate(string username)
    {
        return _joinDateTimes.TryGetValue(username.ToLowerAlphaNum(), out var joinDate)
            ? joinDate
            : DateTime.MinValue;
    }

    private void OnGameChanged(IGame oldGame, IGame newGame)
    {
        if (oldGame != null)
        {
            oldGame.GameStarted -= HandleGameStart;
            oldGame.GameEnded -= HandleGameEnd;
        }

        if (newGame != null)
        {
            newGame.GameStarted += HandleGameStart;
            newGame.GameEnded += HandleGameEnd;
        }

        _game = newGame;
    }

    private void HandleGameEnd()
    {
        Game = null;
    }

    private void HandleGameStart()
    {
        // Do nothing
    }
}