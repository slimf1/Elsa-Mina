using System.Globalization;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Services.Rooms;

public class Room : IRoom
{
    private const int MESSAGE_QUEUE_LENGTH = 30;

    private readonly Dictionary<string, DateTime> _joinDateTimes = [];
    private readonly Queue<Tuple<string, string>> _lastMessages = new(MESSAGE_QUEUE_LENGTH);
    private readonly Dictionary<string, IUser> _users = new();
    private IGame _game;

    public Room(string roomTitle, string roomId, CultureInfo culture)
    {
        RoomId = roomId ?? roomTitle.ToLowerAlphaNum();
        Name = roomTitle;
        Culture = culture;
    }

    public string RoomId { get; }
    public string Name { get; }
    public IReadOnlyDictionary<string, IUser> Users => _users;
    public CultureInfo Culture { get; set; }

    public IGame Game
    {
        get => _game;
        set => OnGameChanged(_game, value);
    }

    public RoomInfo Info { get; set; }
    public IEnumerable<Tuple<string, string>> LastMessages => _lastMessages.Reverse();

    public void UpdateMessageQueue(string user, string message)
    {
        _lastMessages.Enqueue(Tuple.Create(user, message));
        if (_lastMessages.Count == MESSAGE_QUEUE_LENGTH)
        {
            _lastMessages.Dequeue();
        }
    }

    public void InitializeMessageQueueFromLogs(IEnumerable<string> logs)
    {
        var filteredMessages = logs
            .Where(line => line.StartsWith("|c:|"))
            .TakeLast(MESSAGE_QUEUE_LENGTH)
            .Select(line => line.Split("|"))
            .Select(messageParts => (messageParts[3], messageParts[4]));

        foreach (var (user, message) in filteredMessages)
        {
            _lastMessages.Enqueue(Tuple.Create(user, message));
        }
    }

    public void AddUser(string username)
    {
        var user = User.FromUsername(username);
        _users[user.UserId] = user;
        _joinDateTimes[user.UserId] = DateTime.UtcNow;
    }

    public void RemoveUser(string username)
    {
        var userId = username.ToLowerAlphaNum();
        _users.Remove(userId);
        _joinDateTimes.Remove(userId);
    }

    public void RenameUser(string oldName, string newName)
    {
        RemoveUser(oldName);
        AddUser(newName);
    }

    public void AddUsers(IEnumerable<string> users)
    {
        foreach (var user in users)
        {
            AddUser(user);
        }
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