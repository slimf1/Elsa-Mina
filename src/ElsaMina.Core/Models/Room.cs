using System.Globalization;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Models;

public class Room : IRoom
{
    private IGame _game;
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

    public Room(string roomTitle, string roomId, CultureInfo culture)
    {
        RoomId = roomId ?? roomTitle.ToLowerAlphaNum();
        Name = roomTitle;
        Culture = culture;
    }

    public void AddUser(string username)
    {
        var user = User.FromUsername(username);
        Users[user.UserId] = user;
    }

    public void RemoveUser(string username)
    {
        Users.Remove(username.ToLowerAlphaNum());
    }

    public void RenameUser(string oldName, string newName)
    {
        RemoveUser(oldName);
        AddUser(newName);
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